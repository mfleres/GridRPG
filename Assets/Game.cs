using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace GridRPG
{
    /// <summary>
    /// Holds game data and modes
    /// </summary>
    public class Game : MonoBehaviour
    {
        private const string MAP_LIST_FILE = "MapList";
        private const string CAMPAIGN_UNIT_LIST_FILE = "CampaignUnitList";
        private const string UNIT_LIST_FILE = "UnitList";
        private const string SKILL_LIST_FILE = "SkillList";
        public enum Modes { MainMenu, Map };

        public static GridRPG.UnitLibrary unitLibrary;
        public static GridRPG.MapLibrary mapLibrary;
        public static GridRPG.SkillLibrary skillLibrary;
        public static GameObject ui;
        //public static GridRPG.UI ui;
        public static GameObject map;

        public static bool animationInProgress;
        private static float waitEndTime = 0f;
        private static bool waitFlag = false;

        /// <summary>
        /// Supports the following resolutions:
        /// 853x480(wide 480p),
        /// </summary>
        public static Vector2 resolution { get; private set; }

        private static Modes mode;

        // Use this for initialization
        void Start()
        {
            //Set resolution to wide 480p-windowed
            Screen.SetResolution(853, 480, false);

            resolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            skillLibrary = new GridRPG.SkillLibrary(SKILL_LIST_FILE);
            unitLibrary = new GridRPG.UnitLibrary(this);
            unitLibrary.loadCampaignUnitList(CAMPAIGN_UNIT_LIST_FILE);
            unitLibrary.loadUnitList(UNIT_LIST_FILE);
            mapLibrary = new GridRPG.MapLibrary(this);
            mapLibrary.loadMapList(MAP_LIST_FILE);

            ui = new GameObject("UI Canvas");
            ui.AddComponent<GridRPG.UI>();
            animationInProgress = false;

            float height = Camera.main.orthographicSize * 2.0f;
            float width = height * Screen.width / Screen.height;
            Debug.Log(width + "," + height);
        }

        // Update is called once per frame
        void Update()
        {
            if (waitFlag && Time.fixedTime > waitEndTime)
            {
                waitFlag = false;
                Game.animationInProgress = false;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GridRPG.Game.ui.GetComponent<UI>().Mode = GridRPG.UI.Modes.Main;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Acts as if an animation is in progress for a duration.
        /// </summary>
        /// <param name="seconds">Duration to wait.</param>
        /// <returns>if the wait was successfully initialized.</returns>
        public static bool waitDuration(float seconds)
        {
            if (!Game.animationInProgress)
            {
                waitEndTime = Time.fixedTime + seconds;
                waitFlag = true;
                animationInProgress = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Reloads the UI.
        /// </summary>
        /// <remarks>
        /// Useful for when the user changes UI options.
        /// </remarks>
        private static void reloadUI()
        {
            //Todo
        }

        /// <summary>
        /// Updates the resolution and reloads the UI.
        /// </summary>
        /// <param name="resolution">A string in the format "WIDTHxHEIGHT".</param>
        /// <seealso cref="Game.resolution"/>
        public void updateResolution(string resolution)
        {
            switch(resolution)
            {
                case "853x480":
                case "wide 480p":
                    Game.resolution = new Vector2(853f,480f);
                    reloadUI();
                    break;
            }
        }
    }
}
