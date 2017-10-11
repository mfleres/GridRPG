using UnityEngine;
//using UnityEditor;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GridRPG
{
    public class UnitLibrary
    {
        private const string UNIT_FOLDER_FILEPATH = "Assets/Resources/Units/";

        public List<NPCUnit> unitLibrary;
        public List<CampaignUnit> campaignUnits;
        private GameObject core;

        public UnitLibrary()
        {
            unitLibrary = new List<NPCUnit>(0);
            campaignUnits = new List<CampaignUnit>(0);
            core = new GameObject("Unit Library");
            //Debug.Log("Capacity = " + campaignUnits.Count);
        }

        public int addUnit(CampaignUnit unit, int id)
        {
            //Debug.Log(campaignUnits.Count);
            //Debug.Log(id);
            if(campaignUnits.Count < id + 1)
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
                GameObject.Destroy(campaignUnits[id].core);
                campaignUnits[id] = null;
            }
            //Debug.Log("NEW COUNT = " + campaignUnits.Count);

            unit.core.transform.parent = core.transform;
            campaignUnits[id] = unit;
            campaignUnits[id].core.SetActive(false);

            Debug.Log("ADDED " + unit.name + " to campaignUnits with id: " + id);
            return id;
           
        }

        public int addUnit(CampaignUnit unit)
        {
            if (unit.id == -1)
            {
                for (int i = 0; i < campaignUnits.Capacity; i++)
                {
                    if( campaignUnits[i] == null)
                    {
                        return addUnit(unit, i);
                    }
                }
                return addUnit(unit, campaignUnits.Capacity);
            }
            else
            {
                return addUnit(unit, unit.id);
            }
        }

        public int addUnit(NPCUnit unit, int id)
        {
            Debug.Log("addUnit(" + unit.name + ", " + id + ") function start");
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
                GameObject.Destroy(unitLibrary[id].core);
                unitLibrary[id] = null;
            }
            //Debug.Log("NEW COUNT = " + campaignUnits.Count);

            unit.core.transform.parent = core.transform;
            unitLibrary[id] = unit;
            unitLibrary[id].core.SetActive(false);

            Debug.Log("ADDED " + unit.name + " to unitLibrary with id: " + id);
            return id;
        }

        //returns index
        public int addUnit(Unit unit, string type)
        {
            switch(type)
            {
                case "campaign":
                    return addUnit((CampaignUnit)unit);
                case "npc":
                    return addUnit((NPCUnit)unit,-1);
            }
            return -1;
        }

        public int addUnit(Unit unit, int id, string type)
        {
            switch (type)
            {
                case "campaign":
                    return addUnit((CampaignUnit)unit,id);
                case "npc":
                    return addUnit((NPCUnit)unit, id);
            }
            return -1;
        }

        public Unit getUnit(int id, string type)
        {
            switch(type)
            {
                case "campaign":
                    return getCampaignUnit(id);
            }
            return null;
        } 

        public CampaignUnit getCampaignUnit(int id)
        {
            if(id < campaignUnits.Count)
            {
                return campaignUnits[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Loads the contents of the file into campaignUnits. Resets the current contents of campaignUnits.
        /// </summary>
        /// <param name="file">File path.</param>
        public void loadCampaignUnitList(string file)
        {
            System.IO.StreamReader reader = null;

            try
            {
                reader = new System.IO.StreamReader(file);
            }
            catch (System.Exception e)
            {
                Debug.Log("CampaignUnits file load failure");
                reader = null;
            }

            if(reader != null)
            {
                campaignUnits = new List<CampaignUnit>(0);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    int id = int.Parse(values[0]);

                    Debug.Log("Adding " + UNIT_FOLDER_FILEPATH + values[1]);
                    CampaignUnit unit = loadCampaignUnit(UNIT_FOLDER_FILEPATH + values[1]);
                    Debug.Log("Created " + unit.name + " for library");
                    addUnit(unit, id, "campaign");
                }
            }
        }

        public void loadUnitList(string file)
        {
            System.IO.StreamReader reader = null;

            try
            {
                reader = new System.IO.StreamReader(file);
            }
            catch (System.Exception e)
            {
                Debug.Log("UnitList file load failure");
                reader = null;
            }

            if (reader != null)
            {
                unitLibrary = new List<NPCUnit>(0);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    int id = int.Parse(values[0]);

                    Debug.Log("Adding " + UNIT_FOLDER_FILEPATH + values[1]);
                    NPCUnit unit = loadNPCUnit(UNIT_FOLDER_FILEPATH + values[1]);
                    Debug.Log("Created " + unit.name + " for library");
                    addUnit(unit, id, "npc");
                }
            }
        }

        private CampaignUnit loadCampaignUnit(string file)
        {
            return new CampaignUnit(file);
        }

        private NPCUnit loadNPCUnit(string file)
        {
            return new NPCUnit(file);
        }
    }
}