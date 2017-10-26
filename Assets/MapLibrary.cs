using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System;

namespace GridRPG
{
    public class MapLibrary
    {
        /// <summary>
        /// Contains the name and filepath for the map
        /// </summary>
        public struct MapEntry
        {
            public string name, file;

            /// <summary>
            /// Constructs a mapEntry. If either parameter is null, both values are set to null.
            /// </summary>
            /// <param name="name">Name of the map</param>
            /// <param name="file">Filepath of the map XML document.</param>
            public MapEntry(string name, string file)
            {
                if (name != null && file != null)
                {
                    this.name = name;
                    this.file = file;
                }
                else
                {
                    this.name = null;
                    this.file = null;
                }
            }

            /// <summary>
            /// Constructs a mapEntry. If the file loading fails or the map element does not contain a name, both values are set to null.
            /// </summary>
            /// <param name="file">Filepath of the map XML document.</param>
            public MapEntry(string file)
            {
                if (file != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file);
                    XmlNode mapNode = xmlDoc?.DocumentElement.SelectSingleNode("/map");

                    this.name = mapNode?.Attributes["name"]?.Value;
                    if (this.name != null)
                    {
                        this.file = file;
                    }
                    else
                    {
                        this.file = null;
                    }
                }
                else
                {
                    this.name = null;
                    this.file = null;
                }
            }
        }

        public List<MapEntry> mapList;      //List of map filepaths. Unlike units, maps are too large to load all at once
        public Game game;     //Game 
        //public Map map;                     //Loaded map. Only one can be loaded at a time.

        public MapLibrary(Game game)
        {
            mapList = new List<MapEntry>();
            this.game = game;
            //this.map = null;
        }

        /// <summary>
        /// Adds a map to the library at index id. Overwrites any map that already exists with that id.
        /// </summary>
        /// <param name="filepath">Filepath of the map XML document.</param>
        /// <param name="id">Index of the map in mapList. If id is less than 0, the map is added to the end of the library</param>
        /// <returns>The index of the map in mapList</returns>
        public int addMap(string filepath, int id)
        {

            if(id < 0)
            {   //make map at next space
                for(int i = 0; i < mapList.Count; i++)
                {
                    if (mapList[i].name == null)
                    {
                        mapList[i] = new MapEntry(filepath);
                        Debug.Log("Added " + mapList[i].name + " to library at index " + i);
                        return i;
                    }
                }

                //add to end of mapList
                mapList.Add(new MapEntry(filepath));
                Debug.Log("Added " + mapList[mapList.Count-1].name + " to library at index " + (mapList.Count - 1));
                return mapList.Count - 1;
            }

            if (mapList.Capacity < id + 1)
            {   //Expand mapList to hold the map
                int oldCount = mapList.Count;
                
                for (int i = oldCount; i < id + 1; i++)
                {
                    mapList.Add(new MapEntry(null));
                }
            }
            else if (mapList[id].name != null)
            {   //destroy old map reference
                mapList[id] = new MapEntry(null);
            }

            mapList[id] = new MapEntry(filepath);

            Debug.Log("Added " + mapList[id] + " to library at index " + id);
            return id;
        }

        /// <summary>
        /// Adds a map to the end of the library
        /// </summary>
        /// <param name="filepath">Filepath of the map XML document.</param>
        /// <returns>The index of the map in mapList</returns>
        public int addMap(string filepath)
        {
            return addMap(filepath, -1);
        }

        public string getMapFile(int index)
        {
            if (mapList.Capacity > index)
            {
                return mapList[index].file;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Loads a specific map. Unloads the current active map.
        /// </summary>
        /// <param name="id">Index of the map to load.</param>
        /// <returns>The loaded map.</returns>
        public Map loadMap(int id)
        {
            Debug.Log("Loading Map with id = " + id);
            if(Game.map != null)
            {
                unloadMap();
            }

            Game.map = new Map(mapList[id].file, Game.unitLibrary, id);
            return Game.map;
        }

        /// <summary>
        /// Destroys the current active map
        /// </summary>
        public void unloadMap()
        {
            if (Game.map != null)
            {
                GameObject.Destroy(Game.map.mapParent);
                Game.map = null;
            }
        }

        /// <summary>
        /// Returns a list of all the current map names and their indexes.
        /// </summary>
        /// <returns>List Tuples containing each map's name and index.</returns>
        public List<Tuple<string,int>> listMaps()
        {
            List<Tuple<string, int>> ret = new List<Tuple<string, int>>();
            for (int i = 0; i < mapList.Count; i++)
            {
                if(mapList[i].name != null)
                {
                    ret.Add(new Tuple<string, int>(mapList[i].name, i));
                }
            }
            return ret;
        }

        /// <summary>
        /// Loads maps from list text file to the mapLibrary.
        /// </summary>
        /// <param name="filename">File to load from.</param>
        public void loadMapList(string filename)
        {
            string mapFile;
            System.IO.StreamReader file = null;

            try
            {
                file = new System.IO.StreamReader(filename);
            }
            catch (System.Exception e)
            {
                Debug.Log("MAP FILE NOT FOUND");
                file = null;
            }

            while ((mapFile = file.ReadLine()) != null)
            {
                this.addMap(mapFile);
                Debug.Log(mapFile);
            }
        }
    }
}