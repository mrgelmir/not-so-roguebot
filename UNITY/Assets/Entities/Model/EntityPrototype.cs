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
			e.AddMovement(pos);
			e.AddComponent(new EntityVisual("playerVisual"));
			e.AddComponent(new Actor()
			{
				InstantActor = false,
				Type = AIType.None,
				NeededEnergy = 2,
				CurrentEnergy = 0,
			});

			// debug
			//e.GetComponent<Mover>().MoveType = MovementType.Hack;

			return e;
		}

		public static Entity Enemy(GridPosition pos)
		{
			// TODO get id somewhere else
			Entity e = new Entity(++currentID);

			e.AddComponent(new EntityName("enemy"));
			e.AddMovement(pos);
			e.AddComponent(new EntityVisual("enemyVisual"));
			e.AddComponent(new Actor()
			{
				InstantActor = true,
				Type = AIType.Random,
				NeededEnergy = 3,
				CurrentEnergy = 0,
			});

			return e;
		}

		private static void AddMovement(this Entity e, GridPosition pos)
		{

			var position = new Position(pos, GridDirection.North);
			e.AddComponent(position);
			e.AddComponent(new Mover()
			{
				MoveType = MovementType.Walk,
				Pos = position,				
			});
		}
	}
}
