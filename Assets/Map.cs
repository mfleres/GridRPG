using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace GridRPG
{
	
	
	public class Map : MonoBehaviour
	{
		//Center is (0,0)
		private Vector3 worldToPixel(Vector3 worldCoords)
		{
			return new Vector3(worldCoords.x*100,worldCoords.y*100,worldCoords.z);
		}
		private Vector3 pixelToWorld(Vector3 pixelCoords)
		{
			return new Vector3(pixelCoords.x/100f,pixelCoords.y/100f,pixelCoords.z);
		}
		
		public const int layer = 3;
        public const string TERRAIN_LAYER = "Terrain";
        public const float ADVANCE_TURNS_WAIT_TIME = 1f;
		
		//private List<GridRPG.Terrain> _terrainList;
		//private GameObject[,] _spaceObjects;
		private GameObject[,] _spaceObjects;
		public int mapWidth { get; private set; }
		public int mapLength { get; private set; }
        private List<EventCondition> _eventConditions;
		//public GameObject mapParent;
        public int id;

        public List<GameObject> turnOrder { get; private set; }
        public int currentTurn { get; private set; }
        public GameObject currentUnit { get; private set; }
        public enum TurnPhase { Move, Skill }
        public TurnPhase currentTurnPhase = TurnPhase.Move;
        public bool advanceTurnsFlag = false;
        public float advanceTurnsWaitStartTime = 0f;

        public void init()
		{
			_spaceObjects = new GameObject[0,0];
			mapWidth = 0;
			mapLength = 0;
			_eventConditions = new List<EventCondition>();
			//mapParent = new GameObject("Map");
			this.gameObject.AddComponent<MapControl>();
            turnOrder = new List<GameObject>();
            currentTurn = 0;
            currentUnit = null;
            Space.selectEvent += spaceSelected;
		}
		
		//returns null if incompatable xml
		public void init(string filename, UnitLibrary unitLibrary, int id)
		{
            GameObject mapParent = this.gameObject;
            this.id = id;
            //mapParent = new GameObject("Map");
            gameObject.AddComponent<MapControl>();
            turnOrder = new List<GameObject>();
            currentTurn = 0;
            currentUnit = null;
            Space.selectEvent += spaceSelected;

            _eventConditions = new List<EventCondition>();

            XmlDocument xmlDoc = new XmlDocument();
            TextAsset file = Resources.Load(filename) as TextAsset;
			xmlDoc.LoadXml(file.text);	
			
			XmlNode mapNode = xmlDoc.DocumentElement.SelectSingleNode("/map");
			if(mapNode != null)
			{
                //Sprite blackVert = Sprite.Create(Texture2D.blackTexture,new Rect(0f,0f,highlight_width,Terrain.terrain_dim),new Vector2(0.5,0.5));
                //Sprite blackHoriz = Sprite.Create(Texture2D.blackTexture,new Rect(0f,0f,Terrain.terrain_dim,highlight_width),new Vector2(0.5,0.5));

                mapParent.name = mapNode.Attributes["name"]?.Value ?? "Map";
                int width = 0;
                int length = 0;
				Int32.TryParse(mapNode.Attributes["width"].Value,out width);
				Int32.TryParse(mapNode.Attributes["length"].Value,out length);
                mapWidth = width;
                mapLength = length;
                _spaceObjects = new GameObject[mapWidth,mapLength];
				
				//init custom terrains
				XmlNode terrainListNode = mapNode.SelectSingleNode("terrainList");
				if( terrainListNode == null){
					Debug.Log(mapNode.HasChildNodes);
					Debug.Log(mapNode.InnerXml);
				}
				else{
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
				}
				
				//fill spaces[,]
				XmlNode spacesNode = mapNode.SelectSingleNode("spaceMap");
				int y;
				for(y = 0; y < (spacesNode?.SelectNodes("row").Count ?? 0) && y < mapWidth; y++)
				{
					XmlNode rowNode = spacesNode.SelectNodes("row")[y];
					int x;
					for(x = 0; x < (rowNode?.SelectNodes("space").Count ?? 0) && x < mapLength; x++)
					{
						XmlNode spaceNode = rowNode.SelectNodes("space")[x];
						if(spaceNode.Attributes["terrain"] != null)
						{
							//_spaceObjects[y,x] = GridRPG.Space.generateGameObject("("+x.ToString()+","+y.ToString()+")",new Terrain(spaceNode.Attributes["terrain"].Value));
							_spaceObjects[y,x] = Space.newSpace(x,y,new Terrain(spaceNode.Attributes["terrain"].Value));

							_spaceObjects[y,x].GetComponent<Transform>().localPosition = new Vector3(x*(Terrain.terrain_dim)/100f,y*(Terrain.terrain_dim)/100f,layer);
							_spaceObjects[y,x].transform.SetParent(mapParent.transform);
                            


                        }
						else{
                            //Make the object a void
                            //_spaceObjects[y,x] = GridRPG.Space.generateGameObject("MapA:("+x.ToString()+","+y.ToString()+")");
                            Debug.Log("Map File Error: Space (" + x + "," + y + ") Missing terrain attribute");
							_spaceObjects[y,x] = Space.newSpace(x,y,"void");
                            _spaceObjects[y, x].GetComponent<Transform>().localPosition = new Vector3(x * (Terrain.terrain_dim) / 100f, y * (Terrain.terrain_dim) / 100f, layer);
                            _spaceObjects[y,x].transform.SetParent(mapParent.transform);
						}

                        XmlNode unitNode = spaceNode.SelectSingleNode("unit");
                        if(unitNode?.Attributes["type"] != null)
                        {
                            int unitID = 0;
                            Int32.TryParse(unitNode.Attributes["idnum"].Value, out unitID);
                            GameObject newUnitObject = Unit.copy(unitLibrary.getUnit(unitID, unitNode.Attributes["type"].Value));
                            turnOrder.Add(newUnitObject);
                            addUnitToSpace(newUnitObject,x,y);
                        }
					}
				}
				
			}
			else //No mapNode in xml
			{
				//fill with defaults
				_spaceObjects = new GameObject[0,0];
				mapWidth = 0;
				mapLength = 0;
				_eventConditions = new List<EventCondition>();
			}

            reorderTurns(Unit.Stat.agility, false);

            centerMapOnCamera(Camera.main);
		}

        public GameObject nextTurn()
        {
            if(turnOrder.Count == 0)
            {
                return null;
            }
            currentTurn++;
            if(currentTurn >= turnOrder.Count)
            {
                currentTurn = 0;
            }
            currentUnit = turnOrder[currentTurn];
            if (currentUnit != null) {
                Game.ui.updateUnitFrame(currentUnit);
                Game.ui.displayMessage("It is now " + currentUnit.name + "'s turn.");
            }
            advanceTurnsFlag = false;
            currentTurnPhase = TurnPhase.Move;
            return currentUnit;
        }

        public void spaceSelected(GameObject spaceObject)
        {
            Debug.Log("Map.spaceSelected(" + spaceObject?.name + ")");
            if (currentUnit != null)
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    //temporary method of melee attack
                    Skill.activateSkill<MeleeAttack>(this.currentUnit, spaceObject.GetComponent<Space>().coordinates);
                    advanceTurnsWaitStartTime = Time.fixedTime;
                    advanceTurnsFlag = true;
                }
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    //temporary method of spell attack
                    Skill.activateSkill<FireBlast>(this.currentUnit, spaceObject.GetComponent<Space>().coordinates);
                    advanceTurnsWaitStartTime = Time.fixedTime;
                    advanceTurnsFlag = true;
                }
                else if(this.currentTurnPhase != TurnPhase.Skill)
                {
                    //move
                    if (this.currentUnit.GetComponent<Unit>().moveToSpace(spaceObject.GetComponent<Space>().coordinates))
                    {
                        currentTurnPhase = TurnPhase.Skill;
                    }
                }
                
            }
        }
		
        public void removeUnit(GameObject unit)
        {
            if(unit?.GetComponent<Unit>() != null && this.currentUnit == unit)
            {
                this.currentUnit = null;
                this.turnOrder.Remove(unit);
                unit.GetComponent<Unit>().deathEvent -= removeUnit;
                if(this.currentTurn >= this.turnOrder.Count)
                {
                    //cycle currentTurn to front
                    this.currentTurn = 0;
                }
                if(this.turnOrder.Count != 0)
                {
                    this.currentUnit = turnOrder[currentTurn];
                }
            }
        }

        /// <summary>
        /// Reorders the turn count based on the units' stats.
        /// </summary>
        /// <param name="stat">Stat to compare.</param>
        /// <param name="useMin">Order the turn order with the lesser stat first.</param>
        public void reorderTurns(Unit.Stat stat, bool useMin)
        {
            GameObject currentTurnUnit = this.turnOrder[currentTurn];
            List<GameObject> sortedTurns = new List<GameObject>();
            List<GameObject> unsortedTurns = this.turnOrder;
            while (unsortedTurns.Count > 0) {
                Debug.Log("Map.reorderTurns(Unit.stat, bool): unsorted.Count = "+unsortedTurns.Count+"; sorted.Count = "+sortedTurns.Count);
                List<GameObject> groupedTurns = new List<GameObject>();
                bool currentStatSet = false;
                uint currentStat = 0;

                //get the current grouping stat
                for (int i = 0; i < unsortedTurns.Count; i++)
                {
                    Unit currentUnit = unsortedTurns[i]?.GetComponent<Unit>();
                    if (currentUnit != null)
                    {
                        uint currentUnitStat = currentUnit.getStat(stat);
                        if (!currentStatSet)
                        {
                            currentStat = currentUnitStat;
                            currentStatSet = true;
                        }
                        else
                        {
                            if(useMin && currentStat > currentUnitStat)
                            {
                                currentStat = currentUnitStat;
                            }
                            else if (!useMin && currentStat < currentUnitStat)
                            {
                                currentStat = currentUnitStat;
                            }
                        }
                    }
                    else
                    {
                        //Not a unit, but in case the object was there for a reason, move it to new list.
                        sortedTurns.Add(unsortedTurns[i]);
                        unsortedTurns.RemoveAt(i);
                        i--;
                    }
                }

                //get the group of units with the stat tie
                for(int i = unsortedTurns.Count - 1; i >= 0; i--)
                {
                    if(unsortedTurns[i].GetComponent<Unit>().getStat(stat) == currentStat)
                    {
                        groupedTurns.Add(unsortedTurns[i]);
                        unsortedTurns.RemoveAt(i);
                    }
                }

                //add the tied units to the sorted list randomly
                while(groupedTurns.Count > 0)
                {
                    int randomInt = UnityEngine.Random.Range(0, groupedTurns.Count);
                    if(randomInt != groupedTurns.Count)
                    {
                        sortedTurns.Add(groupedTurns[randomInt]);
                        groupedTurns.RemoveAt(randomInt);
                    }
                }
            }

            this.turnOrder = sortedTurns;
            //return the current unit to be active
            this.currentUnit = currentTurnUnit;
            this.currentTurn = turnOrder.IndexOf(this.currentUnit);
        }

		public void addUnitToSpace(GameObject unit, int x, int y)
		{
            if (unit?.GetComponent<Unit>() != null)
            {
                unit.transform.SetParent(this.gameObject.transform);
                unit.GetComponent<Unit>().deathEvent += removeUnit;
                unit.GetComponent<Unit>().warpToSpace(new Vector2(x, y), this);
            }
		}

        public Space getSpace(Vector2 coords)
        {
            return getSpace((int)coords.x, (int)coords.y);
        }

        public Space getSpace(int x, int y)
        {
            if (x < 0 || x > mapLength - 1 || y < 0 || y > mapWidth - 1)
            {
                return null; //easier than using exceptions.
            }

            //Debug.Log("Map.getSpace(" + x + ", " + y + ")");
            GameObject space = _spaceObjects[y, x];
            return space.GetComponent<Space>();
        }

        public Unit getUnitOnSpace(Vector2 coords)
        {
            return getSpace(coords)?.unit?.GetComponent<Unit>();
        }

        /// <summary>
        /// Determines if a given coordinate is valid within the map.
        /// </summary>
        /// <param name="locationCoord"></param>
        /// <returns></returns>
        public bool isLocationValid(Vector2 locationCoord)
        {
            return (locationCoord != null && locationCoord.x >= 0 && locationCoord.x < mapLength && locationCoord.y >= 0 && locationCoord.y < mapWidth);
        }

        /// <summary>
        /// Gets the number of spaces between two coordinates.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        public int getDistance(Vector2 location1, Vector2 location2)
        {
            if(!isLocationValid(location1) || !isLocationValid(location2))
            {
                //one of the arguments is invalid
                return -1;
            }
            Debug.Log("Map.getDistance("+location1+","+location2+"): returns (" + Math.Abs(location2.x - location1.x) + " + " + Math.Abs(location2.y - location1.y) + ")");
            return (int)(Math.Abs(location2.x - location1.x) + Math.Abs(location2.y - location1.y));
        }

        /// <summary>
        /// Gets the direct distance between two spaces.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        public float getDistanceLine(Vector2 location1, Vector2 location2)
        {
            if (!isLocationValid(location1) || !isLocationValid(location2))
            {
                //one of the arguments is invalid
                return -1;
            }
            return (float)Math.Sqrt(Math.Pow(Math.Abs(location2.x - location1.x), 2) + Math.Pow(Math.Abs(location2.y - location1.y), 2));
        }

        /// <summary>
        /// Determines if two spaces meet range criteria. Ignores line of sight.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        public bool inRange(Vector2 location1, Vector2 location2, float minRange, float maxRange)
        {
            int distance = getDistance(location1, location2);
            Debug.Log("Map.inRange(): distance = " + distance);
            if(distance < 0)
            {
                Debug.Log("one of the location arguments is invalid.");
                return false;
            }
            else
            {
                Debug.Log("Map.inRange returns (" + (distance >= minRange) + " && " + (distance <= maxRange) + ")");
                return (distance >= minRange && distance <= maxRange);
            }
        }

        /// <summary>
        /// Determines whether two spaces are within range criteria.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="lineOfSightRequired">Adds line of sight check.</param>
        /// <returns></returns>
        public bool inRangeLine(Vector2 location1, Vector2 location2, float minRange, float maxRange,bool lineOfSightRequired)
        {
            float distance = getDistanceLine(location1, location2);
            if(distance < 0)
            {
                //one of the location arguments is invalid.
                return false;
            }
            else if((distance >= minRange && distance <= maxRange))
            {
                //spaces are in range, check line of sight if needed
                if (lineOfSightRequired)
                {
                    return inLineOfSight(location1, location2);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the two locations are within line of sight of eachother.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        public bool inLineOfSight(Vector2 location1, Vector2 location2)
        {
            if(!isLocationValid(location1) || !isLocationValid(location2))
            {
                return false;
            }
            Vector4 lineSegment = generateMathLineSegment(location1, location2);
            if(lineSegment.w == -1)
            {
                //vertical
                for(float j = lineSegment.y; j <= lineSegment.z; j += 1)
                {
                    if(!getSpace(new Vector2(lineSegment.x, j)).lineOfSight)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //other
                //NOTE: this method is inefficient, but the penalty for double-checking spaces is low.
                //check vertical boundaries
                for(float i = lineSegment.y; i <= lineSegment.z; i += 1)
                {
                    float j = lineSegment.w * i + lineSegment.x;
                    //check spaces
                    if (!getSpace(new Vector2(i-0.5f, j)).lineOfSight || !getSpace(new Vector2(i + 0.5f, j)).lineOfSight)
                    {
                        return false;
                    }
                }
                //check horizontal boundaries
                if (lineSegment.w != 0)
                {
                    //not horizontal
                    Vector4 lineSegmentInverse = inverseLineSegment(lineSegment);
                    for (float i = lineSegment.y; i <= lineSegment.z; i += 1)
                    {
                        float j = lineSegment.w * i + lineSegment.x;
                        //check spaces
                        if (!getSpace(new Vector2(j, i - 0.5f)).lineOfSight || !getSpace(new Vector2(j, i + 0.5f)).lineOfSight)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Generates a vector representing the segment between the centers of 2 spaces.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns>Vector4 with segment information (x,y,x1,x2). If line is vertical, format is (-1,x1,y1,y2) </returns>
        private Vector4 generateMathLineSegment(Vector2 location1, Vector2 location2)
        {
            if(location1.y == location2.y)
            {
                //horizontal line
                if (location1.x < location2.x)
                {
                    return new Vector4(0, location1.x + 0.5f, location1.x + 0.5f, location2.x + 0.5f);
                }
                else
                {
                    return new Vector4(0, location2.x + 0.5f, location2.x + 0.5f, location1.x + 0.5f);
                }
            }
            else if(location1.x == location2.x)
            {
                //vertical line
                if(location1.y < location2.y)
                {
                    return new Vector4(-1, location1.x + 0.5f, location1.y + 0.5f, location2.y + 0.5f);
                }
                else
                {
                    return new Vector4(-1, location2.x + 0.5f, location2.y + 0.5f, location1.y + 0.5f);
                }
            }
            else
            {
                //angled line
                float m = (location2.y - location1.y) / (location2.x - location1.x);
                float c = location1.y + 0.5f;
                return new Vector4(m, c, location1.x + 0.5f, location2.x + 0.5f);
            }
        }

        private Vector4 inverseLineSegment(Vector4 source)
        {
            float y1 = source.w * source.y + source.x;
            float y2 = source.w * source.z + source.x;
            if(y2 < y1)
            {
                //swap
                float temp = y1;
                y1 = y2;
                y2 = temp;
            }
            float m = 1 / source.w;
            float b = -source.x / source.w;
            return new Vector4(m, b, y1, y2);
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
        public Vector3? centerMapOnCamera(Camera camera)
		{
			if (camera != null){
				Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3((float)Screen.width/2f, (float)Screen.height/2f, 0));
				
				Vector3 mapBLToMid = new Vector3((float)mapLength*(Terrain.terrain_dim+Space.highlight_width)/2f/100f,(float)mapWidth*(Terrain.terrain_dim+Space.highlight_width)/2f/100f,-layer);
				
				//Debug.Log("cameraCenter: ("+cameraCenter.x+","+cameraCenter.y+")");
				//Debug.Log("mapBLToMid: ("+mapBLToMid.x+","+mapBLToMid.y+")");
				Vector3 newMapPos = cameraCenter - mapBLToMid;
                //Debug.Log("mnewMapPos: ("+newMapPos.x+","+newMapPos.y+")");

                gameObject.transform.position = newMapPos;
				return newMapPos;
			}
			else{
				return null;	
			}
		}
	}
}
