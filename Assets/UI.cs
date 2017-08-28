using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GridRPG
{
    public class UI
    {
        private const string FRAME_FILE_BLUE = "Sprites/GUI/BlueBox";
        private const string FONT_FILE = "Fonts/VCR_OSD_MONO_1.001";
        private const float UNITFRAME_SPACING = 6.0f;
        private const float UNITFRAME_WIDTH = 200f;
        private const float UNITFRAME_HEIGHT = 200f;
        private const float FRAME_SPRITE_SIZE = 32f;
        private const int BUTTON_SIZE_X = 200;
        private const int BUTTON_SIZE_Y = 50;
        private const int BUTTON_SPACING_Y = 3; //vertical space between each button
        private const int BUTTON_FONT_SIZE = 25;
        private const float UI_SCALE = 1;

        public enum Modes { Main, MapList, ActiveMap };

        private Game game;

        private Modes mode;
        private GameObject canvas;

        //Main Menu
        private GameObject mapSelectButton;

        //Map List
        private GameObject mapList;

        //Active Map
        private GameObject unitFrame;
        private GameObject messageFrame;

        public UI(Game game)
        {
            this.game = game;

            this.canvas = new GameObject("UI Canvas");
            this.canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();
            coreCanvas.pixelPerfect = true;

            mapSelectButton = UI.generateUITextButton("Map Select Button", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), new Rect(0, 0, BUTTON_SIZE_X, BUTTON_SIZE_Y), canvas.transform, (delegate { Mode = Modes.MapList; }), "Select Map", BUTTON_FONT_SIZE, Color.white);
            mapSelectButton.SetActive(true);

            //TODO: ADD REAL MAP UI SETUP
            unitFrame = new GameObject("UI Unit Frame");
            messageFrame = new GameObject("UI Message Frame");
            setMapUIVisibility(false);

            mapList = new GameObject("Map List");
            mapList.transform.SetParent(canvas.transform);
            mapList.transform.localPosition = new Vector3(0, 0, 0);
            mapList.SetActive(false);
        }

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
                        setMapUIVisibility(false);
                        if (mode == Modes.ActiveMap)
                        {
                            game.mapLibrary.unloadMap();
                        }

                        canvas.SetActive(true);
                        mapSelectButton.SetActive(true);
                        mapList.SetActive(false);
                        mode = value;
                        break;
                    case Modes.MapList:
                        setMapUIVisibility(false);
                        if (mode == Modes.ActiveMap)
                        {
                            game.mapLibrary.unloadMap();
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
                        setMapUIVisibility(true);
                        mode = value;
                        break;
                }
            }
        }

        private void setMapUIVisibility(bool active)
        {
            unitFrame.SetActive(active);
            messageFrame.SetActive(active);
        }

        //Loads a the map with index id
        private void LoadMap(int id)
        {
            game.mapLibrary.loadMap(id);
            this.Mode = Modes.ActiveMap;
        }

        /// <summary>
        /// Generates a list of buttons for every map.
        /// </summary>
        private void generateMapList()
        {
            Debug.Log("GENERATING MAP SELECT BUTTONS");
            List<Tuple<string, int>> mapEntries = game.mapLibrary.listMaps();
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
                int id = mapEntries[i].Item2;
                GameObject mapButton = generateUITextButton(mapEntries[i].Item1 + " Select Button", FRAME_FILE_BLUE, new Vector2(0, 0)
                                                            , new Vector4(3, 3, 3, 3), new Rect(0, top - i * (BUTTON_SIZE_Y + BUTTON_SPACING_Y), BUTTON_SIZE_X, BUTTON_SIZE_Y)
                                                            , mapList.transform, () => LoadMap(id), mapEntries[i].Item1, (int)(BUTTON_FONT_SIZE * UI_SCALE), Color.white);
                mapButton.transform.SetParent(mapList.transform);

                Debug.Log("ADDED " + mapEntries[i].Item1 + " BUTTON. ID = " + mapEntries[i].Item2);
            }
            Debug.Log("DONE GENERATING MAP SELECT BUTTONS");
        }

        /// <summary>
        /// Generates a UI frame.
        /// </summary>
        /// <param name="name">Name of the frame.</param>
        /// <param name="spriteFile">File to load the sprite from.</param>
        /// <param name="spriteCoords">Coordinates of the sprite in the file.</param>
        /// <param name="border">Width of the frame borders.</param>
        /// <param name="rect">Position and size of the frame relative to the parent (Pixels from bottom left).</param>
        /// <param name="parent">Parent object.</param>
        /// <returns>The frame as a GameObject</returns>
        public static GameObject generateUIFrame(string name, string spriteFile, Vector2 spriteCoords, Vector4 border, Rect rect, Transform parent)
        {
            //Initialize the GameObject
            GameObject ret = new GameObject(name);
            ret.transform.SetParent(parent);

            //Generate Image component
            ret.AddComponent<Image>();
            Image retImage = ret.GetComponent<Image>();
            Rect retSpriteCoords = new Rect(spriteCoords.x, spriteCoords.y, FRAME_SPRITE_SIZE, FRAME_SPRITE_SIZE);
            try
            {
                retImage.sprite = Sprite.Create((Texture2D)Resources.Load(spriteFile, typeof(Texture2D)), retSpriteCoords, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
            }
            catch (System.Exception e)
            {
                GameObject.Destroy(ret);
                throw e;
            }
            retImage.type = Image.Type.Tiled;

            //Generate RectTransform component
            RectTransform retTransform = ret.GetComponent<RectTransform>();
            retTransform.sizeDelta = rect.size;
            retTransform.localPosition = new Vector3(rect.x, rect.y, 0);
            retTransform.localScale = new Vector3(UI_SCALE, UI_SCALE, UI_SCALE);

            return ret;
        }

        /// <summary>
        /// Generates a UI button.
        /// </summary>
        /// <param name="name">Name of the button.</param>
        /// <param name="spriteFile">File to load the sprite from.</param>
        /// <param name="spriteCoords">Coordinates of the sprite in the file.</param>
        /// <param name="border">Width of the frame borders.</param>
        /// <param name="rect">Position and size of the frame relative to the parent (Pixels from bottom left).</param>
        /// <param name="parent">Parent transform.</param>
        /// <param name="clickEvent">Click event.</param>
        /// <returns>The frame as a GameObject</returns>
        public static GameObject generateUIButton(string name, string spriteFile, Vector2 spriteCoords, Vector4 border, Rect rect, Transform parent, UnityEngine.Events.UnityAction clickEvent)
        {
            GameObject ret = generateUIFrame(name, spriteFile, spriteCoords, border, rect, parent);
            ret.AddComponent<Button>();
            ret.GetComponent<Button>().onClick.AddListener(clickEvent);

            return ret;
        }

        /// <summary>
        /// Generates UI text
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="parent">Parent transform.</param>
        /// <param name="text">Text.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="color">Text color.</param>
        /// <param name="anchor">Text alignment.</param>
        /// <returns>Text as a GameObject.</returns>
        public static GameObject generateUIText(string name, Transform parent, string text, int fontSize, Color color, TextAnchor anchor)
        {
            GameObject ret = new GameObject(name);
            ret.AddComponent<Text>();

            RectTransform retTransform = ret.GetComponent<RectTransform>();
            retTransform.SetParent(parent);
            retTransform.localPosition = new Vector3(0, 0, 0);
            retTransform.sizeDelta = ((RectTransform)parent).sizeDelta;

            Text coreText = ret.GetComponent<Text>();
            try
            {
                coreText.font = ((Font)Resources.Load(FONT_FILE, typeof(Font))) ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            catch(System.Exception e)
            {
                GameObject.Destroy(ret);
                throw e;
            }
            coreText.fontSize = (int)(fontSize*UI_SCALE);
            coreText.text = text;
            coreText.color = color;
            coreText.alignment = TextAnchor.MiddleCenter;

            return ret;
        }

        /// <summary>
        /// Generates UI text
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="text">Text.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="color">Text color.</param>
        /// <param name="anchor">Text alignment.</param>
        /// <returns>Text as a GameObject.</returns>
        public static GameObject generateUIText(Transform parent, string text, int fontSize, Color color, TextAnchor anchor)
        {
            return generateUIText("Text", parent, text, fontSize, color, anchor);
        }

        /// <summary>
        /// Generates a button with text.
        /// </summary>
        /// <param name="name">Name of the button GameObject</param>
        /// <param name="spriteFile">File to load the sprite from.</param>
        /// <param name="spriteCoords">Coordinates of the sprite in the file.</param>
        /// <param name="border">Width of the frame borders.</param>
        /// <param name="rect">Position and size of the frame relative to the parent (Pixels from bottom left).</param>
        /// <param name="parent">Parent transform.</param>
        /// <param name="clickEvent">Click event.</param>
        /// <param name="text">Text.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="color">Text color.</param>
        /// <returns>Button GameObject. Does not include the text GameObject.</returns>
        public static GameObject generateUITextButton(string name, string spriteFile, Vector2 spriteCoords, Vector4 border, Rect rect, Transform parent, UnityEngine.Events.UnityAction clickEvent, string text, int fontSize, Color color)
        {
            GameObject ret = generateUIButton(name, spriteFile, spriteCoords, border, rect, parent, clickEvent);

            GameObject retText;
            try
            {
                retText = generateUIText(ret.transform, text, fontSize, color, TextAnchor.MiddleCenter);
            }
            catch(System.Exception e)
            {
                GameObject.Destroy(ret);
                throw e;
            }

            return ret;
        }
    }
}
