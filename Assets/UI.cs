using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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

        Game game;

        public UI(Game game)
        {
            this.game = game;
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
            retTransform.localScale = new Vector3(1, 1, 1);

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
            retTransform.sizeDelta = retTransform.sizeDelta;

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
            coreText.fontSize = fontSize;
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
        /// <returns>Tuple containing the button GameObject in Item1 and the text GameObject in Item2.</returns>
        public static Tuple<GameObject,GameObject> generateUITextButton(string name, string spriteFile, Vector2 spriteCoords, Vector4 border, Rect rect, Transform parent, UnityEngine.Events.UnityAction clickEvent, string text, int fontSize, Color color)
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

            return Tuple.Create<GameObject, GameObject>(ret, retText);
        }
    }
}
