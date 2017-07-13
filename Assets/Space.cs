using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
	//[RequireComponent(typeof(SpriteRenderer))]
	public class Space //: MonoBehaviour
	{
		private const string black_highlight_file = "Sprites/Highlighting/Black";
		
		public const int highlight_width = 2;
		public const float terrain_dim = 32.0f;
		
		private GridRPG.Terrain terrain = null;
		private List<Unit> unitList = null;
		public GameObject core;
		public List<GameObject> highlights;
		
		public Space()
		{
			Sprite black_vertical = Sprite.Create((Texture2D)Resources.Load(black_highlight_file,typeof(Texture2D)),new Rect(0f,0f,highlight_width,Terrain.terrain_dim+highlight_width*2),new Vector2(0.5f,0.5f));
			Sprite black_horizontal = Sprite.Create((Texture2D)Resources.Load(black_highlight_file,typeof(Texture2D)),new Rect(0f,0f,Terrain.terrain_dim+highlight_width*2,highlight_width),new Vector2(0.5f,0.5f));
			
			core = new GameObject("space");
			core.AddComponent<SpriteRenderer>();
			highlights = new List<GameObject>();

			highlights.Add(new GameObject("left highlight"));
			highlights[0].AddComponent<SpriteRenderer>();
			highlights[0].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[0].transform.SetParent(core.transform);
			highlights[0].transform.localPosition = new Vector3(-(Terrain.terrain_dim/2f+1)/100f,0,0);
			
			
			highlights.Add(new GameObject("right highlight"));
			highlights[1].AddComponent<SpriteRenderer>();
			highlights[1].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[1].transform.SetParent(core.transform);
			highlights[1].transform.localPosition = new Vector3((Terrain.terrain_dim/2f+1)/100f,0,0);
			
			highlights.Add(new GameObject("bottom highlight"));
			highlights[2].AddComponent<SpriteRenderer>();
			highlights[2].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[2].transform.SetParent(core.transform);
			highlights[2].transform.localPosition = new Vector3(0,-(Terrain.terrain_dim/2f+1)/100f,0);
			
			highlights.Add(new GameObject("top highlight"));
			highlights[3].AddComponent<SpriteRenderer>();
			highlights[3].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[3].transform.SetParent(core.transform);
			highlights[3].transform.localPosition = new Vector3(0,(Terrain.terrain_dim/2f+1)/100f,0);
			
			setTerrain("void");
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
			unitList = new List<Unit>();
		}
		public Space(string name)
		{
			Texture2D black = (Texture2D)Resources.Load(black_highlight_file,typeof(Texture2D));
			Sprite black_vertical = Sprite.Create(black,new Rect(0f,0f,highlight_width,Terrain.terrain_dim+highlight_width*2),new Vector2(0.5f,0.5f));
			Sprite black_horizontal = Sprite.Create(black,new Rect(0f,0f,Terrain.terrain_dim+highlight_width*2,highlight_width),new Vector2(0.5f,0.5f));
			
			core = new GameObject(name);
			core.AddComponent<SpriteRenderer>();
			
			highlights = new List<GameObject>();

			highlights.Add(new GameObject("left highlight"));
			highlights[0].AddComponent<SpriteRenderer>();
			highlights[0].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[0].transform.SetParent(core.transform);
			highlights[0].transform.localPosition = new Vector3(-(Terrain.terrain_dim/2f+1)/100f,0,0);
			
			highlights.Add(new GameObject("right highlight"));
			highlights[1].AddComponent<SpriteRenderer>();
			highlights[1].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[1].transform.SetParent(core.transform);
			highlights[1].transform.localPosition = new Vector3((Terrain.terrain_dim/2f+1)/100f,0,0);
			
			highlights.Add(new GameObject("bottom highlight"));
			highlights[2].AddComponent<SpriteRenderer>();
			highlights[2].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[2].transform.SetParent(core.transform);
			highlights[2].transform.localPosition = new Vector3(0,-(Terrain.terrain_dim/2f+1)/100f,0);
			
			highlights.Add(new GameObject("top highlight"));
			highlights[3].AddComponent<SpriteRenderer>();
			highlights[3].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[3].transform.SetParent(core.transform);
			highlights[3].transform.localPosition = new Vector3(0,(Terrain.terrain_dim/2f+1)/100f,0);
			
			setTerrain("void");
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
			unitList = new List<Unit>();
		}
		
		public Space(string name,GridRPG.Terrain terrain)
		{
			Texture2D black = (Texture2D)Resources.Load(black_highlight_file,typeof(Texture2D));
			Sprite black_vertical = Sprite.Create(black,new Rect(0f,0f,highlight_width,Terrain.terrain_dim+highlight_width*2),new Vector2(0.5f,0.5f));
			Sprite black_horizontal = Sprite.Create(black,new Rect(0f,0f,Terrain.terrain_dim+highlight_width*2,highlight_width),new Vector2(0.5f,0.5f));
			
			core = new GameObject(name);
			core.AddComponent<SpriteRenderer>();
			
			highlights = new List<GameObject>();

			highlights.Add(new GameObject("left highlight"));
			highlights[0].AddComponent<SpriteRenderer>();
			highlights[0].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[0].transform.SetParent(core.transform);
			highlights[0].transform.localPosition = new Vector3(-(Terrain.terrain_dim/2f+1)/100f,0,0);
			
			highlights.Add(new GameObject("right highlight"));
			highlights[1].AddComponent<SpriteRenderer>();
			highlights[1].GetComponent<SpriteRenderer>().sprite=black_vertical;
			highlights[1].transform.SetParent(core.transform);
			highlights[1].transform.localPosition = new Vector3((Terrain.terrain_dim/2f+1)/100f,0,0);
			
			highlights.Add(new GameObject("bottom highlight"));
			highlights[2].AddComponent<SpriteRenderer>();
			highlights[2].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[2].transform.SetParent(core.transform);
			highlights[2].transform.localPosition = new Vector3(0,-(Terrain.terrain_dim/2f+1)/100f,0);
			
			highlights.Add(new GameObject("top highlight"));
			highlights[3].AddComponent<SpriteRenderer>();
			highlights[3].GetComponent<SpriteRenderer>().sprite=black_horizontal;
			highlights[3].transform.SetParent(core.transform);
			highlights[3].transform.localPosition = new Vector3(0,(Terrain.terrain_dim/2f+1)/100f,0);
			
			setTerrain(terrain);
			core.GetComponent<SpriteRenderer>().sprite = this.terrain.Sprite;
			unitList = new List<Unit>();
		}
		/**
		void Start ()
		{
			SpriteRenderer renderer = (SpriteRenderer)gameObject.GetComponent<SpriteRenderer>();
			
			if (renderer != null){
				
				//Debug.Log("setting renderer sprite");
				//Debug.Log(this.gameObject.name);
				renderer.sprite = _terrain.Sprite as Sprite;
			}
			else{
				Debug.Log("Space.Awake(): SpriteRenderer Component not found");
			}
		}
		
		void Awake()
		{
			if (_terrain == null)
			{
				_terrain = new GridRPG.Terrain("void");
			}
			if (_unitList == null)
			{
				_unitList = new List<Unit>();
			}
		}
		
		void Update()
		{
			/*SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			if (renderer != null){
				
				//Debug.Log("setting renderer sprite");
				//Debug.Log(this.gameObject.name);
				renderer.sprite = _terrain.Sprite as Sprite;
			}
			else{
				Debug.Log("Space.Update(): SpriteRenderer Component not found");
			}
		}
		
		//Generates a GameObject with only GridRPG.Space added
		public static GameObject generateGameObject()
		{
			GameObject ret = new GameObject();
			ret.AddComponent<GridRPG.Space>();
			ret.GetComponent<GridRPG.Space>().setTerrain("void");
			return ret;
		}
		public static GameObject generateGameObject(string name)
		{
			GameObject ret = new GameObject(name);
			ret.AddComponent<GridRPG.Space>();
			ret.GetComponent<GridRPG.Space>().setTerrain("void");
			return ret;
		}
		public static GameObject generateGameObject(Terrain terrain)
		{
			GameObject ret = new GameObject();
			ret.AddComponent<GridRPG.Space>();
			ret.GetComponent<GridRPG.Space>().setTerrain(terrain);
			return ret;
		}
		public static GameObject generateGameObject(string name,Terrain terrain)
		{
			GameObject ret = new GameObject(name);
			ret.AddComponent<GridRPG.Space>();
			ret.GetComponent<GridRPG.Space>().setTerrain(terrain);
			return ret;
		}*/
		
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
}

