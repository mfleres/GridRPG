using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace GridRPG
{
	
	
	class Map 
	{
		//Center is (0,0)
		Vector3 worldToPixel(Vector3 worldCoords)
		{
			return new Vector3(worldCoords.x*100,worldCoords.y*100,worldCoords.z);
		}
		Vector3 pixelToWorld(Vector3 pixelCoords)
		{
			return new Vector3(pixelCoords.x/100f,pixelCoords.y/100f,pixelCoords.z);
		}
		
		public const int layer = 3;
		
		
		//private List<GridRPG.Terrain> _terrainList;
		//private GameObject[,] _spaceObjects;
		private GridRPG.Space[,] _spaceObjects;
		private int mapWidth;
		private int mapLength;
		private List<EventCondition> _eventConditions;
		public GameObject mapParent;

		public Map()
		{
			_spaceObjects = new GridRPG.Space[0,0];
			mapWidth = 0;
			mapLength = 0;
			_eventConditions = new List<EventCondition>();
			mapParent = new GameObject("Map");
			mapParent.AddComponent<MapControl>();
		}
		
		//returns null if incompatable xml
		public Map(string filename)
		{
			mapParent = new GameObject("Map");
			mapParent.AddComponent<MapControl>();
			//mapParent.transform.localScale=new Vector3(2,2,1);
			
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filename);

			_eventConditions = new List<EventCondition>();
			
			//Kind of a whore
			//GameObject tempObj = new GameObject("Map:tempObj");
			//tempObj.AddComponent<GridRPG.Space>();
			//tempObj.GetComponent<GridRPG.Space>().setTerrain("void");
			
			XmlNode mapNode = xmlDoc.DocumentElement.SelectSingleNode("/map");
			if(mapNode != null)
			{
				//Sprite blackVert = Sprite.Create(Texture2D.blackTexture,new Rect(0f,0f,highlight_width,Terrain.terrain_dim),new Vector2(0.5,0.5));
				//Sprite blackHoriz = Sprite.Create(Texture2D.blackTexture,new Rect(0f,0f,Terrain.terrain_dim,highlight_width),new Vector2(0.5,0.5));
				
				//init spaces[,]
				mapWidth = 0;
				mapLength = 0;
				Int32.TryParse(mapNode.Attributes["width"].Value,out mapWidth);
				Int32.TryParse(mapNode.Attributes["length"].Value,out mapLength);
				_spaceObjects = new GridRPG.Space[mapWidth,mapLength];
				
				//init custom terrains
				XmlNode terrainListNode = mapNode.SelectSingleNode("terrainList");
				if( terrainListNode == null){
					Debug.Log(mapNode.HasChildNodes);
					Debug.Log(mapNode.InnerXml);
				}
				foreach (XmlNode textureXml in terrainListNode.SelectNodes("terrain"))
				{
					string type = textureXml.Attributes["type"].Value;
					int xOffset = 0;
					int yOffset = 0;
					string filePath = textureXml.Attributes["file"].Value;
					
					Int32.TryParse(textureXml.Attributes["spriteXOffset"].Value, out xOffset);
					Int32.TryParse(textureXml.Attributes["spriteYOffset"].Value, out yOffset);
					
					GridRPG.Terrain tempTerrain = new GridRPG.Terrain(type, filePath, xOffset, yOffset);
				}
				
				//fill spaces[,]
				XmlNode spacesNode = mapNode.SelectSingleNode("spaceMap");
				int y;
				for(y = 0; y < spacesNode.SelectNodes("row").Count && y < mapWidth; y++)
				{
					XmlNode rowNode = spacesNode.SelectNodes("row")[y];
					int x;
					for(x = 0; x < rowNode.SelectNodes("space").Count && x < mapLength; x++)
					{
						XmlNode spaceNode = rowNode.SelectNodes("space")[x];
						if(spaceNode.Attributes["terrain"] != null)
						{
							//_spaceObjects[y,x] = GridRPG.Space.generateGameObject("("+x.ToString()+","+y.ToString()+")",new Terrain(spaceNode.Attributes["terrain"].Value));
							_spaceObjects[y,x] = new GridRPG.Space("("+x.ToString()+","+y.ToString()+")",new Terrain(spaceNode.Attributes["terrain"].Value));

							_spaceObjects[y,x].core.GetComponent<Transform>().localPosition = new Vector3(x*(Terrain.terrain_dim+Space.highlight_width)/100f,y*(Terrain.terrain_dim+Space.highlight_width)/100f,layer);
							_spaceObjects[y,x].core.transform.SetParent(mapParent.transform);
							
							
						}
						else{
							//Make the object a void
							//_spaceObjects[y,x] = GridRPG.Space.generateGameObject("MapA:("+x.ToString()+","+y.ToString()+")");
							_spaceObjects[y,x] = new GridRPG.Space("MapA:("+x.ToString()+","+y.ToString()+")");
							_spaceObjects[y,x].core.transform.SetParent(mapParent.transform);
						}
						
						//fill the rest in with void spaces if not defined
						//TODO
					}
				}
				
			}
			else //No mapNode in xml
			{
				//fill with defaults
				_spaceObjects = new GridRPG.Space[0,0];
				mapWidth = 0;
				mapLength = 0;
				_eventConditions = new List<EventCondition>();
			}
			
		}
		
		public void addUnitToSpace(Unit unit, int x, int y)
		{
			unit.warpToSpace(_spaceObjects[y,x]);
		}
		
		/// <summary>
		/// Centers the Map on a Camera.
		/// </summary>
		/// <returns>
		/// The new Map location in World coordinates
		/// </returns>
		/// <param name='camera'>
		/// Camera to center on.
		/// </param>
		public Vector3 centerMapOnCamera(Camera camera)
		{
			Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3((float)Screen.width/2f, (float)Screen.height/2f, 0));
			
			Vector3 mapBLToMid = new Vector3((float)mapLength*(Terrain.terrain_dim+Space.highlight_width)/2f/100f,(float)mapWidth*(Terrain.terrain_dim+Space.highlight_width)/2f/100f,-layer);
			
			Debug.Log("cameraCenter: ("+cameraCenter.x+","+cameraCenter.y+")");
			Debug.Log("mapBLToMid: ("+mapBLToMid.x+","+mapBLToMid.y+")");
			Vector3 newMapPos = cameraCenter - mapBLToMid;
			Debug.Log("mnewMapPos: ("+newMapPos.x+","+newMapPos.y+")");
			
			mapParent.transform.position = newMapPos;
			return newMapPos;
		}
	}
}
