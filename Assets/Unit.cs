using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

/*namespace GridRPG_OLD
{
	public abstract class Unit
	{
		protected const string test_unit_filepath = "Sprites/Unit/TestUnit";
		public const int layer = 4; //need to remove this
        public const string UNIT_LAYER = "Unit";
        public const float UNIT_SPRITE_SIZE = 32f;
		
		public string name;
        public List<KeyValuePair<String,float>> mobility;
		public Space space;
		public GameObject core;

        protected struct Stats_S
        {
            public uint maxHP;
            public uint HP;
            public uint maxMP;
            public uint MP;
            public uint constitution;
            public uint strength;
            public uint dexterity;
            public uint intellect;
            public uint charisma;
            public uint agility;

            public Stats_S(uint maxHP, uint HP, uint maxMP, uint MP, uint constitution, uint strength, uint dexterity, uint intellect, uint charisma, uint agility)
            {
                this.maxHP = maxHP;
                this.HP = HP;
                this.maxMP = maxMP;
                this.MP = MP;
                this.constitution = constitution;
                this.strength = strength;
                this.dexterity = dexterity;
                this.intellect = intellect;
                this.charisma = charisma;
                this.agility = agility;
            }
        }
        protected Stats_S stats;
        protected struct Modifier
        {
            public float scalar;
            public int constant;

            public Modifier(int constant, float scalar)
            {
                this.constant = constant;
                this.scalar = scalar;
            }

            public static Modifier neutral()
            {
                return new Modifier(0, 1f);
            }

            /// <summary>
            /// Loads a modifier from a child node
            /// </summary>
            /// <param name="node"></param>
            /// <param name="childName"></param>
            /// <returns></returns>
            public static Modifier loadElemModifier(XmlNode node, string childName)
            {
                XmlNode childNode = node.SelectSingleNode(childName);
                try
                {
                    int constant = int.Parse(childNode?.Attributes["constant"]?.Value);
                    float scalar = float.Parse(childNode?.Attributes["scalar"]?.Value);

                    return new Modifier(constant, scalar);
                }
                catch (ArgumentNullException)
                {
                    Debug.Log("ERROR: failed to load " + childName + " modifier.");
                    return Modifier.neutral();
                }
            }
        }
        protected struct ElementalMods
        {
            Modifier resilience;
            Modifier mental;
            Modifier physical;
            Modifier arcane;
            Modifier fire;
            Modifier ice;
            Modifier earth;
            Modifier electric;
            Modifier wind;
            Modifier water;
            Modifier psychic;
            Modifier dark;
            Modifier light;

            public ElementalMods(Modifier resilience, Modifier mental, Modifier physical, Modifier arcane, Modifier fire, Modifier ice, Modifier earth, Modifier electric, Modifier wind,
                Modifier water, Modifier psychic, Modifier dark, Modifier light)
            {
                this.resilience = resilience;
                this.mental = mental;
                this.physical = physical;
                this.arcane = arcane;
                this.fire = fire;
                this.ice = ice;
                this.earth = earth;
                this.electric = electric;
                this.wind = wind;
                this.water = water;
                this.psychic = psychic;
                this.dark = dark;
                this.light = light;
            }

            public static ElementalMods neutral()
            {
                Modifier zero = Modifier.neutral();
                return new ElementalMods(zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero);
            }

            public static ElementalMods loadAllModifiers(XmlNode node)
            {
                if (node != null)
                {
                    Modifier resilience = Modifier.loadElemModifier(node, "resilience");
                    Modifier mental = Modifier.loadElemModifier(node, "mental");
                    Modifier physical = Modifier.loadElemModifier(node, "physical");
                    Modifier arcane = Modifier.loadElemModifier(node, "arcane");
                    Modifier fire = Modifier.loadElemModifier(node, "fire");
                    Modifier ice = Modifier.loadElemModifier(node, "ice");
                    Modifier earth = Modifier.loadElemModifier(node, "earth");
                    Modifier electric = Modifier.loadElemModifier(node, "electric");
                    Modifier wind = Modifier.loadElemModifier(node, "wind");
                    Modifier water = Modifier.loadElemModifier(node, "water");
                    Modifier psychic = Modifier.loadElemModifier(node, "psychic");
                    Modifier dark = Modifier.loadElemModifier(node, "dark");
                    Modifier light = Modifier.loadElemModifier(node, "light");

                    return new ElementalMods(resilience, mental, physical, arcane, fire, ice, earth, electric, wind, water, psychic, dark, light);
                }
                else
                {
                    Debug.Log("ERROR: failed to load elemental modifier list.");
                    return ElementalMods.neutral();
                }
            }
        }
        protected ElementalMods resistance;
        protected ElementalMods damage;

		public Unit ()
		{
			this.name = "Unknown";
			this.mobility = new List<KeyValuePair<string, float>>();
			
			this.core = new GameObject("unnamedUnit");
			//this.core.transform.position = new Vector3(0,0,layer);
			this.core.AddComponent<SpriteRenderer>();
			Texture2D spriteSheet = (Texture2D)Resources.Load(test_unit_filepath,typeof(Texture2D));
			Sprite sprite = Sprite.Create(spriteSheet,new Rect(0f,0f,GridRPG.Terrain.terrain_dim,GridRPG.Terrain.terrain_dim),new Vector2(0.5f,0.5f));
			this.core.GetComponent<SpriteRenderer>().sprite = sprite;
            this.core.GetComponent<SpriteRenderer>().sortingLayerName = UNIT_LAYER;

            this.space = null;
        }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GridRPGCode.Unit"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of Unit.
		/// </para>
		public Unit(string name)
		{
			this.name = name;
			this.mobility = new List<KeyValuePair<string, float>>();
			core = new GameObject(name);
		}
		
		public abstract GridRPG.Space warpToSpace(GridRPG.Space space);
        /// <summary>
        /// Gets unit's current HP.
        /// </summary>
        /// <returns>Current HP.</returns>
        public abstract uint getHP();
        /// <summary>
        /// Gets unit's maximum HP.
        /// </summary>
        /// <returns>Maximum HP.</returns>
        public abstract uint getMaxHP();

        /// <summary>
        /// Unit attempts to move to the specified coordinates. 
        /// </summary>
        /// <param name="destination">Destination coordinates</param>
        /// <remarks>If it fails, movement does not occur and returns null.</remarks>
        /// <returns></returns>
        //public abstract Space tryMove(Vector2 destination);
	}
	
	/// <summary>
	/// Unit that has it's data persist through maps.
	/// It has a custom name and an id number
	/// </summary>
	public class CampaignUnit : Unit
	{
		
		public int id;
		
		public CampaignUnit()
		{
			core.SetActive(true);
			this.name = "Unknown";
            this.core.name = "campaignUnit1";			
			this.id = 1;			
			this.space = null;			
			core.SetActive(false);
		}
		
		public CampaignUnit(string displayName, string name)
		{
			core.SetActive(true);
			this.name = displayName;			
			this.core.name = "campaignUnit1";
			this.id = 1;			
			this.space = null;			
			core.SetActive(false);
		}
		
		public CampaignUnit(string displayName, int id)
		{
			core.SetActive(true);
			this.name = "campaignUnit" + id;			
			this.core.name = this.name;
			this.id = id;			
			this.space = null;
			
			core.SetActive(false);
		}
		
        public CampaignUnit(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode unitNode = xmlDoc.DocumentElement.SelectSingleNode("/unit");
            if (unitNode != null)
            {
                this.name = unitNode.Attributes["displayName"]?.Value ?? "Unknown";
                this.core.name = unitNode.Attributes["name"]?.Value ?? "CampaignUnit";

                //SPRITE
                XmlNode spriteNode = unitNode.SelectSingleNode("sprite");
                if(spriteNode != null)
                {
                    SpriteRenderer spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                    Texture2D spriteSheet;
                    float xOffset;
                    float yOffset;
                    try
                    {
                        spriteSheet = Resources.Load<Texture2D>(spriteNode.Attributes["file"]?.Value);
                        xOffset = float.Parse(spriteNode.Attributes["XOffset"]?.Value);
                        yOffset = float.Parse(spriteNode.Attributes["YOffset"]?.Value);
                        spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                    catch(ArgumentNullException)
                    {
                        spriteSheet = Resources.Load<Texture2D>(test_unit_filepath);
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                }
                else
                {
                    //Default sprite
                }

                //STATS
                XmlNode statsNode = unitNode.SelectSingleNode("stats");
                if(statsNode != null)
                {
                    try
                    {
                        uint maxHP = uint.Parse(statsNode.SelectSingleNode("maxHP")?.InnerText);
                        uint HP = uint.Parse(statsNode.SelectSingleNode("HP")?.InnerText);
                        uint maxMP = uint.Parse(statsNode.SelectSingleNode("maxMP")?.InnerText);
                        uint MP = uint.Parse(statsNode.SelectSingleNode("MP")?.InnerText);
                        uint constitution = uint.Parse(statsNode.SelectSingleNode("constitution")?.InnerText);
                        uint strength = uint.Parse(statsNode.SelectSingleNode("strength")?.InnerText);
                        uint dexterity = uint.Parse(statsNode.SelectSingleNode("dexterity")?.InnerText);
                        uint intellect = uint.Parse(statsNode.SelectSingleNode("intellect")?.InnerText);
                        uint charisma = uint.Parse(statsNode.SelectSingleNode("charisma")?.InnerText);
                        uint agility = uint.Parse(statsNode.SelectSingleNode("agility")?.InnerText);

                        this.stats = new Stats_S(maxHP, HP, maxMP, MP, constitution, strength, dexterity, intellect, charisma, agility);
                    }
                    catch(ArgumentNullException)
                    {
                        this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    }
                }
                else
                {
                    this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                //RESISTANCES
                XmlNode resistNode = unitNode.SelectSingleNode("resistance");
                if (resistNode == null) Debug.Log("RESISTANCE NODE NOT FOUND");
                resistance = ElementalMods.loadAllModifiers(resistNode);

                //DAMAGE
                XmlNode damageNode = unitNode.SelectSingleNode("damage");
                damage = ElementalMods.loadAllModifiers(damageNode);

                //MOBILITY
                XmlNode mobilitiesNode = unitNode.SelectSingleNode("mobilities");
                XmlNodeList mobililyNodeList = mobilitiesNode.SelectNodes("mobility");
                for (int i = 0; i < mobililyNodeList.Count; i++)
                {
                    try
                    {
                        string type = mobililyNodeList[i].Attributes["type"]?.Value;
                        float modifier = float.Parse(mobililyNodeList[i].Attributes["modifier"]?.Value);
                        mobility.Add(new KeyValuePair<string, float>(type,modifier));
                    }
                    catch(ArgumentNullException)
                    {
                        Debug.Log("ERROR: Improper mobility XML.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("<unit> node not found in xml file.", "filename");
            }
        }       

		public CampaignUnit(string displayName, int id, string spriteFilepath, Rect spriteOffset)
		{
			core.SetActive(true);
			this.name = displayName;
            this.mobility = new List<KeyValuePair<string, float>>();
			
			this.core = new GameObject(this.name);
			//this.core.transform.position = new Vector3(0,0,layer);
			this.core.AddComponent<SpriteRenderer>();
			Texture2D spriteSheet = (Texture2D)Resources.Load(spriteFilepath,typeof(Texture2D));
			Sprite sprite = Sprite.Create(spriteSheet,spriteOffset,new Vector2(0.5f,0.5f));
			this.core.GetComponent<SpriteRenderer>().sprite = sprite;
			this.id = id;			
			this.space = null;			
			core.SetActive(false);
		}

        //copies the source unit
        public CampaignUnit(CampaignUnit source)
        {
            bool source_active = source.core.activeSelf;

            if (!source_active)
            {
                source.core.SetActive(true);
            }

            this.name = source.name;
            this.stats = source.stats;
            this.resistance = source.resistance;
            this.damage = source.damage;
            this.mobility = source.mobility;
            this.space = null;
            GameObject.Destroy(this.core);
            this.core = GameObject.Instantiate(source.core);
            this.core.name = source.core.name;
            this.id = source.id;

            if (!source_active)
            {
                source.core.SetActive(false);
            }
        }
		
		public override GridRPG.Space warpToSpace(GridRPG.Space space)
		{
			if(this.space != null)
			{
				this.space.removeUnit(this);
			}
            else
            {
                Debug.Log("Activating " + this.name);
            }

			if(space == null)
			{
                Debug.Log("Space doesnt exist. Deactivating " + this.name);
				core.SetActive(false);
                return null;
			}

            core.SetActive(true);
            this.space = space;
			this.space.addUnit(this);
			
			core.transform.SetParent(space.gameObject.transform);
			this.core.transform.localPosition = new Vector3(0,0,-2);
			Debug.Log(core.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
			return this.space;
		}

        public override uint getHP()
        {
            return stats.HP;
        }

        public override uint getMaxHP()
        {
            return stats.maxHP;
        }
    }

    public class NPCUnit : Unit
    {
        public NPCUnit(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode unitNode = xmlDoc.DocumentElement.SelectSingleNode("/unit");
            if (unitNode != null)
            {
                this.name = unitNode.Attributes["name"]?.Value ?? "Unknown";
                this.core.name = this.name;

                //SPRITE
                XmlNode spriteNode = unitNode.SelectSingleNode("sprite");
                if (spriteNode != null)
                {
                    SpriteRenderer spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                    Texture2D spriteSheet;
                    float xOffset;
                    float yOffset;
                    try
                    {
                        spriteSheet = Resources.Load<Texture2D>(spriteNode.Attributes["file"]?.Value);
                        xOffset = float.Parse(spriteNode.Attributes["XOffset"]?.Value);
                        yOffset = float.Parse(spriteNode.Attributes["YOffset"]?.Value);
                        spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                    catch (ArgumentNullException)
                    {
                        spriteSheet = Resources.Load<Texture2D>(test_unit_filepath);
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                }
                else
                {
                    //Default sprite
                }

                //STATS
                XmlNode statsNode = unitNode.SelectSingleNode("stats");
                if (statsNode != null)
                {
                    try
                    {
                        uint maxHP = uint.Parse(statsNode.SelectSingleNode("maxHP")?.InnerText);
                        uint HP = uint.Parse(statsNode.SelectSingleNode("HP")?.InnerText);
                        uint maxMP = uint.Parse(statsNode.SelectSingleNode("maxMP")?.InnerText);
                        uint MP = uint.Parse(statsNode.SelectSingleNode("MP")?.InnerText);
                        uint constitution = uint.Parse(statsNode.SelectSingleNode("constitution")?.InnerText);
                        uint strength = uint.Parse(statsNode.SelectSingleNode("strength")?.InnerText);
                        uint dexterity = uint.Parse(statsNode.SelectSingleNode("dexterity")?.InnerText);
                        uint intellect = uint.Parse(statsNode.SelectSingleNode("intellect")?.InnerText);
                        uint charisma = uint.Parse(statsNode.SelectSingleNode("charisma")?.InnerText);
                        uint agility = uint.Parse(statsNode.SelectSingleNode("agility")?.InnerText);

                        this.stats = new Stats_S(maxHP, HP, maxMP, MP, constitution, strength, dexterity, intellect, charisma, agility);
                    }
                    catch (ArgumentNullException)
                    {
                        this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    }
                }
                else
                {
                    this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                //RESISTANCES
                XmlNode resistNode = unitNode.SelectSingleNode("resistance");
                if (resistNode == null) Debug.Log("RESISTANCE NODE NOT FOUND");
                resistance = ElementalMods.loadAllModifiers(resistNode);

                //DAMAGE
                XmlNode damageNode = unitNode.SelectSingleNode("damage");
                damage = ElementalMods.loadAllModifiers(damageNode);

                //MOBILITY
                XmlNode mobilitiesNode = unitNode.SelectSingleNode("mobilities");
                XmlNodeList mobililyNodeList = mobilitiesNode.SelectNodes("mobility");
                for (int i = 0; i < mobililyNodeList.Count; i++)
                {
                    try
                    {
                        string type = mobililyNodeList[i].Attributes["type"]?.Value;
                        float modifier = float.Parse(mobililyNodeList[i].Attributes["modifier"]?.Value);
                        mobility.Add(new KeyValuePair<string, float>(type, modifier));
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.Log("ERROR: Improper mobility XML.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("<unit> node not found in xml file.", "filename");
            }
        }

        public NPCUnit(NPCUnit source)
        {
            bool source_active = source.core.activeSelf;

            if (!source_active)
            {
                source.core.SetActive(true);
            }

            this.name = source.name;
            this.stats = source.stats;
            this.resistance = source.resistance;
            this.damage = source.damage;
            this.mobility = source.mobility;
            this.space = null;
            GameObject.Destroy(this.core);
            this.core = GameObject.Instantiate(source.core);
            this.core.name = source.core.name;

            if (!source_active)
            {
                source.core.SetActive(false);
            }
        }

        public override GridRPG.Space warpToSpace(GridRPG.Space space)
        {
            if (this.space != null)
            {
                this.space.removeUnit(this);
            }
            else
            {
                Debug.Log("Activating " + this.name);
            }

            if (space == null)
            {
                Debug.Log("Space doesnt exist. Deactivating " + this.name);
                core.SetActive(false);
                return null;
            }

            core.SetActive(true);
            this.space = space;
            this.space.addUnit(this);

            core.transform.SetParent(space.gameObject.transform);
            this.core.transform.localPosition = new Vector3(0, 0, -2);
            Debug.Log(core.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
            return this.space;
        }

        public override uint getHP()
        {
            return stats.HP;
        }

        public override uint getMaxHP()
        {
            return stats.maxHP;
        }
    }
}*/

