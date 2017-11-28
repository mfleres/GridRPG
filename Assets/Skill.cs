using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace GridRPG
{
    /// <summary>
    /// Manages the data of all the Skills in the game.
    /// </summary>
    public class SkillLibrary
    {
        protected const float ICON_DIMS = 32f;
        private List<Type> library;
        public static Dictionary<Type, Skill.Parameters> skillParameterList = new Dictionary<Type, Skill.Parameters>();

        public SkillLibrary()
        {
            library = new List<Type>();
        }

        public SkillLibrary(string filename)
        {
            library = new List<Type>();
            TextAsset file = Resources.Load<TextAsset>(filename);
            if (file == null)
            {
                Debug.Log("File read fail");
                throw new ArgumentException("File " + filename + " could not be read");
            }
            System.IO.StringReader reader = new System.IO.StringReader(file.text);

            if (reader != null)
            {
                string line;
                if ((line = reader.ReadLine()) != null) //Skip the header line
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        if (values.Length < 6)
                        {
                            throw new ArgumentException("SkillLibrary: Entry in file is invalid.");
                        }
                        int skillId = int.Parse(values[0]);
                        Type skillType = Type.GetType(values[1]);
                        string skillName = values[2];
                        int skillRange = int.Parse(values[3]);
                        int skillSize = int.Parse(values[4]);
                        Skill.Shape skillShape = Skill.stringToShape(values[5]);
                        Texture2D skillSpriteSheet = Resources.Load<Texture2D>(values[6]);
                        int skillSpriteX = int.Parse(values[7]);
                        int skillSpriteY = int.Parse(values[8]);
                        Sprite skillSprite = Sprite.Create(skillSpriteSheet, new Rect((float)skillSpriteX, (float)skillSpriteY, ICON_DIMS, ICON_DIMS), new Vector2(0.5f, 0.5f));
                        //Get tags from the remainder of the line
                        List<string> skillTagList = new List<string>();
                        for (int i = 9; i < values.Length; i++)
                        {
                            skillTagList.Add(values[i]);
                        }
                        Skill.Parameters skillParameters = new Skill.Parameters(skillName, skillTagList, skillRange, skillSize, skillShape, skillSprite);
                        Debug.Log("SkillLibrary(string): Attempting to add " + values[1]);
                        add(skillType, skillParameters, skillId);
                    }
                }
            }
        }

        public int add(Type skillType, Skill.Parameters parameters)
        {
            if (Skill.isSkill(skillType) && !skillParameterList.ContainsKey(skillType))
            {
                //skillType is both a Skill and not already in the library.
                skillParameterList.Add(skillType, parameters);
                for (int i = 0; i < library.Count; i++)
                {
                    if (library[i] != null)
                    {
                        library[i] = skillType;
                        return i;
                    }
                }
                library.Add(skillType);
                Debug.Log("SkillLibrary.add(Type,Skill.Parameters): Successfolly added skill: " + skillType.ToString());
                return library.Count - 1;
            }
            return -1;
        }

        public int add(Type skillType, Skill.Parameters parameters, int id)
        {
            if (Skill.isSkill(skillType) && !skillParameterList.ContainsKey(skillType))
            {
                //skillType is both a Skill and not already in the library.
                while (id >= library.Count)
                {
                    //expand library to fit the skill
                    library.Add(null);
                }
                if (library[id] != null)
                {
                    //remove old entry
                    skillParameterList.Remove(library[id]);
                    library[id] = null;
                }
                library[id] = skillType;
                skillParameterList.Add(skillType, parameters);
                Debug.Log("SkillLibrary.add(Type,Skill.Parameters,int): Successfolly added skill: " + skillType.ToString());
                return id;
            }
            return -1;
        }

        public Sprite getIcon(Type T)
        {
            if (T != null)
            {
                Skill.Parameters ret = skillParameterList[T];
                return ret.icon;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds the skill targetting information to a static location.
        /// </summary>
        /// <param name="T">The skill type.</param>
        /// <param name="tags">The skill's tags.</param>
        /// <param name="range">The skill's range.</param>
        /// <param name="radius">The radius of the skill's effect.</param>
        /// <param name="shape">Shape of the skill.</param>
        private void initializeSkillParameters(Type T, string name, List<string> tags, int range, int size, Skill.Shape shape, Sprite icon)
        {
            if (T.IsSubclassOf(typeof(Skill)) && tags != null && !skillParameterList.ContainsKey(T))
            {
                Skill.Parameters sp = new Skill.Parameters(name, tags, range, size, shape, icon);
                skillParameterList.Add(T, sp);
            }
        }
    }

    [RequireComponent(typeof(AnimationManager))]
    public abstract class Skill : MonoBehaviour
    {
        public struct Parameters
        {
            public string name;
            public List<string> tags;
            public int range;
            public int size;
            public Shape shape;
            public Sprite icon;

            public Parameters(string name, List<string> tags, int range, int size, Shape shape, Sprite icon)
            {
                this.name = name;
                this.tags = tags;
                this.range = range;
                this.size = size;
                this.shape = shape;
                this.icon = icon;
            }
        }
        public enum Shape { Single, Square, Circle, Cone, Line }

        protected const string SKILL_LAYER = "Effect";

        protected GameObject user = null;
        public Vector2 target = Vector2.zero;
        public delegate void CompletedEvent(Skill skill, bool success);
        public CompletedEvent complete;

        /// <summary>
        /// Initializes the skill to be active.
        /// </summary>
        /// <param name="user">Source of the skill's activation.</param>
        /// <param name="target">Target of the skill's effect.</param>
        /// <returns></returns>
        public abstract bool initialize(GameObject user, Vector2 target);
        /// <summary>
        /// Displays the description of the skill
        /// </summary>
        /// <returns></returns>
        public abstract string getDescription();
        /// <summary>
        /// Determines if the selected target is valid.
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public abstract bool isTargetValid(GameObject skillSource, Vector2 targetLocation);

        /// <summary>
        /// </summary>
        /// <param name="str">Name of the shape (case sensitive).</param>
        /// <returns>The Shape. Defaults to Single.</returns>
        public static Shape stringToShape(string str)
        {
            if (str != null)
            {
                switch (str)
                {
                    case "Single": return Shape.Single;
                    case "Square": return Shape.Square;
                    case "Circle": return Shape.Circle;
                    case "Cone": return Shape.Cone;
                    case "Line": return Shape.Line;
                }
            }
            return Shape.Single;
        }

        /// <summary>
        /// Tests if a given type is a skill.
        /// </summary>
        /// <param name="T">Type to test.</param>
        /// <returns></returns>
        public static bool isSkill(Type T)
        {
            if (T != null)
            {
                return T.IsSubclassOf(typeof(Skill));
            }
            else
            {
                return false;
            }
        }

        public static Skill genericActivateSkill<T>(GameObject user, Vector2 target) where T : Skill
        {
            //Debug.Log("Attempting to activate skill");
            if (user != null && typeof(T).IsSubclassOf(typeof(Skill)) && SkillLibrary.skillParameterList.ContainsKey(typeof(T)))
            {
                //Debug.Log("Activating skill: " + typeof(T).ToString());
                GameObject skillObject = new GameObject(user.name + "'s " + typeof(T).ToString() + " skill");
                skillObject.AddComponent<T>();
                if (!(skillObject.GetComponent<T>().initialize(user, target)))
                {
                    //Initialization failed
                    Destroy(skillObject);
                    return null;
                }
                return skillObject.GetComponent<T>();
            }
            return null;
        }

        public static Skill activateSkill(Type skill, GameObject user, Vector2 target)
        {
            MethodInfo method = typeof(Skill).GetMethod("genericActivateSkill");
            MethodInfo generic = method.MakeGenericMethod(skill);
            object[] parameters = new object[] { user, target };
            Skill activatedSkill = (Skill)generic.Invoke(null, parameters);
            return activatedSkill;
        }
    }

    /// <summary>
    /// Attacks an adjacent enemy for physical damage.
    /// </summary>
    public class MeleeAttack : Skill
    {
        private const string SPRITE_SHEET = "Sprites/Skill/MeleeAttack";
        private bool success = false;
        Unit sourceUnit = null;
        Unit targetUnit = null;

        public override bool initialize(GameObject user, Vector2 target)
        {
            if (isTargetValid(user, target)) //Test if user is an existing unit, if the target is valid
            {
                this.user = user;
                this.target = target;
                this.sourceUnit = user.GetComponent<Unit>();
                this.targetUnit = Game.map?.GetComponent<Map>().getUnitOnSpace(target);
                setupAnimation();
                Game.animationInProgress = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string getDescription()
        {
            int damage = (int)sourceUnit.getDamageMod("physical").applyTo((int)sourceUnit.stats.strength);
            return "Deals " + damageCalculation() + " physical damage to an adjacent enemy.";
        }

        public override bool isTargetValid(GameObject skillSource, Vector2 targetLocation)
        {
            if (skillSource?.GetComponent<Unit>() != null)
            {
                Vector2 sourceLocation = skillSource.GetComponent<Unit>().spaceCoords;
                bool targetInRange = Game.map.GetComponent<Map>().inRange(sourceLocation, targetLocation, 1, 1);
                bool targetIsUnit = Game.map.GetComponent<Map>().getUnitOnSpace(targetLocation) != null;
                return targetInRange && targetIsUnit;
            }
            else
            {
                //sourceUnit is invalid
                return false;
            }
        }

        private int damageCalculation()
        {
            if (sourceUnit != null)
            {
                int damage = (int)sourceUnit.getDamageMod("physical").applyTo((int)sourceUnit.stats.strength);
                if (damage < 0)
                {
                    //cannot deal less than 0 damage
                    damage = 0;
                }
                return damage;
            }
            else
            {
                return 0;
            }
        }

        private void setupAnimation()
        {
            //Set the starting location for the skill graphic.
            transform.position = targetUnit.transform.position;
            //Place the graphic in the correct layer.
            GetComponent<SpriteRenderer>().sortingLayerName = SKILL_LAYER;

            //Load the spritesheet
            Texture2D spriteSheet = Resources.Load<Texture2D>(SPRITE_SHEET);
            if (spriteSheet == null)
            {
                Debug.Log("MeleeAttack: could not load spritesheet");
                //May want to throw some exceptions here.
            }

            //Setup the sprite order for the animation manager.
            List<int> spriteOrder = new List<int>();
            spriteOrder.Add(1);
            spriteOrder.Add(0);

            //Setup the animation manager.
            GetComponent<AnimationManager>().addAnimation(spriteSheet, new Vector2(32, 32), new Vector2(2, 1), spriteOrder, 10);
            //Setup graphic movement.
            // None
            //Watch for when the graphic completes the animation.
            GetComponent<AnimationManager>().animationDone += hitTarget;
            //Activate the animation by setting the current animation id.
            GetComponent<AnimationManager>().CurrentAnimationId = 0;
        }

        private void hitTarget(int id)
        {
            //Calculate the damage.
            int damage = damageCalculation();
            //Deal the damage.
            int damageDealt = targetUnit.takeDamage((uint)damage, "physical");
            //Display the message describing what happened.
            Game.ui.displayMessage(sourceUnit.name + "'s attack dealt " + damageDealt + " damage to " + targetUnit.name + ".");

            //Clean up.
            Game.animationInProgress = false;
            GetComponent<AnimationManager>().animationDone -= hitTarget;
            success = true;
            GameObject.Destroy(this.gameObject);
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void OnDestroy()
        {
            //Let everyone know that you are done.
            this.complete?.Invoke(this, success);
        }
    }
    /// <summary>
    /// Attacks an enemy in range for fire damage.
    /// </summary>
    public class FireBlast : Skill
    {
        private const string SPRITE_SHEET = "Sprites/Skill/FireBlast"; //Spritesheet for animation here

        private bool success = false;
        Unit sourceUnit = null;
        Unit targetUnit = null;

        /// <summary>
        /// Sets the skill data so it can be run.
        /// Setup animation.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool initialize(GameObject user, Vector2 target)
        {
            if (isTargetValid(user, target)) //Test if user is an existing unit, if the target is valid
            {
                this.user = user;
                this.target = target;
                this.sourceUnit = user.GetComponent<Unit>();
                this.targetUnit = Game.map?.GetComponent<Map>().getUnitOnSpace(target);
                setupAnimation();
                Game.animationInProgress = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the description of the skill.
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            if (sourceUnit != null)
            {
                int damage = damageCalculation();
                return "Deals " + damageCalculation() + " fire damage to an enemy within 4 spaces.";
            }
            else
            {
                return "Deals fire damage to an enemy within 4 spaces";
            }
        }

        /// <summary>
        /// Determines if the skill can be active.
        /// </summary>
        /// <param name="skillSource"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public override bool isTargetValid(GameObject skillSource, Vector2 targetLocation)
        {
            if (skillSource?.GetComponent<Unit>() != null)
            {
                Vector2 sourceLocation = skillSource.GetComponent<Unit>().spaceCoords;
                bool targetInRange = Game.map.GetComponent<Map>().inRange(sourceLocation, targetLocation, 1, 4);
                bool targetIsUnit = Game.map.GetComponent<Map>().getUnitOnSpace(targetLocation) != null;
                return targetInRange && targetIsUnit;
            }
            else
            {
                //sourceUnit is invalid
                return false;
            }
        }

        /// <summary>
        /// Setup the graphical display of the skill.
        /// </summary>
        private void setupAnimation()
        {
            //Set the starting location for the skill graphic.
            transform.position = sourceUnit.transform.position;
            //Place the graphic in the correct layer.
            GetComponent<SpriteRenderer>().sortingLayerName = SKILL_LAYER;

            //Load the spritesheet
            Texture2D spriteSheet = Resources.Load<Texture2D>(SPRITE_SHEET);
            if (spriteSheet == null)
            {
                Debug.Log("FireBlast: could not load spritesheet");
                //May want to throw some exceptions here.
            }

            //Setup the sprite order for the animation manager.
            List<int> spriteOrder = new List<int>();
            spriteOrder.Add(0);

            //Setup the animation manager.
            GetComponent<AnimationManager>().addAnimation(spriteSheet, new Vector2(32, 32), new Vector2(1, 1), spriteOrder, 0);
            //Setup graphic movement.
            GetComponent<AnimationManager>().changeMovement(1, target);
            //Watch for when the graphic reaches the destination.
            GetComponent<AnimationManager>().destinationReached += hitTarget;
            //Activate the animation by setting the current animation id.
            GetComponent<AnimationManager>().CurrentAnimationId = 0;
        }

        /// <summary>
        /// Calculate damage before the target's resistances.
        /// </summary>
        /// <returns></returns>
        private int damageCalculation()
        {
            if (sourceUnit != null)
            {
                int damage = (int)sourceUnit.getDamageMod("fire").applyTo((int)sourceUnit.stats.intellect);
                if (damage < 0)
                {
                    //cannot deal less than 0 damage
                    damage = 0;
                }
                return damage;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Script to run when the animation's event triggers
        /// </summary>
        private void hitTarget(int id)
        {
            //Calculate the damage.
            int damage = damageCalculation();
            //Deal the damage.
            int damageDealt = targetUnit.takeDamage((uint)damage, "fire");
            //Display the message describing what happened.
            Game.ui.displayMessage(sourceUnit.name + "'s Fire Blast dealt " + damageDealt + " damage to " + targetUnit.name + ".");

            //Clean up.
            Game.animationInProgress = false;
            GetComponent<AnimationManager>().destinationReached -= hitTarget;
            success = true;
            GameObject.Destroy(this.gameObject);
        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void Start()
        {
            //Place code you want to occur before the animation.
        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void OnDestroy()
        {
            //Let everyone know that you are done.
            this.complete?.Invoke(this, success);
        }       
    }

    /// <summary>
    /// Example skill template
    /// </summary>
    public class DummySkill : Skill
    {
        private const string SPRITE_SHEET = "Sprites/Skill/FireBlast"; //Spritesheet for animation here

        private bool success = false;
        Unit sourceUnit = null;
        Unit targetUnit = null;

        /// <summary>
        /// Sets the skill data so it can be run.
        /// Setup animation.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool initialize(GameObject user, Vector2 target)
        {
            if (isTargetValid(user, target)) //Test if user is an existing unit, if the target is valid
            {
                this.user = user;
                this.target = target;
                this.sourceUnit = user.GetComponent<Unit>();
                this.targetUnit = Game.map?.GetComponent<Map>().getUnitOnSpace(target);
                setupAnimation();
                Game.animationInProgress = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Setup the graphical display of the skill.
        /// </summary>
        private void setupAnimation()
        {
            //Set the starting location for the skill graphic.
            transform.position = sourceUnit.transform.position;
            //Place the graphic in the correct layer.
            GetComponent<SpriteRenderer>().sortingLayerName = SKILL_LAYER;

            //Load the spritesheet
            Texture2D spriteSheet = Resources.Load<Texture2D>(SPRITE_SHEET);
            if (spriteSheet == null)
            {
                Debug.Log("DummySkill: could not load spritesheet");
                //May want to throw some exceptions here.
            }

            //Setup the sprite order for the animation manager.
            List<int> spriteOrder = new List<int>();
            spriteOrder.Add(0);

            //Setup the animation manager.
            GetComponent<AnimationManager>().addAnimation(spriteSheet, new Vector2(32, 32), new Vector2(1, 1), spriteOrder, 0);
            //Setup graphic movement.
            GetComponent<AnimationManager>().changeMovement(1, target);
            //Watch for when the graphic reaches the destination.
            GetComponent<AnimationManager>().destinationReached += hitTarget;
            //Activate the animation by setting the current animation id.
            GetComponent<AnimationManager>().CurrentAnimationId = 0;

        }

        /// <summary>
        /// Returns the description of the skill.
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            if (sourceUnit != null)
            {
                int damage = damageCalculation();
                return "Deals " + damageCalculation() + " fire damage to an enemy within 4 spaces.";
            }
            else
            {
                return "Deals fire damage to an enemy within 4 spaces";
            }
        }

        /// <summary>
        /// Determines if the skill can be active.
        /// </summary>
        /// <param name="skillSource"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public override bool isTargetValid(GameObject skillSource, Vector2 targetLocation)
        {
            if (skillSource?.GetComponent<Unit>() != null)
            {
                Vector2 sourceLocation = skillSource.GetComponent<Unit>().spaceCoords;
                bool targetInRange = Game.map.GetComponent<Map>().inRange(sourceLocation, targetLocation, 1, 4);
                bool targetIsUnit = Game.map.GetComponent<Map>().getUnitOnSpace(targetLocation) != null;
                return targetInRange && targetIsUnit;
            }
            else
            {
                //sourceUnit is invalid
                return false;
            }
        }

        /// <summary>
        /// Calculate damage before the target's resistances.
        /// </summary>
        /// <returns></returns>
        private int damageCalculation()
        {
            if (sourceUnit != null)
            {
                int damage = (int)sourceUnit.getDamageMod("fire").applyTo((int)sourceUnit.stats.intellect);
                if (damage < 0)
                {
                    //cannot deal less than 0 damage
                    damage = 0;
                }
                return damage;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void Start()
        {
            //Place code you want to occur before the animation.
        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Inherited from MonoBehavior
        /// </summary>
        private void OnDestroy()
        {
            //Let everyone know that you are done.
            this.complete?.Invoke(this, success);
        }

        /// <summary>
        /// Script to run when the animation's event triggers
        /// </summary>
        private void hitTarget(int id)
        {
            //Calculate the damage.
            int damage = damageCalculation();
            //Deal the damage.
            int damageDealt = targetUnit.takeDamage((uint)damage, "fire");
            //Display the message describing what happened.
            Game.ui.displayMessage(sourceUnit.name + "'s Fire Blast dealt " + damageDealt + " damage to " + targetUnit.name + ".");
            
            //Clean up.
            Game.animationInProgress = false;
            GetComponent<AnimationManager>().destinationReached -= hitTarget;
            success = true;
            GameObject.Destroy(this.gameObject);
        }
    }
}