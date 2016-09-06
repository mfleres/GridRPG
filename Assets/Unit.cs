using System;
using System.Collections.Generic;

namespace GridRPGCode
{
	public abstract class Unit : UnityEngine.Object
	{
		private List<KeyValuePair<String,int>> _mobility;

		public Unit ()
		{
			name = "Unnamed";
		}
	}
}

