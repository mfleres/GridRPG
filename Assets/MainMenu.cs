using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GridRPG
{
    public class MainMenu
    {
        const string FONT_FILE = "Fonts/VCR_OSD_MONO_1.001";
        const string BUTTON_FILE = "Sprites/GUI/BlueBox";
        const int BUTTON_SIZE_X = 200;
        const int BUTTON_SIZE_Y = 50;
        const int BUTTON_SPACING_Y = 3; //vertical space between each button
        public enum Modes { Main, MapList, ActiveMap};

        private Modes mode; //Determines which menu to display.
        private List<Tuple<string, int>> mapEntries;

        public GridRPG.MapLibrary mapLibrary;
        public GridRPG.UnitLibrary unitLibrary;

        public MapUI mapUI;

        /// <summary>
        /// Main canvas to display UI elements on.
        /// </summary>
        public GameObject canvas;

        public GameObject mapSelectButton;  //changes mode to MapList
        public GameObject mapList;          //parent of all the map options.

        public MainMenu(MapLibrary mapLibrary, UnitLibrary unitLibrary)
        {
            mapUI = new MapUI(mapLibrary, unitLibrary);
            mapUI.setActive(false);

            canvas = new GameObject("Main Menu UI");
            this.mapLibrary = mapLibrary;
            this.unitLibrary = unitLibrary;
            //Debug.Log("Capacity* = " + unitLibrary.campaignUnits.Count);

            canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();
            coreCanvas.pixelPerfect = true;

            generateMapSelectButton();
        }

        /// <summary>
        /// Gets/Sets the menu mode and activates/deactivates the relative elements
        /// </summary>
        public Modes Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                switch (value)
                {
                    case Modes.Main:
                        mapUI.setActive(false);
                        if (mode == Modes.ActiveMap)
                        {
                            mapLibrary.unloadMap();
                        }

                        canvas.SetActive(true);
                        mapSelectButton.SetActive(true);
                        mapList.SetActive(false);
                        mode = value;
                        break;
                    case Modes.MapList:
                        mapUI.setActive(false);
                        if (mode == Modes.ActiveMap)
                        {
                            mapLibrary.unloadMap();
                        }

                        canvas.SetActive(true);
                        mapSelectButton.SetActive(false);
                        if (mapList)
                        {
                            mapList.SetActive(true);
                        }
                        generateMapList();
                        mode = value;
                        break;
                    case Modes.ActiveMap:
                        canvas.SetActive(false);
                        mapUI.setActive(true);
                        mode = value;
                        break;
                }
            }
        }

        void LoadMapAG()
        {
            Debug.Log("Clicked");
            mapLibrary.loadMap(0);
            canvas.SetActive(false);
        }

        //Loads a the map with index id
        void LoadMap(int id)
        {
            mapLibrary.loadMap(id);
            this.Mode = Modes.ActiveMap;
        }

        /// <summary>
        /// Generates mapSelectButton
        /// </summary>
        private void generateMapSelectButton()
        {
            mapSelectButton = new GameObject("Map Select Button");
            mapSelectButton.transform.SetParent(canvas.transform);
            mapSelectButton.AddComponent<Button>();
            mapSelectButton.GetComponent<Button>().onClick.AddListener(delegate { Mode = Modes.MapList; });

            mapSelectButton.AddComponent<RectTransform>();
            mapSelectButton.AddComponent<Image>();
            mapSelectButton.GetComponent<Image>().sprite = Sprite.Create((Texture2D)Resources.Load(BUTTON_FILE, typeof(Texture2D)), new Rect(0.0f, 0.0f, 32f, 32f), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(3, 3, 3, 3));
            mapSelectButton.GetComponent<Image>().type = Image.Type.Tiled;

            RectTransform buttonTransform = mapSelectButton.GetComponent<RectTransform>();
            buttonTransform.sizeDelta = new Vector2(BUTTON_SIZE_X, BUTTON_SIZE_Y);
            buttonTransform.localPosition = new Vector3(0, 0, 0);
            buttonTransform.localScale = new Vector3(1, 1, 1);

            GameObject buttonText = new GameObject("Text");
            buttonText.transform.SetParent(mapSelectButton.transform);
            buttonText.AddComponent<Text>();
            //buttonText.AddComponent<RectTransform>();
            buttonText.GetComponent<RectTransform>().SetParent(buttonTransform);
            buttonText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            buttonText.GetComponent<RectTransform>().sizeDelta = buttonTransform.sizeDelta;
            Text coreText = buttonText.GetComponent<Text>();
            //coreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            coreText.font = ((Font)Resources.Load(FONT_FILE, typeof(Font))) ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            coreText.fontSize = 25;
            coreText.text = "Select Map";
            coreText.color = Color.white;
            coreText.alignment = TextAnchor.MiddleCenter;
        }

        /// <summary>
        /// Generates a list of buttons for every map.
        /// </summary>
        private void generateMapList()
        {
            Debug.Log("GENERATING MAP SELECT BUTTONS");
            mapEntries = mapLibrary.listMaps();
            if (!mapList)
            {
                mapList = new GameObject("Map List");
                mapList.transform.SetParent(canvas.transform);
                mapList.transform.localPosition = new Vector3(0, 0, 0);
            } 

            //destroy all child transforms of mapList, in case mapLibrary has changed.
            foreach (Transform child in mapList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            //calculate position of the top of the list
            int top = (mapEntries.Count - 1) * (BUTTON_SIZE_Y + BUTTON_SPACING_Y) / 2;

            //Generate each button
            for (int i = 0; i < mapEntries.Count; i++)
            {
                GameObject mapButton = new GameObject(mapEntries[i].Item1 + " Select Button");
                mapButton.transform.SetParent(mapList.transform);

                Debug.Log("ADDED " + mapEntries[i].Item1 + " BUTTON. ID = " + mapEntries[i].Item2);

                int id = mapEntries[i].Item2;
                mapButton.AddComponent<Button>();
                mapButton.GetComponent<Button>().onClick.AddListener(() => LoadMap(id));

                mapButton.AddComponent<RectTransform>();

                mapButton.AddComponent<Image>();
                mapButton.GetComponent<Image>().sprite = Sprite.Create((Texture2D)Resources.Load(BUTTON_FILE, typeof(Texture2D)), new Rect(0.0f, 0.0f, 32f, 32f), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(3, 3, 3, 3));
                mapButton.GetComponent<Image>().type = Image.Type.Tiled;

                RectTransform buttonTransform = mapButton.GetComponent<RectTransform>();
                buttonTransform.sizeDelta = new Vector2(BUTTON_SIZE_X, BUTTON_SIZE_Y);
                buttonTransform.localPosition = new Vector3(0, top - i * (BUTTON_SIZE_Y+BUTTON_SPACING_Y), 0);
                buttonTransform.localScale = new Vector3(1, 1, 1);

                GameObject buttonText = new GameObject(mapEntries[i].Item1 + " Button Text");
                //buttonText.transform.SetParent(buttonTransform.transform);
                buttonText.AddComponent<Text>();
                //buttonText.AddComponent<RectTransform>();
                buttonText.GetComponent<RectTransform>().SetParent(buttonTransform);
                buttonText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                buttonText.GetComponent<RectTransform>().sizeDelta = buttonTransform.sizeDelta;
                Text coreText = buttonText.GetComponent<Text>();
                //coreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                coreText.font = ((Font)Resources.Load(FONT_FILE, typeof(Font))) ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                coreText.fontSize = 25;
                coreText.text = mapEntries[i].Item1;
                coreText.color = Color.white;
                coreText.alignment = TextAnchor.MiddleCenter;
            }
            Debug.Log("DONE GENERATING MAP SELECT BUTTONS");
        }
    }
}