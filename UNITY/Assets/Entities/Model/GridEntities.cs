using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Model.Components;

namespace Entities.Model
{
	public class GridEntities : IEnumerable<Entity>
	{

		// these are private because people need to get data when they subscribe to these callbacks
		private Action<Entity> onEntityAdded;
		private Action<Entity> onEntityRemoved;

		// TODO: change Subscribe and Unsubscribe to something like below?
		//public event Action<int> temp
		//{
		//	add { }
		//	remove { }
		//}

		private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

		public bool AddEntity(Entity entity)
		{
			// TODO check if ID does not already exist
			entities.Add(entity.ID, entity);
			return true;
		}

		/// <summary>
		/// 
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

		#region Indexers

		public Entity this[int id]
		{
			get
			{
				return entities[id];
			}
		}

		public Entity this[GridCode.GridPosition pos]
		{
			get
			{
				// TODO make faster
				var positions = GetComponentEnumerator<Position>();
				while(positions.MoveNext())
				{
					if (positions.Current.Pos == pos)
						return positions.Current.Entity;
				}
				return null;
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

	// iterates over all entities
	public class EntityEnumerator : IEnumerator<Entity>
	{
		private IEnumerator<Entity> entityListEnumerator;

		public EntityEnumerator(IEnumerator<Entity> entityEnumerator)
		{
			entityListEnumerator = entityEnumerator;
		}

		public Entity Current
		{
			get
			{
				return entityListEnumerator.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current as object;
			}
		}

		public void Dispose()
		{
			// nothing to dispose here
		}

		public bool MoveNext()
		{
			return entityListEnumerator.MoveNext();
		}

		public void Reset()
		{
			entityListEnumerator.Reset();
		}
	}

	public class ComponentCollection<T> : IEnumerable<T> where T : Component
	{
		private GridEntities entities;

		public ComponentCollection(GridEntities entities)
		{
			this.entities = entities;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ComponentEnumerator<T>(entities.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator() as IEnumerator;
		}
	}

	// enumerates over a certain component
	public class ComponentEnumerator<T> : IEnumerator<T> where T : Component
	{
		private IEnumerator<Entity> entityEnumerator;

		public ComponentEnumerator(IEnumerator<Entity> enumerator)
		{
			entityEnumerator = enumerator;
		}

		public T Current
		{
			get
			{
				// we rely on the MoveNext function to only allow for entities with the desired component
				return entityEnumerator.Current.GetComponent<T>();
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current as object;
			}
		}

		public void Dispose()
		{
			entityEnumerator.Dispose();
		}

		public bool MoveNext()
		{
			T nextComponent = null;

			// continue to iterate over the entities until the desired component is found
			// breaks when either no component is found or no entities are left
			while(nextComponent == null && entityEnumerator.MoveNext())
			{
				// this will be null if the component is not found
				nextComponent = entityEnumerator.Current.GetComponent<T>();
			}

			// if no compontent is found, this enumerator is done
			return nextComponent != null;
		}

		public void Reset()
		{
			entityEnumerator.Reset();
		}
	}
}