namespace GridRPG
{
    public struct Modifier
    {
        public float scalar;
        public int constant;

        public Modifier(int constant, float scalar)
        {
            this.constant = constant;
            this.scalar = scalar;
        }

        public static Modifier neutral()
        {
            return new Modifier(0, 1f);
        }

        public float applyTo(int initialValue)
        {
            return scalar * initialValue + constant;
        }

        /// <summary>
        /// Loads a modifier from a child node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Modifier loadElemModifier(XmlNode node, string childName)
        {
            XmlNode childNode = node.SelectSingleNode(childName);
            try
            {
                int constant = int.Parse(childNode?.Attributes["constant"]?.Value);
                float scalar = float.Parse(childNode?.Attributes["scalar"]?.Value);

                return new Modifier(constant, scalar);
            }
            catch (ArgumentNullException)
            {
                Debug.Log("ERROR: failed to load " + childName + " modifier.");
                return Modifier.neutral();
            }
        }
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(AnimationManager))]
    public class Unit : MonoBehaviour
    {
        protected const string test_unit_filepath = "Sprites/Unit/TestUnit";
        public const int MAX_NUMBER_ACTIVE_SKILLS = 4;
        public const int layer = 4; //need to remove this
        public const string UNIT_LAYER = "Unit";
        public const float UNIT_SPRITE_SIZE = 32f;
        public enum AnimationState { Idle, North, East, South, West }
        public enum Stat {  maxHP,  HP,  maxMP,  MP,  constitution,  strength,  dexterity,  intellect,  charisma,  agility}
        public delegate void DeathEvent(GameObject unit);

        public GridRPG.Game game;
        public Vector2 spaceCoords;
        public event DeathEvent deathEvent;


        public string faction { get; protected set; }
        public Type[] activeSkills { get; protected set; }
        public class StatsList
        {
            public uint maxHP = 0;
            public uint HP = 0;
            public uint maxMP = 0;
            public uint MP = 0;
            public uint constitution = 0;
            public uint strength = 0;
            public uint dexterity = 0;
            public uint intellect = 0;
            public uint charisma = 0;
            public uint agility = 0;

            public StatsList(uint maxHP, uint HP, uint maxMP, uint MP, uint constitution, uint strength, uint dexterity, uint intellect, uint charisma, uint agility)
            {
                this.maxHP = maxHP;
                this.HP = HP;
                this.maxMP = maxMP;
                this.MP = MP;
                this.constitution = constitution;
                this.strength = strength;
                this.dexterity = dexterity;
                this.intellect = intellect;
                this.charisma = charisma;
                this.agility = agility;
            }

            public StatsList(StatsList source)
            {
                this.maxHP = source.maxHP;
                this.HP = source.HP;
                this.maxMP = source.maxMP;
                this.MP = source.MP;
                this.constitution = source.constitution;
                this.strength = source.strength;
                this.dexterity = source.dexterity;
                this.intellect = source.intellect;
                this.charisma = source.charisma;
                this.agility = source.agility;
            }

            public uint setStat(uint value, string stat)
            {
                string statLowerCase = stat.ToLower();
                switch (statLowerCase)
                {
                    case "maxhp": return maxHP = value;
                    case "hp": return HP = value;
                    case "maxmp": return maxMP = value;
                    case "mp": return MP = value;
                    case "constitution": return constitution = value;
                    case "strength": return strength = value;
                    case "dexterity": return dexterity = value;
                    case "intellect": return intellect = value;
                    case "charisma": return charisma = value;
                    case "agility": return agility = value;

                }
                return 0;
            }

            public void copy(StatsList source)
            {
                this.maxHP = source.maxHP;
                this.HP = source.HP;
                this.maxMP = source.maxMP;
                this.MP = source.MP;
                this.constitution = source.constitution;
                this.strength = source.strength;
                this.dexterity = source.dexterity;
                this.intellect = source.intellect;
                this.charisma = source.charisma;
                this.agility = source.agility;
            }
        }
        public StatsList stats { get; protected set; }
        
        public struct ElementalMods
        {
            public Modifier resilience;
            public Modifier mental;
            public Modifier physical;
            public Modifier arcane;
            public Modifier fire;
            public Modifier ice;
            public Modifier earth;
            public Modifier electric;
            public Modifier wind;
            public Modifier water;
            public Modifier psychic;
            public Modifier dark;
            public Modifier light;

            public ElementalMods(Modifier resilience, Modifier mental, Modifier physical, Modifier arcane, Modifier fire, Modifier ice, Modifier earth, Modifier electric, Modifier wind,
                Modifier water, Modifier psychic, Modifier dark, Modifier light)
            {
                this.resilience = resilience;
                this.mental = mental;
                this.physical = physical;
                this.arcane = arcane;
                this.fire = fire;
                this.ice = ice;
                this.earth = earth;
                this.electric = electric;
                this.wind = wind;
                this.water = water;
                this.psychic = psychic;
                this.dark = dark;
                this.light = light;
            }

            public Modifier getElement(string element)
            {
                string elementLowerCase = element.ToLower();
                switch (elementLowerCase)
                {
                    case "resilience": return resilience;
                    case "mental": return mental;
                    case "physical": return physical;
                    case "arcane": return arcane;
                    case "fire": return fire;
                    case "ice": return ice;
                    case "earth": return earth;
                    case "electric": return electric;
                    case "wind": return wind;
                    case "water": return water;
                    case "psychic": return psychic;
                    case "dark": return dark;
                    case "light": return light;
                }
                return Modifier.neutral();
            }

            public Modifier setElement(Modifier mod, string element)
            {
                string elementLowerCase = element.ToLower();
                switch (elementLowerCase)
                {
                    case "resilience": return resilience = mod;
                    case "mental": return mental = mod;
                    case "physical": return physical = mod;
                    case "arcane": return arcane = mod;
                    case "fire": return fire = mod;
                    case "ice": return ice = mod;
                    case "earth": return earth = mod;
                    case "electric": return electric = mod;
                    case "wind": return wind = mod;
                    case "water": return water = mod;
                    case "psychic": return psychic = mod;
                    case "dark": return dark = mod;
                    case "light": return light = mod;
                }
                return Modifier.neutral();
            }

            public static ElementalMods neutral()
            {
                Modifier zero = Modifier.neutral();
                return new ElementalMods(zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero, zero);
            }

            public static ElementalMods loadAllModifiers(XmlNode node)
            {
                if (node != null)
                {
                    Modifier resilience = Modifier.loadElemModifier(node, "resilience");
                    Modifier mental = Modifier.loadElemModifier(node, "mental");
                    Modifier physical = Modifier.loadElemModifier(node, "physical");
                    Modifier arcane = Modifier.loadElemModifier(node, "arcane");
                    Modifier fire = Modifier.loadElemModifier(node, "fire");
                    Modifier ice = Modifier.loadElemModifier(node, "ice");
                    Modifier earth = Modifier.loadElemModifier(node, "earth");
                    Modifier electric = Modifier.loadElemModifier(node, "electric");
                    Modifier wind = Modifier.loadElemModifier(node, "wind");
                    Modifier water = Modifier.loadElemModifier(node, "water");
                    Modifier psychic = Modifier.loadElemModifier(node, "psychic");
                    Modifier dark = Modifier.loadElemModifier(node, "dark");
                    Modifier light = Modifier.loadElemModifier(node, "light");

                    return new ElementalMods(resilience, mental, physical, arcane, fire, ice, earth, electric, wind, water, psychic, dark, light);
                }
                else
                {
                    Debug.Log("ERROR: failed to load elemental modifier list.");
                    return ElementalMods.neutral();
                }
            }
        }
        protected ElementalMods resistance;
        protected ElementalMods damage;
        //protected List<KeyValuePair<String, int>> mobility;
        protected Dictionary<String, int> mobility;

        protected string currentRoute = "";
        private bool movementInProgress = false;
        private float lastMoveTime = 0;
        private bool deathFlag = false;

        public void loadFromFile(string fileName)
        {
            Debug.Log("Loading unit from file: " + fileName);
            XmlDocument xmlDoc = new XmlDocument();
            TextAsset file = Resources.Load<TextAsset>(fileName);
            if(file == null)
            {
                throw new ArgumentException(fileName + " file could not be loaded.");
            }
            xmlDoc.LoadXml(file.text);

            XmlNode unitNode = xmlDoc.DocumentElement.SelectSingleNode("/unit");
            if (unitNode != null)
            {
                gameObject.name = unitNode.Attributes["name"]?.Value ?? "Unknown";
                faction = unitNode.Attributes["faction"]?.Value ?? "unknown";

                //ANIMATIONS
                XmlNode animationListNode = unitNode.SelectSingleNode("animations");
                if (animationListNode != null)
                {
                    Debug.Log("Loading animations for " + name);
                    SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    AnimationManager animationManager = gameObject.GetComponent<AnimationManager>();
                    Texture2D spriteSheet;
                    int spriteWidth, spriteLength, sheetLength, sheetWidth;

                    spriteRenderer.sortingLayerName = UNIT_LAYER;
                    spriteSheet = Resources.Load<Texture2D>(animationListNode.Attributes["file"]?.Value);
                    spriteLength = int.Parse(animationListNode.Attributes["spriteLength"]?.Value);
                    spriteWidth = int.Parse(animationListNode.Attributes["spriteWidth"]?.Value);
                    sheetLength = int.Parse(animationListNode.Attributes["sheetLength"]?.Value);
                    sheetWidth = int.Parse(animationListNode.Attributes["sheetWidth"]?.Value);

                    XmlNodeList animationNodeList = animationListNode.SelectNodes("animation");
                    foreach (XmlNode animationNode in animationNodeList)
                    {
                        //TODO: add attribute to match AnimationState with animation

                        int cycleRate = 0;
                        cycleRate = int.Parse(animationNode.Attributes["cycleRate"]?.Value);
                        List<int> animation = new List<int>();
                        XmlNodeList spriteNodeList = animationNode.SelectNodes("sprite");
                        foreach (XmlNode spriteNode in spriteNodeList)
                        {
                            int value = int.Parse(spriteNode?.InnerText);
                            //Debug.Log("Adding animation frame: " + value);
                            animation.Add(value);
                        }
                        Debug.Log("Adding animation with first value: " + animation[0]);
                        animationManager.addAnimation(spriteSheet, new Vector2(spriteLength, spriteWidth), new Vector2(sheetLength, sheetWidth), animation, cycleRate);
                        Debug.Log("Added animation with first value!: " + animation[0]);
                    }
                    if (animationNodeList.Count > 0)
                    {
                        //Set to idle animation
                        animationManager.CurrentAnimationId = (int)AnimationState.Idle;
                    }
                }
                else
                {
                    //SPRITE
                    //Debug.Log("loadFromFile(string): Loading Sprite...");
                    XmlNode spriteNode = unitNode.SelectSingleNode("sprite");
                    if (spriteNode != null)
                    {
                        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                        Texture2D spriteSheet;
                        float xOffset;
                        float yOffset;
                        try
                        {
                            spriteSheet = Resources.Load<Texture2D>(spriteNode.Attributes["file"]?.Value);
                            xOffset = float.Parse(spriteNode.Attributes["XOffset"]?.Value);
                            yOffset = float.Parse(spriteNode.Attributes["YOffset"]?.Value);
                            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                            spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                            spriteRenderer.sortingLayerName = UNIT_LAYER;
                        }
                        catch (ArgumentNullException)
                        {
                            spriteSheet = Resources.Load<Texture2D>(test_unit_filepath);
                            spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                        }
                    }
                    else
                    {
                        //Default sprite
                    }
                }

                //STATS
                //Debug.Log("loadFromFile(string): Loading Stats...");
                XmlNode statsNode = unitNode.SelectSingleNode("stats");
                if (statsNode != null)
                {
                    try
                    {
                        uint maxHP = uint.Parse(statsNode.SelectSingleNode("maxHP")?.InnerText);
                        uint HP = uint.Parse(statsNode.SelectSingleNode("HP")?.InnerText);
                        uint maxMP = uint.Parse(statsNode.SelectSingleNode("maxMP")?.InnerText);
                        //Debug.Log("Unit.loadfromfile(str): Max HP = " + maxHP);
                        uint MP = uint.Parse(statsNode.SelectSingleNode("MP")?.InnerText);
                        uint constitution = uint.Parse(statsNode.SelectSingleNode("constitution")?.InnerText);
                        uint strength = uint.Parse(statsNode.SelectSingleNode("strength")?.InnerText);
                        uint dexterity = uint.Parse(statsNode.SelectSingleNode("dexterity")?.InnerText);
                        uint intellect = uint.Parse(statsNode.SelectSingleNode("intellect")?.InnerText);
                        uint charisma = uint.Parse(statsNode.SelectSingleNode("charisma")?.InnerText);
                        uint agility = uint.Parse(statsNode.SelectSingleNode("agility")?.InnerText);

                        this.stats = new StatsList(maxHP, HP, maxMP, MP, constitution, strength, dexterity, intellect, charisma, agility);
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.Log("statsNode missing data");
                        this.stats = new StatsList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    }
                }
                else
                {
                    this.stats = new StatsList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                //RESISTANCES
                //Debug.Log("loadFromFile(string): Loading Resists...");
                XmlNode resistNode = unitNode.SelectSingleNode("resistance");
                if (resistNode == null) Debug.Log("RESISTANCE NODE NOT FOUND");
                resistance = ElementalMods.loadAllModifiers(resistNode);

                //DAMAGE
                //Debug.Log("loadFromFile(string): Loading Damages...");
                XmlNode damageNode = unitNode.SelectSingleNode("damage");
                damage = ElementalMods.loadAllModifiers(damageNode);

                //MOBILITY
                //Debug.Log("loadFromFile(string): Loading Mobilities...");
                //this.mobility = new List<KeyValuePair<string, int>>();
                this.mobility = new Dictionary<string, int>();
                XmlNode mobilitiesNode = unitNode.SelectSingleNode("mobilities");
                XmlNodeList mobililyNodeList = mobilitiesNode.SelectNodes("mobility");
                for (int i = 0; i < mobililyNodeList.Count; i++)
                {
                    //Debug.Log("loadFromFile(string): Loading Mobility["+i+"]...");
                    try
                    {
                        string terrainType = mobililyNodeList[i].Attributes["type"]?.Value;
                        int modifier;
                        try
                        {
                            modifier = int.Parse(mobililyNodeList[i].Attributes["modifier"]?.Value);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("modifier parse fail");
                            throw e;
                        }
                        // Debug.Log("loadFromFile(string): Loading Mobility[" + i + "]: (" + (terrainType ?? "null") + ", " + modifier + ")...");
                        //mobility.Add(new KeyValuePair<string, int>(terrainType, modifier));
                        mobility.Add(terrainType, modifier);
                        //Debug.Log("loadFromFile(string): Loaded Mobility[" + i + "]: " + terrainType);
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.Log("ERROR: Improper mobility XML.");
                    }
                }

                //SKILLS
                activeSkills = new Type[MAX_NUMBER_ACTIVE_SKILLS];
                XmlNode skillListNode = unitNode.SelectSingleNode("skills");
                XmlNodeList skillNodeList = skillListNode.SelectNodes("skill");
                for (int i = 0; i < skillNodeList.Count && i < MAX_NUMBER_ACTIVE_SKILLS; i++)
                {
                    Type skillType = null;
                    try
                    {
                        skillType = Type.GetType(skillNodeList[i].Attributes["class"]?.Value);
                        Debug.Log("Unit.LoadFromFile(): skillType=" + skillType.ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.Log("skill parse fail");
                        throw e;
                    }
                    activeSkills[i] = skillType;
                }
            }
            else
            {
                throw new ArgumentException("<unit> node not found in xml file.", "filename");
            }
        }

        public void Update()
        {
            if(movementInProgress)
            {
                float currentTime = Time.fixedTime;
                if (currentTime > lastMoveTime + 1)
                {
                    //one second wait
                    moveOne();
                    lastMoveTime = currentTime;
                }
            }
            if(deathFlag)
            {
                //He is dead :(
                Game.ui.displayMessage(name + " has died.");
                Debug.Log("Unit.Update(): destroying " + this.name);
                GameObject.Destroy(this.gameObject);
            }
        }

        public uint getStat(string statName)
        {
            string statNameLowerCase = statName.ToLower();
            switch (statNameLowerCase)
            {
                case "maxhp": return stats.maxHP;
                case "hp": return stats.HP;
                case "maxmp": return stats.maxMP;
                case "mp": return stats.MP;
                case "strength": return stats.strength;
                case "constitution": return stats.constitution;
                case "dexterity": return stats.dexterity;
                case "intellect": return stats.intellect;
                case "charisma": return stats.charisma;
                case "agility": return stats.agility;
            }
            return 0;
        }

        public uint getStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.maxHP: return stats.maxHP;
                case Stat.HP: return stats.HP;
                case Stat.maxMP: return stats.maxMP;
                case Stat.MP: return stats.MP;
                case Stat.strength: return stats.strength;
                case Stat.constitution: return stats.constitution;
                case Stat.dexterity: return stats.dexterity;
                case Stat.intellect: return stats.intellect;
                case Stat.charisma: return stats.charisma;
                case Stat.agility: return stats.agility;
            }
            return 0;
        }

        public Modifier getResistance(string element)
        {
            return resistance.getElement(element);
        }

        public Modifier getDamageMod(string element)
        {
            return damage.getElement(element);
        }

        /// <summary>
        /// Unit takes damage.
        /// </summary>
        /// <param name="value">Damage to deal before the unit's resistances.</param>
        /// <param name="element">Type of damage.</param>
        /// <returns>Damage dealt after resistances.</returns>
        public int takeDamage(uint value, string element)
        {
            Modifier mod = getResistance(element);
            int damageAmount = (int)(value * mod.scalar) + mod.constant;
            //Debug.Log("Unit.TakeDamage(): Damage amount = " + damageAmount);
            if(stats.HP < damageAmount)
            {
                //hp cannot become less than 0.
                stats.setStat(0, "hp");
            }
            else
            {
                stats.setStat((uint)(stats.HP - damageAmount), "hp");
            }

            if(stats.HP == 0)
            {
                Debug.Log(name + "'s HP is zero after takeDamage(uint,string).");
                die();
            }
            return damageAmount;
        }

        public void die()
        {
            Space currentSpace = Game.map.GetComponent<Map>().getSpace(spaceCoords);
            currentSpace.removeUnit();
            Debug.Log(name + "'s deathEvent is being invoked.");
            deathEvent?.Invoke(this.gameObject);
            deathFlag = true;
        }

        /// <summary>
        /// Copies the stats from another Unit
        /// </summary>
        /// <param name="source"></param>
        public void copy(Unit source)
        {
            if(source.gameObject == null)
            {
                throw new ArgumentException("source.gameObject is null");
            }

            bool source_active = source.gameObject.activeSelf;

            if (!source_active)
            {
                source.gameObject.SetActive(true);
            }

            this.game = source.game;
            this.name = source.name;
            this.faction = source.faction;
            this.stats = new StatsList(source.stats);
            //Debug.Log("Unit.copy: Max HP = " + this.stats.maxHP);
            this.resistance = source.resistance;
            this.damage = source.damage;
            this.mobility = source.mobility;
            this.activeSkills = new Type[MAX_NUMBER_ACTIVE_SKILLS];
            source.activeSkills.CopyTo(this.activeSkills, 0);

            gameObject.GetComponent<AnimationManager>().copy(source.gameObject.GetComponent<AnimationManager>());

            gameObject.SetActive(false);

            if (!source_active)
            {
                source.gameObject.SetActive(false);
            }
        }

        public static GameObject copy(GameObject source)
        {
            if(source == null)
            {
                throw new ArgumentNullException("Unit.copy(GameObject)");
            }

            GameObject ret = GameObject.Instantiate(source);
            ret.name = source.name;
            ret.GetComponent<Unit>().copy(source.GetComponent<Unit>());
            ret.SetActive(false);
            return ret;
        }

        /// <summary>
        /// Unit moves to the destination coordinates.
        /// </summary>
        /// <param name="destCoords">Destination space.</param>
        /// <remarks>If the movement is not possible, nothing happens.</remarks>
        public bool moveToSpace(Vector2 destCoords)
        {
            if (!Game.animationInProgress && !movementInProgress)
            {
                {
                    //Move
                    string route = tryMove(destCoords);
                    Debug.Log(route);
                    if (route != "X")
                    {
                        //TODO: Animate the movement
                        Game.animationInProgress = true;
                        movementInProgress = true;
                        Game.map.GetComponent<Map>().getSpace(spaceCoords).CurrentHighlight = Space.Highlight.Black;
                        Game.map.GetComponent<Map>().getSpace(spaceCoords).lockUnitinSpace(false);
                        currentRoute = route;
                        lastMoveTime = Time.fixedTime;

                        //warpToSpace(destCoords); //TEMP

                        Game.ui.updateUnitFrame(this);
                        Game.ui.displayMessage(this.name + " is moving...");
                        return true;
                    }
                    else
                    {
                        Game.ui.displayMessage(this.name + " cannot move to that location.");
                        return false;
                    }
                }
            }
            return false;
        }

        private void moveOne()
        {
            if (currentRoute.Length > 0)
            {
                AnimationManager animationManager = GetComponent<AnimationManager>();
                string direction = currentRoute.Substring(0, 1);
                int dx = 0;
                int dy = 0;

                switch (direction)
                {
                    case "N":
                        Debug.Log("Should change animation to north...");
                        animationManager.CurrentAnimationId = (int)AnimationState.North;
                        dx = 0;
                        dy = 1;
                        break;
                    case "E":
                        animationManager.CurrentAnimationId = (int)AnimationState.East;
                        dx = 1;
                        dy = 0;
                        break;
                    case "S":
                        animationManager.CurrentAnimationId = (int)AnimationState.South;
                        dx = 0;
                        dy = -1;
                        break;
                    case "W":
                        animationManager.CurrentAnimationId = (int)AnimationState.West;
                        dx = -1;
                        dy = 0;
                        break;
                }

                warpToSpace(new Vector2(spaceCoords.x + (float)dx, spaceCoords.y + (float)dy),true);
                currentRoute = currentRoute.Substring(1);
                Debug.Log("Remaining route: " + currentRoute);
                if(currentRoute.Length == 0)
                {
                    movementInProgress = false;
                    animationManager.CurrentAnimationId = (int)AnimationState.Idle;
                    Game.map.GetComponent<Map>().getSpace(spaceCoords).CurrentHighlight = Space.Highlight.Blue;
                    Game.map.GetComponent<Map>().getSpace(spaceCoords).lockUnitinSpace(true);
                    Game.animationInProgress = false;
                    Debug.Log("Done moving unit");
                    Game.ui.displayMessage(name + " has finished moving.");
                }
            }
            else
            {
                movementInProgress = false;
                Game.animationInProgress = false;
                Debug.Log("Done moving unit");
                Game.ui.displayMessage(name + " has finished moving.");
            }
        }

        /// <summary>
        /// Teleports unit to space without movement.
        /// </summary>
        /// <param name="spaceCoords">Coordinates of the space to move to.</param>
        /// <returns>new Space.</returns>
        public GridRPG.Space warpToSpace(Vector2 spaceCoords, Map map)
        {
            GridRPG.Space oldSpace = map.getSpace(this.spaceCoords);
            GridRPG.Space newSpace = map.getSpace(spaceCoords);
            this.spaceCoords = spaceCoords;
            oldSpace.removeUnit();
            newSpace.addUnit(this.gameObject);
            this.transform.position = newSpace.transform.position;
            Debug.Log("Unit.wTS(V3): new transform position = " + this.transform.position);
            gameObject.SetActive(true);
            return map.getSpace(spaceCoords);

            Debug.Log("warpToSpace... OHNO");
            if (map == null)
            {
                //Debug.Log("warpToSpace: GAME.MAP IS NULL");
                return null;
            }
            GridRPG.Space space = map.getSpace(spaceCoords);
            if (gameObject.activeSelf)
            {
                //Debug.Log("warpToSpace: Removing unit from old space...");
                map.getSpace(this.spaceCoords).removeUnit();
            }
            else
            {
                Debug.Log("Activating " + this.name);
            }

            if (space == null)
            {
                Debug.Log("Space doesnt exist. Deactivating " + this.name);
                gameObject.SetActive(false);
                return null;
            }

            gameObject.SetActive(true);
            this.spaceCoords = spaceCoords;
            space.addUnit(gameObject);

            //Debug.Log("warpTS: Setting transforms: " + (gameObject?.name ?? "unit has no GO"));
            //Debug.Log("warpTS: Setting transforms: "+(space?.gameObject?.name ?? "new space has no GO"));
            if(gameObject.transform == null || space.gameObject.transform == null)
            {
                Debug.Log("warpTS: A TRANSFORM IS NULL");
            }
            //gameObject.transform.SetParent(space.gameObject.transform);
            //gameObject.transform.SetParent(space.gameObject.transform);
            //Debug.Log("warpTS: Set transform part A done");
            //gameObject.transform.localPosition = new Vector3(0, 0, -2);
            //Debug.Log("warpTS: Set transform part B done");
            //Debug.Log("warpTS: "+ gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
            return space;
        }

        /// <summary>
        /// Teleports unit to space without movement.
        /// </summary>
        /// <param name="spaceCoords">Coordinates of the space to move to.</param>
        /// <param name="tempSpace">True if unit goes into the space's temp slot.</param>
        /// <returns>new Space.</returns>
        public GridRPG.Space warpToSpace(Vector2 spaceCoords, bool tempSpace, Map map)
        {
            GridRPG.Space oldSpace = map.getSpace(this.spaceCoords);
            GridRPG.Space newSpace = map.getSpace(spaceCoords);
            this.spaceCoords = spaceCoords;
            
            if (tempSpace)
            {
                oldSpace.removeTempUnit();
                newSpace.addTempUnit(this.gameObject);
            }
            else
            {
                oldSpace.removeUnit();
                newSpace.addUnit(this.gameObject);
            }
            this.transform.position = newSpace.transform.position;
            Debug.Log("Unit.wTS(V3): new transform position = " + this.transform.position);
            gameObject.SetActive(true);
            return map.getSpace(spaceCoords);
        }

        public GridRPG.Space warpToSpace(Vector2 spaceCoords)
        {
            

            return warpToSpace(spaceCoords, Game.map);
        }

        public GridRPG.Space warpToSpace(Vector2 spaceCoords, bool tempSpace)
        {
            return warpToSpace(spaceCoords, tempSpace, Game.map.GetComponent<Map>());
        }

        /// <summary>
        /// Unit attempts to move to the specified coordinates. 
        /// </summary>
        /// <param name="destCoords">Destination coordinates</param>
        /// <remarks>If it fails, movement does not occur and returns null.</remarks>
        /// <returns></returns>
        public string tryMove(Vector2 destCoords)
        {
            //First test if there is a unit in the way
            if(Game.map?.GetComponent<Map>().getSpace(destCoords)?.unit != null)
            {
                //Can't move there
                Debug.Log("Another Unit is at the destination already!");
                return "X";
            }

            return tryMoveDynamic(spaceCoords, destCoords);
            //return tryMoveUgly(this, game.map, spaceCoords, destCoords, (int)stats.agility);
        }

        /// <summary>
        /// Creates a route from the source node to the dest node. Uses brute force and recursion.
        /// </summary>
        /// <param name="srcCoords"></param>
        /// <param name="destCoords"></param>
        /// <param name="unit"></param>
        /// <param name="map"></param>
        /// <param name="moveRemaining">The amount of remaining spaces the unit can move.</param>
        /// <returns>route with each move in characters ("NSEW"). returns "X" if no route.</returns>
        /// <remarks>Unit can move through units of the same faction, but not other units.</remarks>
        private static string tryMoveUgly(Unit unit, Map map, Vector2 srcCoords, Vector2 destCoords,  int moveRemaining)
        {
            //Check if we are at the destination.
            if (srcCoords == destCoords)
            {
                //We did it!
                return "";
            }
            if (moveRemaining == 0)
            {
                //Unit cannot move anymore, and is not at the destination.
                return "X";
            }

            int neededMove;
            Vector2 nextCoords;
            Space nextSpace;
            string nextTerrain;

            //Check north
            nextCoords = new Vector2((int)srcCoords.x, (int)srcCoords.y + 1);
            nextSpace = map?.getSpace(nextCoords);
            if (nextSpace != null)
            {
                nextTerrain = nextSpace.getTerrainType();

                //See if movement north is possible           
                if (!unit.mobility.TryGetValue(nextTerrain, out neededMove))
                {
                    //not in mobility list, make 0
                    neededMove = 0;
                }
                if (neededMove > 0 && neededMove <= moveRemaining)
                {
                    //north is a possible path
                    string recurse = tryMoveUgly(unit, map, nextCoords, destCoords, moveRemaining - neededMove);
                    if (recurse != "X")
                    {
                        //destination was reached
                        return "N" + recurse;
                    }
                }
            }

            //Check east
            nextCoords = new Vector2((int)srcCoords.x+1, (int)srcCoords.y);
            nextSpace = map?.getSpace(nextCoords);
            if (nextSpace != null)
            {
                nextTerrain = nextSpace.getTerrainType();

                //See if movement north is possible           
                if (!unit.mobility.TryGetValue(nextTerrain, out neededMove))
                {
                    //not in mobility list, make 0
                    neededMove = 0;
                }
                if (neededMove > 0 && neededMove <= moveRemaining)
                {
                    //north is a possible path
                    string recurse = tryMoveUgly(unit, map, nextCoords, destCoords, moveRemaining - neededMove);
                    if (recurse != "X")
                    {
                        //destination was reached
                        return "E" + recurse;
                    }
                }
            }

            //Check West
            nextCoords = new Vector2((int)srcCoords.x-1, (int)srcCoords.y);
            nextSpace = map?.getSpace(nextCoords);
            if (nextSpace != null)
            {
                nextTerrain = nextSpace.getTerrainType();

                //See if movement north is possible           
                if (!unit.mobility.TryGetValue(nextTerrain, out neededMove))
                {
                    //not in mobility list, make 0
                    neededMove = 0;
                }
                if (neededMove > 0 && neededMove <= moveRemaining)
                {
                    //north is a possible path
                    string recurse = tryMoveUgly(unit, map, nextCoords, destCoords, moveRemaining - neededMove);
                    if (recurse != "X")
                    {
                        //destination was reached
                        return "W" + recurse;
                    }
                }
            }

            //Check South
            nextCoords = new Vector2((int)srcCoords.x, (int)srcCoords.y - 1);
            nextSpace = map?.getSpace(nextCoords);
            if (nextSpace != null)
            {
                nextTerrain = nextSpace.getTerrainType();

                //See if movement north is possible           
                if (!unit.mobility.TryGetValue(nextTerrain, out neededMove))
                {
                    //not in mobility list, make 0
                    neededMove = 0;
                }
                if (neededMove > 0 && neededMove <= moveRemaining)
                {
                    //north is a possible path
                    string recurse = tryMoveUgly(unit, map, nextCoords, destCoords, moveRemaining - neededMove);
                    if (recurse != "X")
                    {
                        //destination was reached
                        return "S" + recurse;
                    }
                }
            }

            //Route not found
            return "X";
        }

        /// <summary>
        /// Tests neighbor nodes for destination.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Assumes movement is an integer.(so no mobility with a decimal modifier)</remarks>
        private string tryMoveOne(Vector2 srcCoords, Vector2 destCoords, int moveRemaining)
        {
            //Test current space for destination
            if(srcCoords == destCoords)
            {
                return "";
            }

            //Test neighboring spaces for destination
            Vector2 nextSpace;
            int neededMove;
            nextSpace = new Vector2(srcCoords.x, srcCoords.y + 1);  //NORTH
            if (nextSpace == destCoords)
            {
                //Test if movement is possible
                neededMove = testTerrain(nextSpace, moveRemaining);
                if(neededMove != 0)
                {
                    return "N";
                }
                return "X";
            }
            nextSpace = new Vector2(srcCoords.x, srcCoords.y - 1);  //SOUTH
            if (nextSpace == destCoords)
            {
                //Test if movement is possible
                neededMove = testTerrain(nextSpace, moveRemaining);
                if (neededMove != 0)
                {
                    return "S";
                }
                return "X";
            }
            nextSpace = new Vector2(srcCoords.x + 1, srcCoords.y);  //EAST
            if (nextSpace == destCoords)
            {
                //Test if movement is possible
                neededMove = testTerrain(nextSpace, moveRemaining);
                if (neededMove != 0)
                {
                    return "E";
                }
                return "X";
            }
            nextSpace = new Vector2(srcCoords.x + 1, srcCoords.y);  //WEST
            if (nextSpace == destCoords)
            {
                //Test if movement is possible
                neededMove = testTerrain(nextSpace, moveRemaining);
                if (neededMove != 0)
                {
                    return "W";
                }
                return "X";
            }
            //Not in a neigboring space
            return "X";
        }

        private string tryMoveDynamic(Vector2 srcCoords, Vector2 destCoords)
        {
            Debug.Log("tryMoveDynamic(" + srcCoords.ToString() + ", " + destCoords.ToString() + ") start!");
            //Test self first
            if(srcCoords == destCoords)
            {
                return "X";
            }

            //Setup the stored data
            int[,] weightedRoute = new int[Game.map.GetComponent<Map>().mapLength,Game.map.GetComponent<Map>().mapWidth]; //(C# compiler sets all to 0)

            //mark start as -1
            weightedRoute[(int)srcCoords.x, (int)srcCoords.y] = -1;

            return tryMoveDynamicR(weightedRoute, (int)srcCoords.x, (int)srcCoords.y, (int)destCoords.x, (int)destCoords.y, (int)stats.agility, 0);
        }

        private string tryMoveDynamicR(int[,] weights, int srcX, int srcY, int destX, int destY, int moveRemaining, int moveUsed)
        {
            //test if movement is even possible in the 1st place
            if(moveRemaining < 1)
            {
                //Don't bother trying
                return "X";
            }

            string ret = "X";
            int[] testPath = new int[4]; //NESW, int represents needed move for route

            //Add weights to neighboring nodes
            int move,nextX,nextY;
            //North
            nextX = srcX;
            nextY = srcY + 1;
            move = testTerrain(new Vector2(nextX, nextY), moveRemaining);
            if(move != 0 && ((move+moveUsed)<weights[nextX,nextY] || weights[nextX,nextY] == 0))
            {
                weights[nextX, nextY] = moveUsed + move;
                testPath[0] = move;
                if (nextX == destX && nextY == destY)
                {
                    ret = "N";
                    Debug.Log("U.tMDR(...): Found a route requiring move " + (moveUsed + move));
                }
            }
            //East
            nextX = srcX + 1;
            nextY = srcY;
            move = testTerrain(new Vector2(nextX, nextY), moveRemaining);
            if (move != 0 && ((move + moveUsed) < weights[nextX, nextY] || weights[nextX, nextY] == 0))
            {
                weights[nextX, nextY] = moveUsed + move;
                testPath[1] = move;
                if (nextX == destX && nextY == destY)
                {
                    ret = "E";
                    Debug.Log("U.tMDR(...): Found a route requiring move " + (moveUsed + move));
                }
            }
            //South
            nextX = srcX;
            nextY = srcY - 1;
            move = testTerrain(new Vector2(nextX, nextY), moveRemaining);
            if (move != 0 && ((move + moveUsed) < weights[nextX, nextY] || weights[nextX, nextY] == 0))
            {
                weights[nextX, nextY] = moveUsed + move;
                testPath[2] = move;
                if (nextX == destX && nextY == destY)
                {
                    ret = "S";
                    Debug.Log("U.tMDR(...): Found a route requiring move " + (moveUsed + move));
                }
            }
            //West
            nextX = srcX - 1;
            nextY = srcY;
            move = testTerrain(new Vector2(nextX, nextY), moveRemaining);
            if (move != 0 && ((move + moveUsed) < weights[nextX, nextY] || weights[nextX, nextY] == 0))
            {
                weights[nextX, nextY] = moveUsed + move;
                testPath[3] = move;
                if (nextX == destX && nextY == destY)
                {
                    ret = "W";
                    Debug.Log("U.tMDR(...): Found a route requiring move " + (moveUsed + move));
                }
            }
            
            //Return if route is found
            if(ret != "X")
            {
                return ret;
            }

            //Continue search further
            int i;
            //North
            i = 0;
            if (testPath[i] != 0)
            {
                //Test path
                string route = tryMoveDynamicR(weights, srcX, srcY + 1, destX, destY, moveRemaining - testPath[i], moveUsed + testPath[i]);
                if(route != "X")
                {
                    ret = "N" + route;
                }
            }
            //East
            i = 1;
            if (testPath[i] != 0)
            {
                //Test path
                string route = tryMoveDynamicR(weights, srcX + 1, srcY, destX, destY, moveRemaining - testPath[i], moveUsed + testPath[i]);
                if (route != "X")
                {
                    ret = "E" + route;
                }
            }
            //South
            i = 2;
            if (testPath[i] != 0)
            {
                //Test path
                string route = tryMoveDynamicR(weights, srcX, srcY - 1, destX, destY, moveRemaining - testPath[i], moveUsed + testPath[i]);
                if (route != "X")
                {
                    ret = "S" + route;
                }
            }
            //West
            i = 3;
            if (testPath[i] != 0)
            {
                //Test path
                string route = tryMoveDynamicR(weights, srcX - 1, srcY, destX, destY, moveRemaining - testPath[i], moveUsed + testPath[i]);
                if (route != "X")
                {
                    ret = "W" + route;
                }
            }
            return ret;
        }

        /// <summary>
        /// Tests if the unit has mobility to move through specified space. Also checks if a hostile unit is in the way
        /// </summary>
        /// <param name="destCoords"></param>
        /// <param name="moveRemaining"></param>
        /// <returns>Amount of move needed.</returns>
        private int testTerrain(Vector2 destCoords, int moveRemaining)
        {
            Space destSpace = Game.map?.GetComponent<Map>()?.getSpace(destCoords);
            if (destSpace != null)
            {
                //First test if there is a hostile unit
                if(!isFriendly(destSpace.unit))
                {
                    //Can't move through a hostile unit.
                    return 0;
                }

                string destTerrain = destSpace.getTerrainType();
                int neededMove = 0;

                //See if movement is possible           
                if (!this.mobility.TryGetValue(destTerrain, out neededMove))
                {
                    //not in mobility list, make 0
                    return 0;
                }
                if (neededMove <= moveRemaining)
                {
                    return neededMove;
                }
            }
            return 0;
        }

        public bool isFriendly(Unit a)
        {
            return isFriendly(this, a);
        }
        public bool isFriendly(GameObject a)
        {
            return isFriendly(gameObject, a);
        }
        public static bool isFriendly(Unit a, Unit b)
        {
            if(a == null || b == null)
            {
                return true;
            }
            return a.faction == b.faction;
        }
        public static bool isFriendly(GameObject a, GameObject b)
        {
            Unit uA = a?.GetComponent<Unit>();
            Unit uB = b?.GetComponent<Unit>();
            return Unit.isFriendly(uA, uB);
        }
    } 

    //Some legacy support... should be able to remove
    public class CampaignUnit : Unit { }
    public class NPCUnit : Unit { }

    /*/// <summary>
    /// Unit that has it's data persist through maps.
    /// It has a custom name and an id number
    /// </summary>
    public class CampaignUnit : Unit
    {
        public override GridRPG.Space warpToSpace(GridRPG.Space space)
        {
            if (this.space != null)
            {
                this.space.removeUnit(this);
            }
            else
            {
                Debug.Log("Activating " + this.name);
            }

            if (space == null)
            {
                Debug.Log("Space doesnt exist. Deactivating " + this.name);
                core.SetActive(false);
                return null;
            }

            core.SetActive(true);
            this.space = space;
            this.space.addUnit(this);

            core.transform.SetParent(space.gameObject.transform);
            this.core.transform.localPosition = new Vector3(0, 0, -2);
            Debug.Log(core.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
            return this.space;
        }

        public override uint getHP()
        {
            return stats.HP;
        }

        public override uint getMaxHP()
        {
            return stats.maxHP;
        }
    }

    public class NPCUnit : Unit
    {
        public NPCUnit(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode unitNode = xmlDoc.DocumentElement.SelectSingleNode("/unit");
            if (unitNode != null)
            {
                this.name = unitNode.Attributes["name"]?.Value ?? "Unknown";
                this.core.name = this.name;

                //SPRITE
                XmlNode spriteNode = unitNode.SelectSingleNode("sprite");
                if (spriteNode != null)
                {
                    SpriteRenderer spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                    Texture2D spriteSheet;
                    float xOffset;
                    float yOffset;
                    try
                    {
                        spriteSheet = Resources.Load<Texture2D>(spriteNode.Attributes["file"]?.Value);
                        xOffset = float.Parse(spriteNode.Attributes["XOffset"]?.Value);
                        yOffset = float.Parse(spriteNode.Attributes["YOffset"]?.Value);
                        spriteRenderer = this.core.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                    catch (ArgumentNullException)
                    {
                        spriteSheet = Resources.Load<Texture2D>(test_unit_filepath);
                        spriteRenderer.sprite = Sprite.Create(spriteSheet, new Rect(0f, 0f, UNIT_SPRITE_SIZE, UNIT_SPRITE_SIZE), new Vector2(0.5f, 0.5f));
                    }
                }
                else
                {
                    //Default sprite
                }

                //STATS
                XmlNode statsNode = unitNode.SelectSingleNode("stats");
                if (statsNode != null)
                {
                    try
                    {
                        uint maxHP = uint.Parse(statsNode.SelectSingleNode("maxHP")?.InnerText);
                        uint HP = uint.Parse(statsNode.SelectSingleNode("HP")?.InnerText);
                        uint maxMP = uint.Parse(statsNode.SelectSingleNode("maxMP")?.InnerText);
                        uint MP = uint.Parse(statsNode.SelectSingleNode("MP")?.InnerText);
                        uint constitution = uint.Parse(statsNode.SelectSingleNode("constitution")?.InnerText);
                        uint strength = uint.Parse(statsNode.SelectSingleNode("strength")?.InnerText);
                        uint dexterity = uint.Parse(statsNode.SelectSingleNode("dexterity")?.InnerText);
                        uint intellect = uint.Parse(statsNode.SelectSingleNode("intellect")?.InnerText);
                        uint charisma = uint.Parse(statsNode.SelectSingleNode("charisma")?.InnerText);
                        uint agility = uint.Parse(statsNode.SelectSingleNode("agility")?.InnerText);

                        this.stats = new Stats_S(maxHP, HP, maxMP, MP, constitution, strength, dexterity, intellect, charisma, agility);
                    }
                    catch (ArgumentNullException)
                    {
                        this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    }
                }
                else
                {
                    this.stats = new Stats_S(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }

                //RESISTANCES
                XmlNode resistNode = unitNode.SelectSingleNode("resistance");
                if (resistNode == null) Debug.Log("RESISTANCE NODE NOT FOUND");
                resistance = ElementalMods.loadAllModifiers(resistNode);

                //DAMAGE
                XmlNode damageNode = unitNode.SelectSingleNode("damage");
                damage = ElementalMods.loadAllModifiers(damageNode);

                //MOBILITY
                XmlNode mobilitiesNode = unitNode.SelectSingleNode("mobilities");
                XmlNodeList mobililyNodeList = mobilitiesNode.SelectNodes("mobility");
                for (int i = 0; i < mobililyNodeList.Count; i++)
                {
                    try
                    {
                        string type = mobililyNodeList[i].Attributes["type"]?.Value;
                        float modifier = float.Parse(mobililyNodeList[i].Attributes["modifier"]?.Value);
                        mobility.Add(new KeyValuePair<string, float>(type, modifier));
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.Log("ERROR: Improper mobility XML.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("<unit> node not found in xml file.", "filename");
            }
        }

        public NPCUnit(NPCUnit source)
        {
            bool source_active = source.core.activeSelf;

            if (!source_active)
            {
                source.core.SetActive(true);
            }

            this.name = source.name;
            this.stats = source.stats;
            this.resistance = source.resistance;
            this.damage = source.damage;
            this.mobility = source.mobility;
            this.space = null;
            GameObject.Destroy(this.core);
            this.core = GameObject.Instantiate(source.core);
            this.core.name = source.core.name;

            if (!source_active)
            {
                source.core.SetActive(false);
            }
        }

        public override GridRPG.Space warpToSpace(GridRPG.Space space)
        {
            if (this.space != null)
            {
                this.space.removeUnit(this);
            }
            else
            {
                Debug.Log("Activating " + this.name);
            }

            if (space == null)
            {
                Debug.Log("Space doesnt exist. Deactivating " + this.name);
                core.SetActive(false);
                return null;
            }

            core.SetActive(true);
            this.space = space;
            this.space.addUnit(this);

            core.transform.SetParent(space.gameObject.transform);
            this.core.transform.localPosition = new Vector3(0, 0, -2);
            Debug.Log(core.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
            return this.space;
        }

        public override uint getHP()
        {
            return stats.HP;
        }

        public override uint getMaxHP()
        {
            return stats.maxHP;
        }
    }*/
}

