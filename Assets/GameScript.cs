using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
    public GridRPG.Game game;

    private GridRPG.MainMenu mainMenu;

    // Use this for initialization
    void Start () {
        //Lock resolution at 720p
        Screen.SetResolution(1280, 720, true);

        game = new GridRPG.Game();

        //Add the campaign units
        game.unitLibrary.addUnit(new GridRPG.CampaignUnit("George"));

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Debug.Log(width + "," + height);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
            game.ui.Mode = GridRPG.UI.Modes.Main;
        }
    }
	
	void LateUpdate() {

	}

    public void LoadMapA()
    {

    }
}
