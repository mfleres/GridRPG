using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace GridRPG
{
    /// <summary>
    /// Holds game data and modes
    /// </summary>
    public class Game
    {
        private const string MAP_LIST_FILE = "MapList";
        private const string CAMPAIGN_UNIT_LIST_FILE = "CampaignUnitList";
        private const string UNIT_LIST_FILE = "UnitList";
        private const string SKILL_LIST_FILE = "SkillList";
        public enum Modes { MainMenu, Map };

        public static GridRPG.UnitLibrary unitLibrary;
        public static GridRPG.MapLibrary mapLibrary;
        public static GridRPG.SkillLibrary skillLibrary;
        public static GridRPG.UI ui;
        public static GameObject map;

        public static bool animationInProgress;

        /// <summary>
        /// Supports the following resolutions:
        /// 853x480(wide 480p),
        /// </summary>
        public static Vector2 resolution { get; private set; }

        private static Modes mode;

        public Game()
        {
            resolution= new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
            skillLibrary = new GridRPG.SkillLibrary(SKILL_LIST_FILE);
            unitLibrary = new GridRPG.UnitLibrary(this);
            unitLibrary.loadCampaignUnitList(CAMPAIGN_UNIT_LIST_FILE);
            unitLibrary.loadUnitList(UNIT_LIST_FILE);
            mapLibrary = new GridRPG.MapLibrary(this);
            mapLibrary.loadMapList(MAP_LIST_FILE);

            ui = new UI();
            animationInProgress = false;
        }

        /// <summary>
        /// Reloads the UI.
        /// </summary>
        /// <remarks>
        /// Useful for when the user changes UI options.
        /// </remarks>
        private static void reloadUI()
        {
            Debug.Log("Reloading UI.");
            UI.Modes oldMode = ui.Mode;
            ui.destroy();
            ui = new UI();
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
