using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GridRPG
{
    public class UI
    {
        private const string FRAME_FILE_BLUE = "Sprites/GUI/BlueBox";
        private const string WHITE_TEX = "Sprites/GUI/White";
        private const string FONT_FILE = "Fonts/PressStart2P";
        private const float FRAME_SPRITE_SIZE = 32f;
        private const float UI_SCALE = 1; //Changing this value causes ui to overlap and not display properly.
       
        //Main menu button parameters for wide 480p
        private const int BUTTON_SIZE_X = 200;
        private const int BUTTON_SIZE_Y = 50;
        private const int BUTTON_SPACING_Y = 3; //vertical space between each button
        private const int BUTTON_FONT_SIZE = 24;

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
        private struct MainMenuStruct
        {
            public GameObject mapSelectButton;
        }
        MainMenuStruct mainMenu;

        //Map List
        private GameObject mapList;

        //Active Map
        //Organize unit frame elements
        private struct UnitFrameStruct
        {
            public GameObject frame;
            public GameObject unitName;
            public GameObject line1;
            public GameObject hpTitle;
            public GameObject hpText;
        }
        private UnitFrameStruct unitFrame;
        //Message frame
        private GameObject messageFrame;

        public UI(Game game)
        {
            this.game = game;

            //Game.selectEvent += updateUnitFrame;

            //Setup main canvas
            this.canvas = new GameObject("UI Canvas");
            this.canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();
            coreCanvas.pixelPerfect = true;

            //Setup main menu
            setupMainMenu();

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
            //Update Name
            Text nameText = unitFrame.unitName.GetComponent<Text>();
            nameText.text = unit.name;
            trimText(unitFrame.unitName);

            //Update HP
            Text hpText = unitFrame.hpText.GetComponent<Text>();
            hpText.text = unit.getHP().ToString() + " / " + unit.getMaxHP().ToString();

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
            mainMenu.mapSelectButton.SetActive(active);
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
        /// Setup the main menu.
        /// </summary>
        private void setupMainMenu()
        {
            float button_size_x, button_size_y;
            int button_font_size;

            //Set resolution parameters
            if (true || game.resolution.x == 853f && game.resolution.y == 480f)
            {   //wide 480p or unsupported
                if (game.resolution.x != 853f || game.resolution.y != 480f)
                {   //unsupported warning
                    Debug.Log("Unit Frame: Unsupported resolution " + game.resolution.x + "x" + game.resolution.y);
                }

                button_font_size = 24;
                button_size_x = 300f;
                button_size_y = 50f;
            }

            mainMenu.mapSelectButton = UI.generateUITextButton("Map Select Button", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), new Rect(0, 0, button_size_x, button_size_y), canvas.transform, (delegate { Mode = Modes.MapList; }), "Select Map", button_font_size, Color.white);
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
            float line_length, line_pos_y;
            float unitFrameUnitNameWidth, unitFrameUnitNameSize, unitFrameUnitNamePosX, unitFrameUnitNamePosY;
            float hpTitle_posX, hpTitle_posY, hpTitle_size, hpTitle_width;
            float hp_pos_x, hp_pos_y, hp_width, hp_size;

            //Set resolution parameters
            if(true || game.resolution.x==853f && game.resolution.y == 480f)
            {   //wide 480p or unsupported
                if (game.resolution.x != 853f || game.resolution.y != 480f)
                {   //unsupported warning
                    Debug.Log("Unit Frame: Unsupported resolution " + game.resolution.x + "x" + game.resolution.y);
                }

                frame_width = 210f;
                frame_height = 360f;
                frame_pos_x = -300f;
                frame_pos_y = 48f;
                
                unitFrameUnitNameWidth = frame_width;
                unitFrameUnitNameSize = 16f;
                unitFrameUnitNamePosX = 0;
                unitFrameUnitNamePosY = 164f;

                line_length = 200f;
                line_pos_y = unitFrameUnitNamePosY - 16;

                hpTitle_posX = -40;
                hpTitle_posY = line_pos_y - 15;
                hpTitle_size = 8;
                hpTitle_width = frame_width / 2f;

                hp_pos_x = 25;
                hp_pos_y = hpTitle_posY;
                hp_width = frame_width / 2f;
                hp_size = 8f;
            }

            //Generate frame.
            Rect unitFrameDimensions = new Rect(frame_pos_x, frame_pos_y, frame_width, frame_height);
            unitFrame.frame = UI.generateUIFrame("Unit Frame", FRAME_FILE_BLUE, new Vector2(0f, 0f), new Vector4(3f, 3f, 3f, 3f), unitFrameDimensions, canvas.transform);

            //Add Unit Name Text frame.
            Rect unitFrameUnitNameDims = new Rect(unitFrameUnitNamePosX, unitFrameUnitNamePosY, unitFrameUnitNameWidth, unitFrameUnitNameSize);
            unitFrame.unitName = UI.generateUIText("UnitName", unitFrame.frame.transform, "", (int)unitFrameUnitNameSize, Color.white, unitFrameUnitNameDims);

            //Line underneath unit name
            Vector2 linePosition = new Vector2(0, line_pos_y);
            unitFrame.line1 = generateUILine("Line 1", WHITE_TEX, new Vector2(0, 0), new Vector2(2, 1), linePosition, line_length, true, unitFrame.frame.transform);

            //HP Title
            Rect hpTitleRect = new Rect(hpTitle_posX, hpTitle_posY, hpTitle_width, hpTitle_size);
            unitFrame.hpTitle = generateUIText("HP Title", unitFrame.frame.transform, "HP:", (int)hpTitle_size, Color.white, hpTitleRect);

            //HP Numbers
            Rect hpRect = new Rect(hp_pos_x, hp_pos_y, hp_width, hp_size);
            unitFrame.hpText = generateUIText("HP Values", unitFrame.frame.transform, "/", (int)hp_size, Color.yellow, hpRect);
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

        /// <summary>
        /// Generates a line
        /// </summary>
        /// <param name="name">Name of the line.</param>
        /// <param name="spriteFile">File to load sprite from.</param>
        /// <param name="spriteCoords">Coordinates of the sprite in the file.</param>
        /// <param name="border">Width of the 2 sides of the line.</param>
        /// <param name="position">Position of the line.</param>
        /// <param name="length">length of the line</param>
        /// <param name="direction">Direction of the line. (Horizontal=true, Vertical=false)</param>
        /// <param name="parent">Parent UI frame.</param>
        /// <returns></returns>
        public static GameObject generateUILine(string name, string spriteFile, Vector2 spriteCoords, Vector2 border, Vector2 position, float length, bool direction, Transform parent)
        {
            Vector4 lineBorder;
            Rect lineRect;

            if (direction)
            {   //Horizontal
                lineBorder = new Vector4(0, border.x, 0, border.y);
                lineRect = new Rect(position, new Vector2(length,border.x + border.y));
            }
            else
            {   //Vertical
                lineBorder = new Vector4(border.x, 0, border.y, 0);
                lineRect = new Rect(position, new Vector2(border.x + border.y, length));
            }

            return generateUIFrame(name, spriteFile, spriteCoords, lineBorder, lineRect, parent);
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
