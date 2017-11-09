using UnityEngine;
//using UnityEditor;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GridRPG
{
    public class UnitLibrary
    {
        private const string UNIT_FOLDER_FILEPATH = "Units/";

        public Game game;
        public List<GameObject> unitLibrary;
        public List<GameObject> campaignUnits;
        private GameObject core;

        public UnitLibrary(Game game)
        {
            this.game = game;
            unitLibrary = new List<GameObject>(0);
            campaignUnits = new List<GameObject>(0);
            core = new GameObject("Unit Library");
            //Debug.Log("Capacity = " + campaignUnits.Count);
        }

        public int addUnit(GameObject unit, int id, string type)
        {
            switch (type)
            {
                case "campaign":
                    return addCampaignUnit(unit, id);
                case "npc":
                    return addNPCUnit(unit, id);
            }
            throw new ArgumentException("type not valid");
        }

        public int addCampaignUnit(GameObject unit, int id)
        {
            if (id < 0)
            {
                for (int i = 0; i < campaignUnits.Capacity; i++)
                {
                    if (campaignUnits[i] == null)
                    {
                        return addCampaignUnit(unit, i);
                    }
                }
                return addCampaignUnit(unit, campaignUnits.Capacity);
            }

            if (campaignUnits.Count < id + 1)
            {
                int oldCount = campaignUnits.Count;
                //campaignUnits.Count = id + 1;
                for(int i = oldCount; i < id + 1; i++)
                {
                    campaignUnits.Add(null);
                }
            }
            else if (campaignUnits[id] != null)
            {   //destroy old unit
                GameObject.Destroy(campaignUnits[id]);
                campaignUnits[id] = null;
            }
            //Debug.Log("NEW COUNT = " + campaignUnits.Count);

            unit.transform.parent = core.transform;
            campaignUnits[id] = unit;
            campaignUnits[id].SetActive(false);

            Debug.Log("ADDED " + unit.name + " to campaignUnits with id: " + id);
            return id;
           
        }


        public int addNPCUnit(GameObject unit, int id)
        {
            Debug.Log("addUnit(" + unit.name + ", " + id + ") function start");
            if (id < 0)
            {
                for (int i = 0; i < campaignUnits.Capacity; i++)
                {
                    if (campaignUnits[i] == null)
                    {
                        return addNPCUnit(unit, i);
                    }
                }
                return addNPCUnit(unit, campaignUnits.Capacity);
            }

            if (unitLibrary.Count < id + 1)
            {
                
                int oldCount = unitLibrary.Count;
                for (int i = oldCount; i < id + 1; i++)
                {
                    unitLibrary.Add(null);
                }
                Debug.Log("Increased unitLibrary.Count to " + unitLibrary.Count);
            }
            else if (id == -1)
            {   //Add to end
                int oldCount = unitLibrary.Count;
                unitLibrary.Add(null);
                id = oldCount;
            }
            else if (unitLibrary[id] != null)
            {   //destroy old unit
                GameObject.Destroy(unitLibrary[id]);
                unitLibrary[id] = null;
            }
            //Debug.Log("NEW COUNT = " + campaignUnits.Count);

            unit.transform.parent = core.transform;
            unitLibrary[id] = unit;
            unitLibrary[id].SetActive(false);

            Debug.Log("ADDED " + unit.name + " to unitLibrary with id: " + id);
            return id;
        }

        public GameObject getUnit(int id, string type)
        {
            switch(type)
            {
                case "campaign":
                    if (id < campaignUnits.Count)
                    {
                        return campaignUnits[id];
                    }
                    else
                    {
                        return null;
                    }
                case "npc":
                    if (id < campaignUnits.Count)
                    {
                        return unitLibrary[id];
                    }
                    else
                    {
                        return null;
                    }
            }
            return null;
        } 

        /// <summary>
        /// Loads the contents of the file into campaignUnits. Resets the current contents of campaignUnits.
        /// </summary>
        /// <param name="file">File path.</param>
        public void loadCampaignUnitList(string filepath)
        {
            TextAsset file = Resources.Load<TextAsset>(filepath);
            if(file == null)
            {
                Debug.Log("File read fail");
                throw new ArgumentException("File "+ filepath +" could not be read");
            }
            System.IO.StringReader reader = new System.IO.StringReader(file.text);

            if(reader != null)
            {
                string line;
                campaignUnits = new List<GameObject>(0);
                while ((line = reader.ReadLine())!= null)
                {
                    string[] values = line.Split(',');

                    int id = int.Parse(values[0]);

                    Debug.Log("Adding " + UNIT_FOLDER_FILEPATH + values[1]);
                    GameObject unit = loadUnit(UNIT_FOLDER_FILEPATH + values[1]);
                    Debug.Log("Created " + unit.name + " for library");
                    addUnit(unit, id, "campaign");
                }
            }
        }

        public void loadUnitList(string filepath)
        {
            TextAsset file = Resources.Load<TextAsset>(filepath);
            if (file == null)
            {
                Debug.Log("File read fail");
                throw new ArgumentException("File " + filepath + " could not be read");
            }
            System.IO.StringReader reader = new System.IO.StringReader(file.text);

            if (reader != null)
            {
                string line;
                unitLibrary = new List<GameObject>(0);
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    int id = int.Parse(values[0]);

                    Debug.Log("Adding " + UNIT_FOLDER_FILEPATH + values[1]);
                    GameObject unit = loadUnit(UNIT_FOLDER_FILEPATH + values[1]);
                    Debug.Log("Created " + unit.name + " for library");
                    addUnit(unit, id, "npc");
                }
            }
        }

        private GameObject loadUnit(string file)
        {
            GameObject ret = new GameObject();
            Unit retUnit = ret.AddComponent<Unit>();
            retUnit.game = this.game;
            retUnit.loadFromFile(file);
            return ret;
        }
    }
}