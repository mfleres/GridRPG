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
        unitLibrary = new GridRPG.UnitLibrary();
        mapLibrary = new GridRPG.MapLibrary();
        gameMode = "Main Menu";

        
        //Debug.Log("Capacity = " + unitLibrary.campaignUnits.Count);
        mainMenu = new GridRPG.MainMenu(mapLibrary, unitLibrary);
        unitLibrary.addUnit(new GridRPG.CampaignUnit("George"));



        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        Debug.Log(width + "," + height);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
            GameObject.Destroy(mapLibrary.map.mapParent);
            mainMenu.canvas.SetActive(true);
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

    /*public class MainMenu : ScriptableObject
    {
        const float originX = 0;
        const float originY = 0;

        public GameObject canvas;
        public GameObject gameStartButton;

        private void OnEnable()
        {
            canvas = new GameObject("Main Menu UI");

            canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            gameStartButton = new GameObject("Game Start Button");
            gameStartButton.transform.parent = canvas.transform;
            gameStartButton.AddComponent<Button>();
            gameStartButton.GetComponent<Button>().onClick.AddListener(LoadMapA);
            gameStartButton.AddComponent<RectTransform>();
            gameStartButton.AddComponent<Image>();
            //gameStartButton.GetComponent<Image>().sprite = //TODO
            RectTransform buttonTransform = gameStartButton.GetComponent<RectTransform>();
            buttonTransform.localPosition = new Vector3(0, 0, 0);

            GameObject buttonText = new GameObject("Text");
            buttonText.transform.parent = gameStartButton.transform;
            buttonText.AddComponent<Text>();
            buttonText.AddComponent<RectTransform>();
            buttonText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            Text coreText = buttonText.GetComponent<Text>();
            coreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            coreText.text = "Main Menu";
            coreText.color = Color.black;

        }
    }*/

    public void LoadMapA()
    {

    }
}
