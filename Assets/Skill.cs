using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GridRPG {
    /// <summary>
    /// Manages the data of all the Skills in the game.
    /// </summary>
    public class SkillLibrary
    {
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
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if(values.Length < 6)
                    {
                        throw new ArgumentException("SkillLibrary: Entry in file is invalid.");
                    }
                    int skillId = int.Parse(values[0]);
                    Type skillType = Type.GetType(values[1]);
                    string skillName = values[2];
                    int skillRange = int.Parse(values[3]);
                    int skillSize = int.Parse(values[4]);
                    Skill.Shape skillShape = Skill.stringToShape(values[5]);
                    //Get tags from the remainder of the line
                    List<string> skillTagList = new List<string>();
                    for(int i = 6; i < values.Length; i++)
                    {
                        skillTagList.Add(values[i]);
                    }
                    Skill.Parameters skillParameters = new Skill.Parameters(skillName, skillTagList, skillRange, skillSize, skillShape);
                    Debug.Log("SkillLibrary(string): Attempting to add " + values[1]);
                    add(skillType, skillParameters,skillId);
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
                if(library[id] != null)
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

        /// <summary>
        /// Adds the skill targetting information to a static location.
        /// </summary>
        /// <param name="T">The skill type.</param>
        /// <param name="tags">The skill's tags.</param>
        /// <param name="range">The skill's range.</param>
        /// <param name="radius">The radius of the skill's effect.</param>
        /// <param name="shape">Shape of the skill.</param>
        private void initializeSkillParameters(Type T, string name, List<string> tags, int range, int size, Skill.Shape shape)
        {
            if (T.IsSubclassOf(typeof(Skill)) && tags != null && !skillParameterList.ContainsKey(T))
            {
                Skill.Parameters sp = new Skill.Parameters(name, tags, range, size, shape);
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

            public Parameters(string name, List<string> tags,int range, int size, Shape shape)
            {
                this.name = name;
                this.tags = tags;
                this.range = range;
                this.size = size;
                this.shape = shape;
            }
        }
        public enum Shape { Single, Square, Circle, Cone, Line }

        protected const string SKILL_LAYER = "Effect";
        protected GameObject user = null;
        public Vector2 target = Vector2.zero;
        public delegate void CompletedEvent(Skill skill);
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

        public static Skill activateSkill<T>(GameObject user, Vector2 target) where T : Skill
        {
            Debug.Log("Attempting to activate skill");
            if (user != null && typeof(T).IsSubclassOf(typeof(Skill)) && SkillLibrary.skillParameterList.ContainsKey(typeof(T)))
            {
                Debug.Log("Activating skill: " + typeof(T).ToString());
                GameObject skillObject = new GameObject(user.name + "'s " + typeof(T).ToString() + " skill");
                skillObject.AddComponent<T>();
                skillObject.GetComponent<T>().initialize(user, target);
                return skillObject.GetComponent<T>();
            }
            return null;
        }
    }

    /// <summary>
    /// Attacks an adjacent enemy for physical damage.
    /// </summary>
    public class MeleeAttack : Skill
    {
        private bool skillDone = false;
        Unit sourceUnit = null;
        Unit targetUnit = null;

        public override bool initialize(GameObject user, Vector2 target)
        {
            if ((sourceUnit = user?.GetComponent<Unit>()) != null)
            {
                this.target = target;
                this.targetUnit = Game.map?.GetComponent<Map>().getUnitOnSpace(target);
                this.user = user;
                return true;
            }
            else
            {
                sourceUnit = null;
                targetUnit = null;
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
            Unit sourceUnitArg = skillSource?.GetComponent<Unit>();
            if(sourceUnitArg != null)
            {
                //source unit exists
                return Space.isAdjacent(sourceUnit.spaceCoords, targetLocation);
            }
            return false;
        }

        private int damageCalculation()
        {
            return (int)sourceUnit.getDamageMod("physical").applyTo((int)sourceUnit.stats.strength);
        }

        private void Update()
        {
            if(!skillDone)
            {
                if(sourceUnit!=null)
                {
                    //ready to activate
                    if (targetUnit != null && Space.isAdjacent(sourceUnit.spaceCoords, targetUnit.spaceCoords))
                    {
                        //target found in range
                        int damage = damageCalculation();
                        if (damage < 0)
                        {
                            //cannot deal less than 0 damage
                            damage = 0;
                        }
                        int damageDealt = targetUnit.takeDamage((uint)damage, "physical");
                        Game.ui.displayMessage(sourceUnit.name + "'s attack dealt " + damageDealt + " damage to " + targetUnit.name + ".");
                        //Game.ui.displayMessage("Attacking for "+damageDealt);
                        Game.ui.updateUnitFrame(targetUnit);
                    }
                    else
                    {
                        //no target unit or target space is out of range.
                        Game.ui.displayMessage(sourceUnit.name + "'s attack failed to hit a target.");
                    }
                    skillDone = true;
                }
            }
            else
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
    /// <summary>
    /// Attacks an enemy in range for fire damage.
    /// </summary>
    public class FireBlast : Skill
    {
        private const string FIRE_BLAST_SHEET = "Sprites/Skill/FireBlast";

        private bool skillDone = false;
        Unit sourceUnit = null;
        Unit targetUnit = null;

        public override bool initialize(GameObject user, Vector2 target)
        {
            if ((sourceUnit = user?.GetComponent<Unit>()) != null)
            {
                this.target = target;
                this.targetUnit = Game.map?.GetComponent<Map>().getUnitOnSpace(target);
                GetComponent<SpriteRenderer>().sortingLayerName = SKILL_LAYER;
                this.user = user;
                setupAnimation();
                transform.position = sourceUnit.transform.position;
                return true;
            }
            else
            {
                sourceUnit = null;
                targetUnit = null;
                return false;
            }
        }

        private void setupAnimation()
        {
            Texture2D spriteSheet = Resources.Load<Texture2D>(FIRE_BLAST_SHEET);
            if(spriteSheet == null)
            {
                Debug.Log("FireBlast: could not load spritesheet");
            }
            List<int> spriteOrder = new List<int>();
            spriteOrder.Add(0);
            GetComponent<AnimationManager>().addAnimation(spriteSheet, new Vector2(32, 32), new Vector2(1, 1), spriteOrder, 0);
            GetComponent<AnimationManager>().CurrentAnimationId = 0;
        }

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

        public override bool isTargetValid(GameObject skillSource, Vector2 targetLocation)
        {
            if(skillSource?.GetComponent<Unit>() != null)
            {
                Vector2 sourceLocation = skillSource.GetComponent<Unit>().spaceCoords;
                return Game.map.GetComponent<Map>().inRange(sourceLocation, targetLocation, 1, 4);
            }
            else
            {
                return false;
            }
        }

        private int damageCalculation()
        {
            if (sourceUnit != null)
            {
                return (int)sourceUnit.getDamageMod("fire").applyTo((int)sourceUnit.stats.intellect);
            }
            else
            {
                return 0;
            }
        }

        private void Update()
        {
            if (!skillDone)
            {
                if (sourceUnit != null && !Game.animationInProgress)
                {
                    //ready to activate
                    if (isTargetValid(sourceUnit.gameObject,target))
                    {
                        //target found in range
                        GetComponent<AnimationManager>().changeMovement(1, target);
                        GetComponent<AnimationManager>().destinationReached += hitTarget;
                        Game.animationInProgress = true;
                    }
                    else
                    {
                        //no target unit or target space is out of range.
                        Game.ui.displayMessage(sourceUnit.name + "'s spell failed to find a target.");
                        skillDone = true;
                    }
                }
            }
            else
            {
                this.complete?.Invoke(this);
                GameObject.Destroy(gameObject);
            }
        }

        private void hitTarget()
        {
            int damage = damageCalculation();
            if (damage < 0)
            {
                //cannot deal less than 0 damage
                damage = 0;
            }
            int damageDealt = targetUnit.takeDamage((uint)damage, "fire");
            Game.ui.displayMessage(sourceUnit.name + "'s Fire Blast dealt " + damageDealt + " damage to " + targetUnit.name + ".");
            //Game.ui.displayMessage("Attacking for "+damageDealt);
            //Game.ui.updateUnitFrame(targetUnit);
            Game.animationInProgress = false;
            GetComponent<AnimationManager>().destinationReached -= hitTarget;
            skillDone = true;
        }
    }

    /*public abstract class Skill
    {
        public string name { get; protected set; }
        public List<string> tags { get; protected set; }
        public string description { get; protected set; }
        public int range { get; protected set; }
        public List<Effect> effects { get; protected set; }

        public abstract bool activate(GameObject user, Vector2 target);
        public abstract string getDescription(GameObject user);
    }*/

    /*public class MeleeAttack : Skill
    {
        public MeleeAttack()
        {
            name = "MeleeAttack";
            tags = new List<string>();
            tags.Add("physical");
            tags.Add("contact");
        }

        public override string getDescription(GameObject user)
        {
            return "Deals " + user.GetComponent<Unit>().stats.strength + "physical damage to an adjacent enemy";
        }

        public override bool activate(GameObject user, Vector2 target)
        {
            Unit currentUnit = user?.GetComponent<Unit>();
            Unit targetUnit = Game.map.getSpace(target)?.unit?.GetComponent<Unit>();
            if (currentUnit == null || targetUnit == null)
            {
                return false;
            }
            else
            {
                //todo
                return true;
            }
        }
    }*/
}
