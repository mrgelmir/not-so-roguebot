using Entities.Model;
using Entities.Model.Components;

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
			e.AddMovementComponents(pos);
			e.AddComponent(new HitPoints()
			{
				CurrentHitpoints = 5,
				MaxHitPoints = 5,
			});
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
			Entity e = GetNewEntity();

			e.AddComponent(new EntityName("enemy"));
			e.AddMovementComponents(pos);
			e.AddComponent(new HitPoints()
			{
				CurrentHitpoints = 1,
				MaxHitPoints = 2,
			});
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

		public static Entity SimpleDoor(GridPosition pos)
		{
			Entity e = GetNewEntity();

			// Keep door non-blockable until we can 
			// figure out how to handle opening doors in pathfinder
			e.AddComponent(new Position(pos, GridDirection.None, false));
			e.AddComponent(new EntityVisual("door"));

			// TODO: make this doorInteractable or whatever
			e.AddComponent(new Interactable());
			
			return e;
		}

		public static Entity TestTrigger(GridPosition pos)
		{
			Entity e = GetNewEntity();

			e.AddComponent(new Position(pos, GridDirection.None, false));
			e.AddComponent(new Trigger()
			{
				position = pos,
			});

			e.AddComponent(new EntityVisual("test"));

			return e;
		}

		// Helper functions

		private static Entity GetNewEntity()
		{
			return new Entity(++currentID);
		}

		private static void AddMovementComponents(this Entity e, GridPosition pos)
		{

			var position = new Position(pos, GridDirection.North, true);
			e.AddComponent(position);
			e.AddComponent(new Mover()
			{
				//MoveType = MovementType.Walk,
				MoveBehaviour = MovementBehaviour.GetMoveBehaviour(MoveType.WalkSmart),
				Pos = position,
			});
		}
	}
}
