using System;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Model
{
	public class GridEntities : IEnumerable<Entity>
	{

		// these are private because people need to get data when they subscribe to these callbacks
		private Action<int> OnEntityAdded;
		private Action<int> OnEntityRemoved;

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
		public void SubscribeOnEntities(Action<int> onEntityAdd, Action<int> onEntityRemove, bool startUpdated)
		{
			OnEntityAdded += onEntityAdd;
			OnEntityRemoved += onEntityRemove;

			if (startUpdated && onEntityAdd != null)
			{
				foreach (int id in entities.Keys)
				{
					onEntityAdd(id);
				}
			}
		}

		public void UnSubscribeFromEntities(Action<int> onEntityAdd, Action<int> onEntityRemove)
		{
			OnEntityAdded -= onEntityAdd;
			OnEntityRemoved -= onEntityRemove;
		}

		#region Indexers

		public Entity this[int id]
		{
			get
			{
				return entities[id];
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
	}

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
}
