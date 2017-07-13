using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
	public class Terrain
	{
		//filepath to core terrain variants. This must exist
		private const string core_filepath = "Sprites/Terrain/Core";
		public const float terrain_dim = 32.0f;
		
		//List of all terrain variants used
		private static List<GridRPG.Terrain> terrain_list = new List<GridRPG.Terrain>();
		
		private Sprite _sprite = null;
		private String _type = null;

		public Terrain ()
		{
			initTerrainList();
			_sprite = null;
			_type = null;
		}
		
		public Terrain (string type, Sprite sprite)
		{
			initTerrainList();
			
			Terrain search = terrain_list.Find(x => x._type == type);
			if (search == null)
			{	//Need to update terrain_list
				_type = type;
				_sprite = sprite;
				terrain_list.Add(this);
			}
			else
			{	//Get straight to the point
				_sprite = search._sprite;
				_type = type;
			}
		}
		
		public Terrain (string type, string filepath, int x_offset, int y_offset)
		{
			initTerrainList();
			
			Terrain search = terrain_list.Find(x => x._type == type);
			if (search == null)
			{	//Need to load new Sprite and update terrain_list
				_type = type;
				Texture2D spriteSheet = (Texture2D)Resources.Load(filepath,typeof(Texture2D));
				_sprite = Sprite.Create(spriteSheet,new Rect((float)(x_offset*terrain_dim),(float)(y_offset*terrain_dim),terrain_dim,terrain_dim),new Vector2(0.5f,0.5f));
				terrain_list.Add(this);
			}
			else
			{	//Get straight to the point
				_sprite = search._sprite;
				_type = type;
			}
		}
		
		public Terrain(Terrain terrain)
		{
			initTerrainList();
			_sprite = terrain.Sprite;
			_type = terrain.Type;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GridRPG.Terrain"/> class.
		/// Searches database for 'type' and autofills sprite. If not found, returns void space
		/// </summary>
		/// <param name='type'>
		/// Type.
		/// </param>
		public Terrain(string type)
		{
			initTerrainList();
			Terrain search = terrain_list.Find(x => x._type == type);
			if (search != null)
			{
				_type = type;
				_sprite = search.Sprite;
			}
			else
			{
				_sprite = terrain_list[0].Sprite;
				_type = terrain_list[0].Type;
			}
		}

		public Sprite Sprite
		{
			get{
				return _sprite;
			}
		}

		public String Type
		{
			get{
				return _type;
			}
		}
		
		private void initTerrainList()
		{
			if(terrain_list == null || terrain_list.Capacity == 0 || terrain_list[0].Type != "void")
			{
				Debug.Log("INITIALIZING TERRAINLIST");
				terrain_list.Clear();
				Sprite tempSprite = Sprite.Create((Texture2D)Resources.Load(core_filepath,typeof(Texture2D)),new Rect(0.0f,0.0f,terrain_dim,terrain_dim),new Vector2(0.5f,0.5f));	
				terrain_list.Add(new GridRPG.Terrain("void",tempSprite,true));
				tempSprite = Sprite.Create((Texture2D)Resources.Load(core_filepath,typeof(Texture2D)),new Rect(terrain_dim,terrain_dim,terrain_dim,terrain_dim),new Vector2(0.5f,0.5f));
				terrain_list.Add(new GridRPG.Terrain("empty",tempSprite,true));
			}
		}
		
		//Some messy recursion handling
		private Terrain (string type, Sprite sprite, bool initOverride)
		{
			if(initOverride){
				_type = type;
				_sprite = sprite;
			}
			else{
				initTerrainList();
				Terrain search = terrain_list.Find(x => x._type == type);
				if (search != null)
				{
					_type = type;
					_sprite = sprite;
					terrain_list.Add(this);
				}
				else
				{
					_sprite = sprite;
					_type = type;
				}
			}
		}
	}
}

