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

            unitFrame = new GameObject("Unit Frame");
            unitFrame.transform.SetParent(coreCanvas.transform);
            unitFrame.AddComponent<Image>();
            Image unitFrameImage = unitFrame.GetComponent<Image>();
            RectTransform unitFrameTransform = unitFrame.GetComponent<RectTransform>();

            //Load sprite
            unitFrameImage.sprite = Sprite.Create((Texture2D)Resources.Load(FRAME_FILE_BLUE, typeof(Texture2D)), new Rect(0.0f, 0.0f, 32f, 32f), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(3, 3, 3, 3));
            unitFrameImage.type = Image.Type.Tiled;
            //Set frame size
            unitFrameTransform.sizeDelta = new Vector2(UNITFRAME_WIDTH, Screen.height * 7.0f/8.0f - UNITFRAME_SPACING);
            unitFrameTransform.localPosition = new Vector3(-Screen.width/2.0f + unitFrameTransform.sizeDelta.x/2.0f + UNITFRAME_SPACING, Screen.height / 2.0f - unitFrameTransform.sizeDelta.y / 2.0f - UNITFRAME_SPACING, 0);
            unitFrameTransform.localScale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Set visibility of the UI
        /// </summary>
        /// <param name="value"></param>
        public void setActive(bool value)
        {
            canvas.SetActive(value);
        }
    }
}
