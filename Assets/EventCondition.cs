﻿using System;

namespace GridRPGCode
{
	public abstract class EventCondition
	{
		public abstract bool testCondition();
		public abstract void runEvents();
	}
}

