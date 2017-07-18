using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
	private GridRPG.UnitLibrary unitLibrary;
	
	private GridRPG.Map map;
	
	
	
	
	// Use this for initialization
	void Start () {
        unitLibrary = new GridRPG.UnitLibrary();
        //unitLibrary.campaignUnits.Add(new GridRPG.CampaignUnit("George", unitLibrary.campaignUnits.Capacity+1));
        unitLibrary.addUnit(new GridRPG.CampaignUnit("George"));
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
            GameObject.Destroy(map.mapParent);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            map = new GridRPG.Map("Assets/Resources/Maps/MapA.xml",unitLibrary);
            map.centerMapOnCamera(Camera.main);
            
        }
    }
	
	void LateUpdate(){
		//map.mapParent.transform.localScale.Scale(new Vector3(2,2,1));
	}
}
