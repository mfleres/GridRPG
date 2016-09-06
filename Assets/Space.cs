using System;
using System.Collections.Generic;

namespace GridRPGCode
{
	public class Space
	{
		private Terrain _terrain;
		private List<Unit> _unitList;

		// Creates a void space
		public Space ()
		{
			_terrain = new Terrain();
			_unitList = new List<Unit>();
		}

		// Creates an empty space with specified terrain
		public Space (Terrain terrain)
		{
			_terrain = new Terrain(terrain);
			_unitList = new List<Unit>();
		}
	}
}

