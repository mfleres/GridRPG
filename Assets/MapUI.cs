using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace GridRPG
{
    /// <summary>
    /// The game interface
    /// </summary>
    public class MapUI
    {
        private const string FRAME_FILE_BLUE = "Sprites/GUI/BlueBox";
        private const float UNITFRAME_SPACING = 6.0f;
        private const float UNITFRAME_WIDTH = 200f;
        private const float UNITFRAME_HEIGHT = 200f;
        private const float FRAME_SPRITE_SIZE = 32f;

        public GridRPG.MapLibrary mapLibrary;
        public GridRPG.UnitLibrary unitLibrary;

        // Master Canvas to display the elements on.
        private GameObject canvas = null;
        private GameObject unitFrame = null;

        public MapUI(MapLibrary mapLibrary, UnitLibrary unitLibrary)
        {
            canvas = new GameObject("Map UI");
            this.mapLibrary = mapLibrary;
            this.unitLibrary = unitLibrary;

            canvas.AddComponent<Canvas>();
            Canvas coreCanvas = canvas.GetComponent<Canvas>();
            coreCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<GraphicRaycaster>();
            coreCanvas.pixelPerfect = true;

            RectTransform canvasTransform = coreCanvas.GetComponent<RectTransform>();
            Rect unitFrameDimensions = new Rect(-canvasTransform.sizeDelta.x/2.0f + UNITFRAME_SPACING+UNITFRAME_WIDTH/2.0f, canvasTransform.sizeDelta.y/2.0f - UNITFRAME_SPACING - UNITFRAME_HEIGHT/2.0f, UNITFRAME_WIDTH, UNITFRAME_HEIGHT);
            unitFrame = generateUIFrame("Unit Frame", FRAME_FILE_BLUE, new Vector2(0, 0), new Vector4(3, 3, 3, 3), unitFrameDimensions, coreCanvas.transform);

        }

        /// <summary>
        /// Set visibility of the UI
        /// </summary>
        /// <param name="value"></param>
        public void setActive(bool value)
        {
            canvas.SetActive(value);
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
    }
}
