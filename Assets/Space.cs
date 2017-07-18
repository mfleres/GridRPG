using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
	public class Space
	{
		private const string black_highlight_file = "Sprites/Highlighting/BlackBox";
		
		public const int highlight_width = 2;
		public const float terrain_dim = 32.0f;
        public const int highlight_Layer = 2;
        public const string HIGHLIGHT_LAYER = "SpaceHighlighting";
        public const string TERRAIN_LAYER = "Terrain";

        private GridRPG.Terrain terrain = null;
		private List<Unit> unitList = null;
		public GameObject core;
		public GameObject highlight;
		
		public Space()
		{			
			core = new GameObject("space");
			core.AddComponent<SpriteRenderer>();
			highlight = new GameObject("highlight");

            //highlight setup
            Sprite black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, terrain_dim, terrain_dim), new Vector2(0.5f, 0.5f));
            highlight.AddComponent<SpriteRenderer>();
			highlight.GetComponent<SpriteRenderer>().sprite=black_box;
			highlight.transform.SetParent(core.transform);
            highlight.transform.localPosition = new Vector3(0, 0, highlight_Layer);
            highlight.GetComponent<SpriteRenderer>().sortingLayerName = HIGHLIGHT_LAYER;

            //mouseover setup
            highlight.AddComponent<MouseSelection>();
			
			setTerrain("void");
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
            core.GetComponent<SpriteRenderer>().sortingLayerName = TERRAIN_LAYER;
            unitList = new List<Unit>();
		}
		public Space(string name)
		{	
			core = new GameObject(name);
			core.AddComponent<SpriteRenderer>();
			
			highlight = new GameObject("highlight");

            Sprite black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, terrain_dim, terrain_dim), new Vector2(0.5f, 0.5f));
            highlight.AddComponent<SpriteRenderer>();
            highlight.GetComponent<SpriteRenderer>().sprite = black_box;
            highlight.transform.SetParent(core.transform);
            highlight.transform.localPosition = new Vector3(0, 0, highlight_Layer);
            highlight.GetComponent<SpriteRenderer>().sortingLayerName = HIGHLIGHT_LAYER;

            highlight.AddComponent<MouseSelection>();

            setTerrain("void");
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
            core.GetComponent<SpriteRenderer>().sortingLayerName = TERRAIN_LAYER;
            unitList = new List<Unit>();
		}
		
		public Space(string name,GridRPG.Terrain terrain)
		{			
			core = new GameObject(name);
			core.AddComponent<SpriteRenderer>();

            highlight = new GameObject("highlight");

            Sprite black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, terrain_dim, terrain_dim), new Vector2(0.5f, 0.5f));
            highlight.AddComponent<SpriteRenderer>();
            highlight.GetComponent<SpriteRenderer>().sprite = black_box;
            highlight.transform.SetParent(core.transform);
            highlight.transform.localPosition = new Vector3(0, 0, highlight_Layer);
            highlight.GetComponent<SpriteRenderer>().sortingLayerName = HIGHLIGHT_LAYER;

            highlight.AddComponent<MouseSelection>();

            setTerrain(terrain);
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
            core.GetComponent<SpriteRenderer>().sortingLayerName = TERRAIN_LAYER;
            unitList = new List<Unit>();
		}
		
		public void setTerrain(GridRPG.Terrain terrain)
		{
			this.terrain = terrain;
		}
		
		public void setTerrain(string type)
		{
			this.terrain = new Terrain(type);
		}
		
		public void setTerrain(string type, Sprite sprite)
		{
			this.terrain = new Terrain(type,sprite);
		}
		
		/// <summary>
		/// Adds the unit.
		/// </summary>
		/// <returns>
		/// The unit.
		/// </returns>
		/// <param name='unitName'>
		/// 	Unit's name.
		/// </param>
		public Unit addUnit(Unit unit)
		{
			unitList.Add(unit);
			return unit;
		}
		public Unit removeUnit(string unitName)
		{
			int search = unitList.FindIndex(x => x.name == unitName);
			
			if (search != -1)
			{
				Unit ret = unitList[search];
				unitList.RemoveAt(search);
				return ret;
			}
			else
			{
				return null;
			}
		}
		
		public Unit removeUnit(Unit unit)
		{
			int search = unitList.FindIndex(x => x == unit);
			
			if (search != -1)
			{
				Unit ret = unitList[search];
				unitList.RemoveAt(search);
				return ret;
			}
			else
			{
				return null;
			}
		}
		
		public Terrain getTerrain()
		{
			return this.terrain;	
		}
		
		public List<Unit> UnitList
		{
			get{
				return this.unitList;
			}
		}
	}

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class MouseSelection : MonoBehaviour
    {
        private const string black_highlight_file = "Sprites/Highlighting/BlackBox";
        private const string yellow_highlight_file = "Sprites/Highlighting/YellowBox";

        public BoxCollider2D collide;
        public Sprite black_box;
        public Sprite yellow_box;

        private void Start()
        {
            black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            yellow_box = Sprite.Create((Texture2D)Resources.Load(yellow_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));

            collide = GetComponent<BoxCollider2D>();

            GetComponent<SpriteRenderer>().sprite = black_box;
        }

        private void OnMouseEnter()
        {
            GetComponent<SpriteRenderer>().sprite = yellow_box;
        }

        private void OnMouseExit()
        {
            GetComponent<SpriteRenderer>().sprite = black_box;
        }
    }
}

