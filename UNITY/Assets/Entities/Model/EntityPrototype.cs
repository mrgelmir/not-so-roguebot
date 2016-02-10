using System;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	public static class EntityPrototype
	{
		public static EntityData Player(GridPosition pos)
		{
			return new EntityData()
			{
				Position = pos,
			};
		}
	}
}
