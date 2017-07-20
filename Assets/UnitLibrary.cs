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

        public int addUnit(CampaignUnit unit)
        {
            //Debug.Log("Capacity* = " + campaignUnits.Count);
            campaignUnits.Add(unit);
            unit.id = campaignUnits.Count;
            unit.name = "campaignUnit" + unit.id;
            unit.core.name = unit.name;
            unit.core.transform.parent = core.transform;
            return campaignUnits.Capacity - 1;
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