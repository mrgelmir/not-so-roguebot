using Entities.Model;
using Entities.Model.Components;
using System;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	public static class EntityPrototype
	{
		// TODO: make a better entity system when this breaks
		private static int currentID = 0;

		public static Entity Player(string name, GridPosition pos)
		{
			// TODO get id somewhere else
			Entity e = new Entity(++currentID); 

			e.AddComponent(new EntityName(name));
			e.AddComponent(new Position(pos, GridDirection.North));
			e.AddComponent(new EntityVisual("playerVisual"));
			e.AddComponent(new Actor()
			{
				InstantActor = false,
				Type = AIType.None,
			});

			return e;
		}

		public static Entity Enemy(GridPosition pos)
		{
			// TODO get id somewhere else
			Entity e = new Entity(++currentID);

			e.AddComponent(new EntityName("enemy"));
			e.AddComponent(new Position(pos, GridDirection.North));
			e.AddComponent(new EntityVisual("enemyVisual"));
			e.AddComponent(new Actor()
			{
				InstantActor = true,
				Type = AIType.Random,
			});

			return e;
		}
	}
}
