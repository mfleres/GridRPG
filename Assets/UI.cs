using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GridRPG
{
    public class UI
    {
        private const string FRAME_FILE_BLUE = "Sprites/GUI/BlueBox";
        private const string FONT_FILE = "Fonts/PressStart2P";
        private const float FRAME_SPRITE_SIZE = 32f;
        private const float UI_SCALE = 1; //Changing this value causes ui to overlap and not display properly.
       
        //Main menu button parameters
        private const int BUTTON_SIZE_X = 200;
        private const int BUTTON_SIZE_Y = 50;
        private const int BUTTON_SPACING_Y = 3; //vertical space between each button
        private const int BUTTON_FONT_SIZE = 24;

        //Unit frame parameters.
        //  Displays at top left of the screen.
        //  Will not extend below the top of the message frame.
        private const float UNITFRAME_SPACING = 6.0f;
        private const float UNITFRAME_WIDTH = 400f;
        private const float UNITFRAME_MAX_WIDTH = 0.25f; //% of screen width
        private const float UNITFRAME_HEIGHT = 600f;
        private const int UNITFRAME_NAME_FONT_SIZE = 16;

        //Message Frame parameters.
        //  Displays at the bottom center of the screen.
        private const float MESSAGE_FRAME_SPACING = 6.0f;
        private const float MESSAGE_FRAME_WIDTH = 600f;
        private const float MESSAGE_FRAME_HEIGHT = 150f;
        private const float MESSAGE_FRAME_MAX_HEIGHT = 0.16f; //% of screen height
        //(Message frame displays at the bottom center of the screen.)


        public enum Modes { Main, MapList, ActiveMap };

        private Game game;

        private Modes mode;
        private GameObject canvas;

        //Main Menu
        private GameObject mapSelectButton;

        //Map List
        private GameObject mapList;

        //Active Map
        //Organize unit frame elements
        private struct UnitFrameStruct
        {
            public GameObject frame;
            public GameObject unitName;
        }
        private UnitFrameStruct unitFrame;
        //Message frame
        private GameObject messageFrame;

        public UI(Game game)
        {
            this.game = game;

            //Setup main canvas
            this.canvas = new GameObject("UI Canvas");
            this.canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();
            coreCanvas.pixelPerfect = true;

            //Setup main menu
            mapSelectButton = UI.generateUITextButton("Map Select Button", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), new Rect(0, 0, BUTTON_SIZE_X, BUTTON_SIZE_Y), canvas.transform, (delegate { Mode = Modes.MapList; }), "Select Map", BUTTON_FONT_SIZE, Color.white);
            

            //Setup mapList controller
            mapList = new GameObject("Map List");
            mapList.transform.SetParent(canvas.transform);
            mapList.transform.localPosition = new Vector3(0, 0, 0);
            

            //Setup map ui
            setupMessageFrame();
            setupUnitFrame();

            this.Mode = Modes.Main;
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
                        
                        if (mode == Modes.ActiveMap)
                        {
                            game.mapLibrary.unloadMap();
                        }

                        setMainMenuVisibility(true);
                        setMapListVisibility(false);
                        setMapUIVisibility(false);

                        mode = value;
                        break;

                    case Modes.MapList:
                        
                        if (mode == Modes.ActiveMap)
                        {
                            game.mapLibrary.unloadMap();
                        }

                        setMainMenuVisibility(false);
                        setMapListVisibility(true);
                        setMapUIVisibility(false);

                        mode = value;
                        break;

                    case Modes.ActiveMap:

                        setMainMenuVisibility(false);
                        setMapListVisibility(false);
                        setMapUIVisibility(true);

                        updateUnitFrame(game.unitLibrary.getUnit(1, "campaign")); //TEMP

                        mode = value;
                        break;
                }
            }
        }

        public void updateUnitFrame(Unit unit)
        {
            Text nameText = unitFrame.unitName.GetComponent<Text>();
            nameText.text = unit.name;
            trimText(unitFrame.unitName);

            //TODO: Finish
        }

        /// <summary>
        /// !!!!Destroys the UI gameobjects!!!!
        /// </summary>
        public void destroy()
        {
            GameObject.Destroy(canvas);
        }

        private void setMapUIVisibility(bool active)
        {
            unitFrame.frame.SetActive(active);
            messageFrame.SetActive(active);
        }

        private void setMainMenuVisibility(bool active)
        {
            mapSelectButton.SetActive(active);
        }

        private void setMapListVisibility(bool active)
        {
            if(mapList.activeSelf && !active)
            {   //Hide mapList
                mapList.SetActive(false);
            }
            else if(!(mapList.activeSelf) && active)
            {   //Show maplist
                mapList.SetActive(true);
                generateMapList();
            }
            else
            {
                //No change
            }
        }

        //Loads a the map with index id.
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
            //Debug.Log("GENERATING MAP SELECT BUTTONS");
            List<Tuple<string, int>> mapEntries = game.mapLibrary.listMaps();

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

                //Debug.Log("ADDED " + mapEntries[i].Item1 + " BUTTON. ID = " + mapEntries[i].Item2);
            }
            //Debug.Log("DONE GENERATING MAP SELECT BUTTONS");
        }

        /// <summary>
        /// Setup the message frame within the screen boundaries
        /// </summary>
        private void setupMessageFrame()
        {
            float frame_width, frame_height, frame_pos_x, frame_pos_y;

            //Setup frame width.
            if(MESSAGE_FRAME_WIDTH + MESSAGE_FRAME_SPACING * 2f > Screen.width)
            {   
                frame_width = Screen.width - MESSAGE_FRAME_SPACING * 2f;
            }
            else
            {
                frame_width = MESSAGE_FRAME_WIDTH;
            }

            //Setup frame height.
            if(MESSAGE_FRAME_HEIGHT > Screen.height*MESSAGE_FRAME_MAX_HEIGHT)
            {
                frame_height = Screen.height * MESSAGE_FRAME_MAX_HEIGHT;
            }
            else
            {
                frame_height = MESSAGE_FRAME_HEIGHT;
            }

            //Setup frame positioning.
            frame_pos_x = 0f;
            frame_pos_y = -Screen.height / 2.0f + MESSAGE_FRAME_SPACING + frame_height / 2.0f;

            //Generate frame.
            Rect messageFrameDimensions = new Rect(frame_pos_x, frame_pos_y, frame_width, frame_height);
            messageFrame = UI.generateUIFrame("Message Frame", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), messageFrameDimensions, canvas.transform);
        }

        /// <summary>
        /// Setup the unit frame within the screen boundaries.
        /// If the message frame is already generated, will adjust to fit.
        /// </summary>
        private void setupUnitFrame()
        {
            float frame_width, frame_height, frame_pos_x, frame_pos_y;
            Vector2 messageFrameSize;

            if (messageFrame)
            {   //use message frame dimensions.
                RectTransform messageFrameRect = messageFrame.GetComponent<RectTransform>();
                messageFrameSize = messageFrameRect.sizeDelta;
            }
            else
            {   //assume message frame is 0.
                messageFrameSize = new Vector2(0, 0);
            }

            //Setup frame width.
            if (UNITFRAME_WIDTH > Screen.width * UNITFRAME_MAX_WIDTH)
            {
                frame_width = Screen.width * UNITFRAME_MAX_WIDTH;
            }
            else
            {
                frame_width = UNITFRAME_WIDTH;
            }

            //Setup frame height.
            if (UNITFRAME_HEIGHT > Screen.height - messageFrameSize.y - MESSAGE_FRAME_SPACING - UNITFRAME_SPACING*2)
            {
                frame_height = Screen.height - messageFrameSize.y - MESSAGE_FRAME_SPACING - UNITFRAME_SPACING * 2;
            }
            else
            {
                frame_height = UNITFRAME_HEIGHT;
            }

            //Setup frame positioning.
            frame_pos_x = -Screen.width / 2.0f + UNITFRAME_SPACING + frame_width / 2.0f;
            frame_pos_y = Screen.height / 2.0f - UNITFRAME_SPACING - frame_height / 2.0f;

            //Generate frame.
            Rect unitFrameDimensions = new Rect(frame_pos_x, frame_pos_y, frame_width, frame_height);
            unitFrame.frame = UI.generateUIFrame("Unit Frame", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), unitFrameDimensions, canvas.transform);

            //Add Unit Name Text frame.
            float unitFrameUnitNameWidth = frame_width;
            float unitFrameUnitNameHeight = UNITFRAME_NAME_FONT_SIZE;
            float unitFrameUnitNamePosX = 0;
            float unitFrameUnitNamePosY = frame_height / 2.0f - UNITFRAME_SPACING - unitFrameUnitNameHeight / 2.0f;
            Rect unitFrameUnitNameDims = new Rect(unitFrameUnitNamePosX, unitFrameUnitNamePosY, unitFrameUnitNameWidth, unitFrameUnitNameHeight);

            unitFrame.unitName = UI.generateUIText("UnitName", unitFrame.frame.transform, "", UNITFRAME_NAME_FONT_SIZE, Color.white, unitFrameUnitNameDims);
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
                Debug.Log(name + " Creation Error: Failed to load sprite.");
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
            //.
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
            coreText.alignment = anchor;

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
        /// Generates UI text
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="parent">Parent transform.</param>
        /// <param name="text">Text.</param>
        /// <param name="fontSize">Font size.</param>
        /// <param name="color">Text color.</param>
        /// <param name="rect">Text position relative to parent.</param>
        /// <returns>Text as a GameObject.</returns>
        public static GameObject generateUIText(string name, Transform parent, string text, int fontSize, Color color, Rect rect)
        {
            GameObject ret = generateUIText(name, parent, text, fontSize, color, TextAnchor.MiddleCenter);
            RectTransform retRectTransform = ret.GetComponent<RectTransform>();
            RectTransform parentRectTransform = ret.GetComponentInParent<RectTransform>();

            retRectTransform.localPosition = new Vector3(rect.x, rect.y, 0);
            retRectTransform.sizeDelta = rect.size;

            return ret;
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

        private static void trimText(GameObject textObject)
        {
            Text textComponent = textObject.GetComponent<Text>();
            float textWidth = LayoutUtility.GetPreferredWidth(textComponent.rectTransform);
            float parentWidth = textObject.GetComponent<RectTransform>().rect.width;

            if (textWidth > parentWidth)
            {
                //Resize to one word
                string text = textComponent.text;

                string[] splitText = text.Split(null);
                textComponent.text = splitText[0];
            }
        }
    }
}
