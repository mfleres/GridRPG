using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;

namespace GridRPG
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
            uint maxHP;
            uint HP;
            uint maxMP;
            uint MP;
            uint constitution;
            uint strength;
            uint dexterity;
            uint intellect;
            uint charisma;
            uint agility;

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
			
			core.transform.SetParent(space.core.transform);
			this.core.transform.localPosition = new Vector3(0,0,-2);
			Debug.Log(core.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);
			return this.space;
		}
	}
}

