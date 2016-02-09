using System;
using System.Collections;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	public class GridEntities : IEnumerable<Entity>
	{
		private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

		public bool AddEntity(Entity entity)
		{
			// TODO check if ID does not already exist
			entities.Add(entity.ID, entity);
			return true;
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
