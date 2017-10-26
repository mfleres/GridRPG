using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
    public GridRPG.Game game;

    //private GridRPG.MainMenu mainMenu;

    // Use this for initialization
    void Start () {
        //Set resolution to wide 480p-windowed
        Screen.SetResolution(853, 480, false);

        game = new GridRPG.Game();

        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Debug.Log(width + "," + height);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q))
        {
            GridRPG.Game.ui.Mode = GridRPG.UI.Modes.Main;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
	
	void LateUpdate() {

	}

    public void LoadMapA()
    {

    }
}
