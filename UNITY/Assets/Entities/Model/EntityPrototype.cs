using Entities.Model;
using Entities.Model.Components;

namespace GridCode.Entities.Model
{
	/// <summary>
	/// Factory for creating Entities
	/// atm only selected presets available
	/// </summary>
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

		public static Entity SimpleDoor(GridPosition pos, bool open, bool locked)
		{
			Entity e = GetNewEntity();
			
			Position position = new Position(pos, GridDirection.None, !open);
			e.AddComponent(position);
			e.AddComponent(new EntityVisual("door"));
			
			Interactable i = new Interactable();
			e.AddComponent(i);
			PathBlocker pathBlocker = new PathBlocker() { Block = !open, };
			e.AddComponent(pathBlocker);

			EntityAnimator animator = new EntityAnimator();
			e.AddComponent(animator);

			// add opening/closing mechanic
			if(!locked)
			{
				i.OnInteract += (Entity entity) =>
				{
					position.Blocking = !position.Blocking;
					pathBlocker.Block = !pathBlocker.Block;
					animator.UpdateFloat("openness", pathBlocker.Block ? 1 : 2);
				};
			}

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

			e.AddComponent(new EntityVisual("triggerVisual"));

			return e;
		}

		public static Entity BlockingObject(GridPosition pos)
		{
			Entity e = GetNewEntity();

			e.AddComponent(new Position(pos, GridDirection.None, false));
			e.AddComponent(new PathBlocker() { Block = true, });

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
