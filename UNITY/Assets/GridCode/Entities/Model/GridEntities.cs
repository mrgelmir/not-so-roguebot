using System;
using System.Collections;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	public class GridEntities : IEnumerable<EntityData>
	{
		private List<EntityData> entities = new List<EntityData>();
		
		public void AddEntity(EntityData entity)
		{
			entities.Add(entity);
		}

		#region IEnumerable implementation
		public IEnumerator<EntityData> GetEnumerator()
		{
			return new EntityEnumerator(entities.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator() as IEnumerator;
		}
		#endregion
	}

	public class EntityEnumerator : IEnumerator<EntityData>
	{
		private IEnumerator<EntityData> entityListEnumerator;

		public EntityEnumerator(IEnumerator<EntityData> entityEnumerator)
		{
			entityListEnumerator = entityEnumerator;
		}

		public EntityData Current
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
