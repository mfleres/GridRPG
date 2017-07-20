using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace GridRPG
{
    public class MainMenu
    {
        const float originX = 0;
        const float originY = 0;

        public GridRPG.MapLibrary mapLibrary;
        public GridRPG.UnitLibrary unitLibrary;

        public GameObject canvas;
        public GameObject gameStartButton;

        public MainMenu(MapLibrary mapLibrary,UnitLibrary unitLibrary)
        {
            canvas = new GameObject("Main Menu UI");
            this.mapLibrary = mapLibrary;
            this.unitLibrary = unitLibrary;
            //Debug.Log("Capacity* = " + unitLibrary.campaignUnits.Count);

            canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();

            gameStartButton = new GameObject("Game Start Button");
            gameStartButton.transform.parent = canvas.transform;
            gameStartButton.AddComponent<Button>();
            gameStartButton.GetComponent<Button>().onClick.AddListener(delegate { LoadMapAG(); });
            
            gameStartButton.AddComponent<RectTransform>();
            gameStartButton.AddComponent<Image>();
            gameStartButton.GetComponent<Image>().sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/Terrain/Water", typeof(Texture2D)), new Rect(0.0f, 0.0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
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

        void LoadMapAG()
        {
            Debug.Log("Clicked");
            mapLibrary.map = new GridRPG.Map("Assets/Resources/Maps/MapA.xml", unitLibrary);
            mapLibrary.map.centerMapOnCamera(Camera.main);
            canvas.SetActive(false);
        }
    }
}