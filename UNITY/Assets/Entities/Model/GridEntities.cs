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
			entity.OnComponentAdded += OnComponentAdded;
			entity.OnComponentRemoved += OnComponentRemoved;

			// Make up for already added components
			foreach (Component component in entity.Components)
			{
				OnComponentAdded(component);
			}			

			return true;
		}

		public bool RemoveEntity()
		{
			throw new NotImplementedException("removal of entities not yet implemented");
		}

		/// <summary>
		/// Subscribe on all events of entities getting added to the game
		/// </summary>
		/// <param name="onEntityAdd">function called when an entity is added</param>
		/// <param name="onEntityRemove">function called when an entity is removed</param>
		/// <param name="startUpdated">should the add function be called for all current entities</param>
		public void SubscribeOnEntities(Action<Entity> onEntityAdd, Action<Entity> onEntityRemove, bool startUpdated)
		{
			onEntityAdded += onEntityAdd;
			onEntityRemoved += onEntityRemove;

			if (startUpdated && onEntityAdd != null)
			{
				foreach (Entity e in entities.Values)
				{
					onEntityAdd(e);
				}
			}
		}

		public void UnSubscribeFromEntities(Action<Entity> onEntityAdd, Action<Entity> onEntityRemove)
		{
			onEntityAdded -= onEntityAdd;
			onEntityRemoved -= onEntityRemove;
		}

		private void OnComponentAdded(Component component)
		{
			// Temp keep track of added positions here
			if(component is Position)
			{
				Position position = (component as Position);
				position.OnPositionUpdated += PositionChanged;
				PositionChanged(position, GridPosition.Zero);
			}

		}

		private void OnComponentRemoved(Component component)
		{
			// unsubscribe
			if (component is Position)
			{
				(component as Position).OnPositionUpdated -= PositionChanged;
			}
		}


		private Dictionary<GridPosition, int[]> positionRef = new Dictionary<GridPosition, int[]>();
		private void PositionChanged(Position position, GridPosition previousPos)
		{
			// check if previous pos should be removed
			if(positionRef.ContainsKey(previousPos))
			{
				List<int> idsAtPos = new List<int>(positionRef[previousPos]);
				idsAtPos.Remove(position.Entity.ID);
				positionRef[previousPos] = idsAtPos.ToArray();
            }

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
				// TODO make faster
				//var positions = GetComponentEnumerator<Position>();
				//while (positions.MoveNext())
				//{
				//	if (positions.Current.Pos == pos)
				//		return positions.Current.Entity;
				//}

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
