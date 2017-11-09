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
        private const string blue_highlight_file = "Sprites/Highlighting/BlueBox";

        public const int highlight_width = 0;
        public const float terrain_dim = 32.0f;
        public const int highlight_Layer = 2;
        public const string HIGHLIGHT_LAYER = "SpaceHighlighting";
        public const string TERRAIN_LAYER = "Terrain";

        private GridRPG.Terrain terrain = null;
        public GameObject unit {get; private set;}
        public GameObject tempUnit { get; private set; } //A unit that is simply passing through the space.
        //public GameObject core;
        public GameObject highlight;
        public Vector2 coordinates { get; private set; } //Location of the space.

        public Sprite black_box;
        public Sprite yellow_box;
        public Sprite blue_box;

        public delegate void SelectEvent(GameObject space);
        public static event SelectEvent selectEvent;
        private bool selectFlag;

        /// <summary>
        /// Gets the terrain type as a string.
        /// </summary>
        /// <returns>The type.</returns>
        public string getTerrainType()
        {
            if(terrain == null)
            {
                return "void";
            }

            return terrain.Type;
        }

        public void Start()
        {
            highlight = new GameObject("highlight");
            black_box = Sprite.Create((Texture2D)Resources.Load(black_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            yellow_box = Sprite.Create((Texture2D)Resources.Load(yellow_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            blue_box = Sprite.Create((Texture2D)Resources.Load(blue_highlight_file, typeof(Texture2D)), new Rect(0f, 0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));

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

            selectFlag = false;
            
        }

        private void OnMouseEnter()
        {
            if (!selectFlag)
            {
                highlight.GetComponent<SpriteRenderer>().sprite = yellow_box;
            }
        }

        private void OnMouseExit()
        {
            if (!selectFlag)
            {
                highlight.GetComponent<SpriteRenderer>().sprite = black_box;
            }
        }

        private void OnMouseDown()
        {
            //Debug.Log("Selected unit: " + (unit?.name ?? "NONE"));
            selectEvent?.Invoke(gameObject);
            selectFlag = true;
            highlight.GetComponent<SpriteRenderer>().sprite = blue_box;
            selectEvent += deselect;
            Debug.Log("Space.OnMouseDown(): Unit Faction: " + unit?.GetComponent<Unit>()?.faction ?? "None");
        }

        private void OnDestroy()
        {
            if(selectFlag == true)
            {
                selectEvent -= deselect;
                //Debug.Log("Removed " + gameObject.name + " from deselect watch");
            }
        }

        private void deselect(GameObject newSpace)
        {
            if (selectFlag)
            {
                if(unit != null /*&& newSpace?.GetComponent<Space>()?.unit == null*/)
                {
                    //Debug.Log(unit.GetComponent<Unit>().tryMove(newSpace.GetComponent<Space>().coordinates));
                    if (Input.GetKey(KeyCode.Alpha1))
                    {
                        //temporary method of melee attack
                        Skill.activateSkill<MeleeAttack>(unit, newSpace.GetComponent<Space>().coordinates);
                    }
                    else
                    {
                        unit.GetComponent<Unit>().moveToSpace(newSpace.GetComponent<Space>().coordinates);
                    }
                    
                }

                highlight.GetComponent<SpriteRenderer>().sprite = black_box;
                selectFlag = false;
                selectEvent -= deselect;
                //Debug.Log("Removed " + gameObject.name + " from deselect watch");
            }           
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <returns>The new GameObject.</returns>
        private static GameObject newSpace(string name)
        {
            GameObject ret = new GameObject(name);
            ret.AddComponent<Space>();
            ret.GetComponent<Space>().coordinates = new Vector2(0, 0);
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <param name="terrain">Terrain of the space.</param>
        /// <returns>The new GameObject.</returns>
        private static GameObject newSpace(string name,Terrain terrain)
        {
            GameObject ret = newSpace(name);
            ret.GetComponent<Space>().setTerrain(terrain);
            ret.GetComponent<Space>().coordinates = new Vector2(0, 0);
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="name">Name of the GameObject.</param>
        /// <param name="terrain">Name of the terrain type.</param>
        /// <returns>The new GameObject.</returns>
        private static GameObject newSpace(string name, string terrain)
        {
            GameObject ret = newSpace(name);
            ret.GetComponent<Space>().setTerrain(terrain);
            ret.GetComponent<Space>().coordinates = new Vector2(0, 0);
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="x">X coordinate of the Space.</param>
        /// <param name="y">Y coordinate of the Space.</param>
        /// <param name="terrain">Name of the terrain type.</param>
        /// <returns>The new GameObject.</returns>
        public static GameObject newSpace(int x, int y, string terrain)
        {
            GameObject ret = newSpace("("+x+","+y+")");
            ret.GetComponent<Space>().setTerrain(terrain);
            ret.GetComponent<Space>().coordinates = new Vector2(x, y);
            ret.GetComponent<Space>().unit = null;
            return ret;
        }

        /// <summary>
        /// Generates a new GameObject with the Space component.
        /// </summary>
        /// <param name="x">X coordinate of the Space.</param>
        /// <param name="y">Y coordinate of the Space.</param>
        /// <param name="terrain">Terrain of the space.</param>
        /// <returns>The new GameObject.</returns>
        public static GameObject newSpace(int x, int y, Terrain terrain)
        {
            GameObject ret = newSpace("(" + x + "," + y + ")");
            ret.GetComponent<Space>().setTerrain(terrain);
            ret.GetComponent<Space>().coordinates = new Vector2(x, y);
            ret.GetComponent<Space>().unit = null;
            return ret;
        }

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
		public GameObject addUnit(GameObject unit)
		{
            if (this.unit == null && unit?.GetComponent<Unit>() != null)
            {
                Debug.Log("S.aU(GO): Added Unit \"" + unit.name + "\" to space " + coordinates.ToString());
                this.unit = unit;
                return unit;
            }
            else
            {
                Debug.Log("S.aU(GO): Unit add failure "+ coordinates.ToString());
                return this.unit;
            }
		}

        public GameObject addTempUnit(GameObject unit)
        {
            if (this.tempUnit == null && unit?.GetComponent<Unit>() != null)
            {
                Debug.Log("S.aU(GO): Added Temporary Unit \"" + unit.name + "\" to space " + coordinates.ToString());
                this.tempUnit = unit;
                return unit;
            }
            else
            {
                Debug.Log("S.aU(GO): Unit add failure " + coordinates.ToString());
                return this.tempUnit;
            }
        }

        public override string ToString()
        {
            return coordinates.ToString();
        }

        public string getUnitName()
        {
            //Debug.Log("TEST");
            if(unit != null)
            {
                return unit.name;
            }
            else
            {
                return "NONE";
            }
        }

        public GameObject removeUnit()
        {
            GameObject ret = this.unit;
            this.unit = null;
            return ret;
        }

        public GameObject removeTempUnit()
        {
            GameObject ret = this.tempUnit;
            this.tempUnit = null;
            return ret;
        }

        /// <summary>
        /// Toggles a unit between the temp position and the standing position.
        /// </summary>
        /// <param name="key">True to lock, false to unlock.</param>
        public void lockUnitinSpace(bool key)
        {
            if(key && unit == null && tempUnit != null)
            {
                unit = tempUnit;
                tempUnit = null;
            }
            else if(!key && unit != null && tempUnit == null)
            {
                tempUnit = unit;
                unit = null;
            }
        }

        public static bool isAdjacent(Vector2 space1,Vector2 space2)
        {
            return (Math.Abs(space1.x - space2.x) == 1 && space1.y == space2.y) || (Math.Abs(space1.y - space2.y) == 1 && space1.x == space2.x);
        }

        /*/// <summary>
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
		}*/
		
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

    /*/// <summary>
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
    }*/
}

