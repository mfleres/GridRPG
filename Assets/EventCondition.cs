using System;

namespace GridRPG
{
	public abstract class EventCondition
	{
		public abstract bool testCondition();
		public abstract void runEvents();
	}
}

