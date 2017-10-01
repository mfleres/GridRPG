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

        private Modes mode;

        public Game()
        {
            unitLibrary = new GridRPG.UnitLibrary();
            unitLibrary.loadCampaignUnitList(CAMPAIGN_UNIT_LIST_FILE);
            mapLibrary = new GridRPG.MapLibrary(unitLibrary);
            mapLibrary.loadMapList(MAP_LIST_FILE);
            ui = new UI(this);
        }
        //.
        /// <summary>
        /// Reloads the UI.
        /// </summary>
        /// <remarks>
        /// Useful for when the user changes UI options.
        /// </remarks>
        private void reloadUI()
        {
            UI.Modes oldMode = ui.Mode;

            //TODO: Finish coding.
        }
    }
}
