using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
	public GridRPG.UnitLibrary unitLibrary;
    public GridRPG.MapLibrary mapLibrary;

    //"Map"/"Main Menu"
    private GridRPG.Map map;

    private string gameMode;
    GridRPG.MainMenu mainMenu;

    // Use this for initialization
    void Start () {
        //Lock resolution at 720p
        Screen.SetResolution(1280, 720, true);

        unitLibrary = new GridRPG.UnitLibrary();
        mapLibrary = new GridRPG.MapLibrary(unitLibrary);
        gameMode = "Main Menu";

        mainMenu = new GridRPG.MainMenu(mapLibrary, unitLibrary);

        //Add the campaign units
        unitLibrary.addUnit(new GridRPG.CampaignUnit("George"));

        //Add the maps
        mapLibrary.addMap("Assets/Resources/Maps/MapA.xml", -1);
        mapLibrary.addMap("Assets/Resources/Maps/MapB.xml", -1);
        mapLibrary.addMap("Assets/Resources/Maps/MapC.xml", -1);

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Debug.Log(width + "," + height);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
            mainMenu.Mode = GridRPG.MainMenu.Modes.Main;
        }
    }
	
	void LateUpdate(){
		//map.mapParent.transform.localScale.Scale(new Vector3(2,2,1));
	}

    public void LoadMapA()
    {

    }
}
