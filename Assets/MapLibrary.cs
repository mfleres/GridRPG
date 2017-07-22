using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GridRPG
{
    public class MapLibrary
    {
        public List<string> mapList;    //list of map filepaths. Unlike units, maps are too large to load all at once
        public UnitLibrary unitLibrary;
        public Map map; //Loaded map

        public MapLibrary(UnitLibrary unitLibrary)
        {
            mapList = new List<string>();
            this.unitLibrary = unitLibrary;
            this.map = null;
        }

        public int addMap(string filepath, int id)
        {

            if(id < 0)
            { //make map at next space
                for(int i = 0; i < mapList.Count; i++)
                {
                    if (mapList[i] == null)
                    {
                        mapList[i] = filepath;
                        return i;
                    }
                }

                //add to end of mapList
                mapList.Add(filepath);
                return mapList.Count - 1;
            }

            if (mapList.Capacity < id + 1)
            {
                int oldCount = mapList.Count;
                
                for (int i = oldCount; i < id + 1; i++)
                {
                    mapList.Add(null);
                }
            }
            else if (mapList[id] != null)
            {   //destroy old map reference
                mapList[id] = null;
            }

            mapList[id] = filepath;

            return id;
        }

        public string getMap(int index)
        {
            if (mapList.Capacity > index)
            {
                return mapList[index];
            }
            else
            {
                return null;
            }
        }

        public Map loadMap(int id)
        {
            if(map != null)
            {
                unloadMap();
            }

            map = new Map(mapList[id], unitLibrary, id);
            return map;
        }

        public void unloadMap()
        {
            if (map != null)
            {
                GameObject.Destroy(map.mapParent);
                map = null;
            }
        }
    }
}