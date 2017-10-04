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

        public List<Unit> unitLibrary;
        public List<CampaignUnit> campaignUnits;
        private GameObject core;

        public UnitLibrary()
        {
            unitLibrary = new List<Unit>(0);
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

        //returns index
        public int addUnit(Unit unit, string type)
        {
            if(type == "campaign")
            {
                return addUnit((CampaignUnit)unit);
            }
            return -1;
        }

        public int addUnit(Unit unit, int id, string type)
        {
            if (type == "campaign")
            {
                return addUnit((CampaignUnit)unit, id);
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

        private CampaignUnit loadCampaignUnit(string file)
        {
            return new CampaignUnit(file);
        }
    }
}