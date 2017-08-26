using UnityEngine;
using System.Collections;

namespace GridRPG
{
    /// <summary>
    /// Holds game data and modes
    /// </summary>
    public class Game
    {
        private const string mapListFile = "Assets/Resources/MapList.txt";
        public enum Modes { MainMenu, Map };

        public GridRPG.UnitLibrary unitLibrary;
        public GridRPG.MapLibrary mapLibrary;
        public GridRPG.UI ui;

        private Modes mode;

        public Game()
        {
            unitLibrary = new GridRPG.UnitLibrary();
            mapLibrary = new GridRPG.MapLibrary(unitLibrary);
            loadMapList();
            ui = new UI();
        }

        /// <summary>
        /// Loads the maps listed in mapListFile to the mapLibrary.
        /// </summary>
        private void loadMapList()
        {
            string mapFile;
            System.IO.StreamReader file = null;

            try
            {
                file = new System.IO.StreamReader(mapListFile);
            }
            catch(System.Exception e)
            {
                Debug.Log("MAP FILE NOT FOUND");
            }

            while ((mapFile = file.ReadLine()) != null)
            {
                mapLibrary.addMap(mapFile);
                Debug.Log(mapFile);
            }
        }
    }
}
