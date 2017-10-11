using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Space : MonoBehaviour
	{
		private const string black_highlight_file = "Sprites/Highlighting/BlackBox";
        private const string yellow_highlight_file = "Sprites/Highlighting/YellowBox";

        public const int highlight_width = 0;
		public const float terrain_dim = 32.0f;
        public const int highlight_Layer = 2;
        public const string HIGHLIGHT_LAYER = "SpaceHighlighting";
        public const string TERRAIN_LAYER = "Terrain";

        private GridRPG.Terrain terrain = null;
		private Unit unit = null;
		//public GameObject core;
		public GameObject highlight;        

        public Sprite black_box;
        public Sprite yellow_box;

        public delegate void SelectEvent(Unit unit);
        public static event SelectEvent selectEvent;

        public void Start()
        {
            highlight = new GameObject("highlight");
            black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            yellow_box = Sprite.Create((Texture2D)Resources.Load(yellow_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));

            //highlight setup
            highlight.AddComponent<SpriteRenderer>();
            highlight.GetComponent<SpriteRenderer>().sprite = black_box;
            highlight.transform.SetParent(gameObject.transform);
            highlight.transform.localPosition = new Vector3(0, 0, highlight_Layer);
            highlight.GetComponent<SpriteRenderer>().sortingLayerName = HIGHLIGHT_LAYER;

            //terrain setup
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = TERRAIN_LAYER;
            if (terrain == null)
            {
                setTerrain("void");
                gameObject.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;                
            }

            Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
            gameObject.GetComponent<BoxCollider2D>().size = S;
        }

        private void OnMouseEnter()
        {
            highlight.GetComponent<SpriteRenderer>().sprite = yellow_box;
        }

        private void OnMouseExit()
        {
            highlight.GetComponent<SpriteRenderer>().sprite = black_box;
        }

        private void OnMouseDown()
        {
            Debug.Log("Selected unit: " + (unit?.name ?? "NONE"));
            if (selectEvent != null)
            {                
                selectEvent(unit);
            }
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <returns>The new GameObject.</returns>
        public static GameObject newSpace(string name)
        {
            GameObject ret = new GameObject(name);
            ret.AddComponent<Space>();
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <param name="terrain">Terrain of the space.</param>
        /// <returns>The new GameObject.</returns>
        public static GameObject newSpace(string name,Terrain terrain)
        {
            GameObject ret = newSpace(name);
            ret.GetComponent<Space>().setTerrain(terrain);
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <param name="terrain">Name of the terrain type.</param>
        /// <returns>The new GameObject.</returns>
        public static GameObject newSpace(string name, string terrain)
        {
            GameObject ret = newSpace(name);
            ret.GetComponent<Space>().setTerrain(terrain);
            return ret;
        }

        /*public Space()
		{
            GameObject core = this.gameObject;
			core = new GameObject("space");
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
            unit = null;
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
            //highlight.transform.localPosition = new Vector3(0, 0, highlight_Layer);
            highlight.GetComponent<SpriteRenderer>().sortingLayerName = HIGHLIGHT_LAYER;

            highlight.AddComponent<MouseSelection>();

            setTerrain("void");
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
            core.GetComponent<SpriteRenderer>().sortingLayerName = TERRAIN_LAYER;
            unit = null;
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
            unit = null;
		}*/

        public void setTerrain(GridRPG.Terrain terrain)
		{
			this.terrain = terrain;
            gameObject.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
		}
		
		public void setTerrain(string type)
		{
			this.terrain = new Terrain(type);
            gameObject.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
        }
		
		public void setTerrain(string type, Sprite sprite)
		{
			this.terrain = new Terrain(type,sprite);
            gameObject.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
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
            if (this.unit == null)
            {
                this.unit = unit;
                return unit;
            }
            else
            {
                return this.unit;
            }
		}

        public Unit removeUnit()
        {
            Unit ret = this.unit;
            this.unit = null;
            return ret;
        }

        /// <summary>
        /// DEPRICATED
        /// </summary>
        /// <param name="unitName"></param>
        /// <returns></returns>
		public Unit removeUnit(string unitName)
		{
            return removeUnit();
		}
		
        /// <summary>
        /// DEPRICATED
        /// </summary>
		public Unit removeUnit(Unit unit)
		{
            return removeUnit();
		}
		
		public Terrain getTerrain()
		{
			return this.terrain;	
		}
		
        /// <summary>
        /// DEPRICATED
        /// </summary>
		public List<Unit> UnitList
		{
			get{
                return null;
			}
		}
	}

    /// <summary>
    /// DEPRICATED: Handled in GridRPG.Space
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class MouseSelection : MonoBehaviour
    {
        private const string black_highlight_file = "Sprites/Highlighting/BlackBox";
        private const string yellow_highlight_file = "Sprites/Highlighting/YellowBox";

        public BoxCollider2D collide;
        public Sprite black_box;
        public Sprite yellow_box;

        public delegate void SelectEvent(Unit unit);
        public static event SelectEvent selectEvent;

        private void Start()
        {
            black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            yellow_box = Sprite.Create((Texture2D)Resources.Load(yellow_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));           

            GetComponent<SpriteRenderer>().sprite = black_box;

            Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
            gameObject.GetComponent<BoxCollider2D>().size = S;
        }

        private void OnMouseEnter()
        {
            GetComponent<SpriteRenderer>().sprite = yellow_box;
        }

        private void OnMouseExit()
        {
            GetComponent<SpriteRenderer>().sprite = black_box;
        }

        private void OnMouseDown()
        {
            if(selectEvent != null)
            {
                selectEvent(null);
                //TO FIX
            }
        }
    }
}

