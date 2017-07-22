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
            coreCanvas.pixelPerfect = true;

            gameStartButton = new GameObject("Game Start Button");
            gameStartButton.transform.parent = canvas.transform;
            gameStartButton.AddComponent<Button>();
            gameStartButton.GetComponent<Button>().onClick.AddListener(delegate { LoadMapAG(); });
            
            gameStartButton.AddComponent<RectTransform>();
            gameStartButton.AddComponent<Image>();
            gameStartButton.GetComponent<Image>().sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/GUI/BlueBox", typeof(Texture2D)), new Rect(0.0f, 0.0f, 32f, 32f), new Vector2(0.0f, 0.0f),100f,0,SpriteMeshType.FullRect,new Vector4(3,3,3,3));
            gameStartButton.GetComponent<Image>().type = Image.Type.Tiled;
            //gameStartButton.GetComponent<Image>().material.mainTexture.filterMode = FilterMode.Point;
            //gameStartButton.GetComponent<Image>().sprite.
            //gameStartButton.AddComponent<SpriteRenderer>();
            //gameStartButton.GetComponent<SpriteRenderer>().sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/GUI/BlueBox", typeof(Texture2D)), new Rect(0.0f, 0.0f, Terrain.terrain_dim, Terrain.terrain_dim), new Vector2(0.5f, 0.5f));
            //gameStartButton.GetComponent<Button>().targetGraphic = Gra
            RectTransform buttonTransform = gameStartButton.GetComponent<RectTransform>();
            buttonTransform.sizeDelta = new Vector2(200, 50);
            buttonTransform.localPosition = new Vector3(0, 0, 0);
            buttonTransform.localScale = new Vector3(1, 1, 1);

            GameObject buttonText = new GameObject("Text");
            buttonText.transform.parent = gameStartButton.transform;
            buttonText.AddComponent<Text>();
            buttonText.AddComponent<RectTransform>();
            buttonText.GetComponent<RectTransform>().SetParent(buttonTransform);
            buttonText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            buttonText.GetComponent<RectTransform>().sizeDelta = buttonTransform.sizeDelta;
            Text coreText = buttonText.GetComponent<Text>();
            coreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            coreText.font = (Font)Resources.Load("Fonts/VCR_OSD_MONO_1.001", typeof(Font));
            coreText.fontSize = 25;
            coreText.text = "Load MapA";
            coreText.color = Color.white;
            coreText.alignment = TextAnchor.MiddleCenter;

        }

        void LoadMapAG()
        {
            Debug.Log("Clicked");
            mapLibrary.loadMap(0);
            //mapLibrary.map.centerMapOnCamera(Camera.main);
            canvas.SetActive(false);
        }
    }
}