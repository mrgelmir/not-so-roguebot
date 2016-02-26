using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Model.Components;
using GridCode;

namespace Entities.Model
{
	public class GridEntities : IEnumerable<Entity>
	{

		// these are private because people could need to get existing data when they subscribe
		private Action<Entity> onEntityAdded;
		private Action<Entity> onEntityRemoved;

		private Action<Component> onComponentAdded;
		private Action<Component> onComponentRemoved;

		private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

		public bool AddEntity(Entity entity)
		{
			// TODO check if ID does not already exist
			// Maybe assign entity ID's here instead?

			entities.Add(entity.ID, entity);

			// Notify listeners
			if (onEntityAdded != null)
			{
				onEntityAdded(entity);
			}

			// Subscribe on adding/removing of components
			entity.OnComponentAdded += ComponentAdded;
			entity.OnComponentRemoved += ComponentRemoved;

			// Make up for already added components
			foreach (Component component in entity.Components)
			{
				ComponentAdded(component);
			}			

			return true;
		}

		public void RemoveEntity(int id)
		{
			RemoveEntity(entities[id]);
		}

		public void RemoveEntity(Entity entity)
		{
			// step 1: remove components
			entity.Remove();
			// step 2: remove entity
			entities.Remove(entity.ID);
			// step 3: ??
			// step 4: profit

			if(onEntityRemoved != null)
			{
				onEntityRemoved(entity);
			}
		}

		/// <summary>
		/// Subscribe on all events of entities getting added to the game
		/// </summary>
		/// <param name="onEntityAdded">function called when an entity is added</param>
		/// <param name="onEntityRemoved">function called when an entity is removed</param>
		/// <param name="startUpdated">should the add function be called for all current entities</param>
		public void SubscribeOnEntities(Action<Entity> onEntityAdded, Action<Entity> onEntityRemoved, bool startUpdated)
		{
			this.onEntityAdded += onEntityAdded;
			this.onEntityRemoved += onEntityRemoved;

			if (startUpdated && onEntityAdded != null)
			{
				foreach (Entity e in entities.Values)
				{
					onEntityAdded(e);
				}
			}
		}

		public void UnSubscribeFromEntities(Action<Entity> onEntityAdded, Action<Entity> onEntityRemoved)
		{
			this.onEntityAdded -= onEntityAdded;
			this.onEntityRemoved -= onEntityRemoved;
		}

		public void SubscribeOnComponents(Action<Component> onComponentAdded, Action<Component> onComponentRemoved, bool startUpdated)
		{
			this.onComponentAdded += onComponentAdded;
			this.onComponentRemoved += onComponentRemoved;

			// do we want this?
			if(startUpdated)
			{
				foreach (Component component in Components<Component>())
				{
					onComponentAdded(component);
				}
			}
		}

		public void UnSubscribeFromComponents(Action<Component> onComponentAdded, Action<Component> onComponentRemoved)
		{
			this.onComponentAdded -= onComponentAdded;
			this.onComponentRemoved -= onComponentRemoved;
		}

		private void ComponentAdded(Component component)
		{
			// Temp keep track of added positions here
			if(component is Position)
			{
				Position position = (component as Position);
				position.OnPositionUpdated += PositionChanged;
				PositionChanged(position, GridPosition.Zero);
			}

			if (onComponentAdded != null)
				onComponentAdded(component);
		}

		private void ComponentRemoved(Component component)
		{
			// unsubscribe
			if (component is Position)
			{
				Position position = (component as Position);
				position.OnPositionUpdated -= PositionChanged;
				RemoveEntityAtPosition(position.Entity, position.Pos);
			}

			if (onComponentRemoved != null)
				onComponentRemoved(component);
		}


		private Dictionary<GridPosition, int[]> positionRef = new Dictionary<GridPosition, int[]>();

		private void PositionChanged(Position position, GridPosition previousPos)
		{
			// 1. Keeping references of positions of entities in grid

			// check if previous pos should be removed
			RemoveEntityAtPosition(position.Entity, previousPos);

			// add new position
			if(positionRef.ContainsKey(position.Pos))
			{
				int[] idsAtPos = positionRef[position.Pos];
				int[] newIds = new int[idsAtPos.Length + 1];

				// copy over curretn ids
				for (int i = 0; i < idsAtPos.Length; i++)
				{
					newIds[i] = idsAtPos[i];
				}

				// add new id
				newIds[newIds.Length - 1] = position.Entity.ID;

				// assign to dictionary
				positionRef[position.Pos] = newIds;
			}
			else
			{
				positionRef.Add(position.Pos, new int[] { position.Entity.ID });
			}

			// 2. Activating triggers or whatever
			// TODO move to trigger system later
			// TODO trigger for blocking/non blocking positions?

			foreach (Trigger trigger in Components<Trigger>())
			{
				// alert trigger of entry
				if(position.Pos == trigger.position)
				{
					trigger.OnTriggerEnter(position.Entity);
				}
				// alert trigger of exit
				else if(previousPos == trigger.position)
				{
					trigger.OnTriggerExit(position.Entity);
				}
			}

		}

		private void RemoveEntityAtPosition(Entity entity, GridPosition position)
		{
			if (positionRef.ContainsKey(position))
			{
				List<int> idsAtPos = new List<int>(positionRef[position]);
				idsAtPos.Remove(entity.ID);
				positionRef[position] = idsAtPos.ToArray();
			}
		}

		#region Indexers

		public Entity this[int id]
		{
			get
			{
				return entities[id];
			}
		}

		public IEnumerable<Entity> this[GridPosition pos]
		{
			get
			{
				int[] entityIDs;
				if(positionRef.TryGetValue(pos, out entityIDs))
				{
					// found at least one entity
					Entity[] foundEntities = new Entity[entityIDs.Length];

					// populate found entities
					for (int i = 0; i < entityIDs.Length; i++)
					{
						foundEntities[i] = entities[entityIDs[i]];
					}

					return foundEntities;
				}

				return new Entity[0];
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<Entity> GetEnumerator()
		{
			return new EntityEnumerator(entities.Values.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator() as IEnumerator;
		}
		#endregion

		// extra component enumerator
		public ComponentEnumerator<T> GetComponentEnumerator<T>() where T : Component
		{
			return new ComponentEnumerator<T>(GetEnumerator());
		}

		public IEnumerable<T> Components<T>() where T : Component
		{
			return new ComponentCollection<T>(this);
		}
	}

}
