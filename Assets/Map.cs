using System;
using System.Collections.Generic;

namespace GridRPGCode
{
	class Map
	{
		private List<List<Space>> _spaces;
		private List<EventCondition> _eventConditions;

		Map()
		{
			_spaces = new List<List<Space>>();
			_eventConditions = new List<EventCondition>();
		}
	}
}
