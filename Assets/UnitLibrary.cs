using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace GridRPG
{
    public class UnitLibrary
    {
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
            Debug.Log(campaignUnits.Count);
            Debug.Log(id);
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
            Debug.Log("NEW COUNT = " + campaignUnits.Count);

            unit.id = id;
            unit.name = "campaignUnit" + unit.id;
            unit.core.name = unit.name;
            unit.core.transform.parent = core.transform;
            campaignUnits[id] = unit;

            Debug.Log("ADDED");
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
    }
}