using UnityEngine;
using System.Collections;

namespace GridRPG
{
    /// <summary>
    /// Holds game data and modes
    /// </summary>
    public class Game
    {
        private const string MAP_LIST_FILE = "Assets/Resources/MapList.txt";
        private const string CAMPAIGN_UNIT_LIST_FILE = "Assets/Resources/CampaignUnitList.csv";
        public enum Modes { MainMenu, Map };

        public GridRPG.UnitLibrary unitLibrary;
        public GridRPG.MapLibrary mapLibrary;
        public GridRPG.UI ui;
        public GridRPG.Map map;

        /// <summary>
        /// Supports the following resolutions:
        /// 853x480(wide 480p),
        /// </summary>
        public Vector2 resolution { get; private set; }

        private Modes mode;

        public Game()
        {
            resolution= new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
            unitLibrary = new GridRPG.UnitLibrary();
            unitLibrary.loadCampaignUnitList(CAMPAIGN_UNIT_LIST_FILE);
            mapLibrary = new GridRPG.MapLibrary(unitLibrary);
            mapLibrary.loadMapList(MAP_LIST_FILE);
            ui = new UI(this);
        }

        /// <summary>
        /// Reloads the UI.
        /// </summary>
        /// <remarks>
        /// Useful for when the user changes UI options.
        /// </remarks>
        private void reloadUI()
        {
            Debug.Log("Reloading UI.");
            UI.Modes oldMode = ui.Mode;
            ui.destroy();
            ui = new UI(this);
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
                    this.resolution = new Vector2(853f,480f);
                    reloadUI();
                    break;
            }
        }
    }
}
