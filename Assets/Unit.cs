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
		
		public string name;
		public List<KeyValuePair<String,int>> mobility;
		public Space space;
		public GameObject core;

		public Unit ()
		{
			this.name = "unnamedUnit";
			this.mobility = new List<KeyValuePair<string, int>>();
			
			this.core = new GameObject(this.name);
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
			this.mobility = new List<KeyValuePair<string, int>>();
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
		public string displayName;
		public int id;
		
		public CampaignUnit()
		{
			core.SetActive(true);
			this.name = "campaignUnit1";
			
			
			this.core.name = this.name;
	

			
			
			this.displayName = "unnamed";
			this.id = 1;
			
			this.space = null;
			
			core.SetActive(false);
		}
		
		public CampaignUnit(string displayName)
		{
			core.SetActive(true);
			this.name = "campaignUnit1";
			
			
			this.core.name = this.name;

		
		
			
			this.displayName = displayName;
			this.id = 1;
			
			this.space = null;
			
			core.SetActive(false);
		}
		
		public CampaignUnit(string displayName, int id)
		{
			core.SetActive(true);
			this.name = "campaignUnit" + id;
		
			
			this.core.name = this.name;
	
	
		
			
			this.displayName = displayName;
			this.id = id;
			
			this.space = null;
			
			core.SetActive(false);
		}
		
		public CampaignUnit(string displayName, int id, string spriteFilepath, Rect spriteOffset)
		{
			core.SetActive(true);
			this.name = "campaignUnit" + id;
			this.mobility = new List<KeyValuePair<string, int>>();
			
			this.core = new GameObject(this.name);
			//this.core.transform.position = new Vector3(0,0,layer);
			this.core.AddComponent<SpriteRenderer>();
			Texture2D spriteSheet = (Texture2D)Resources.Load(spriteFilepath,typeof(Texture2D));
			Sprite sprite = Sprite.Create(spriteSheet,spriteOffset,new Vector2(0.5f,0.5f));
			this.core.GetComponent<SpriteRenderer>().sprite = sprite;
			
			this.displayName = displayName;
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
            this.displayName = source.displayName;
            this.id = source.id;

            if (!source_active)
            {
                source.core.SetActive(false);
            }
        }
		
		public CampaignUnit(XmlDocument xmlDoc)
		{
			//TODO	
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

