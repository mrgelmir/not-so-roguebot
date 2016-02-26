using System.Collections;
using System.Collections.Generic;

namespace Entities.Model
{
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
}
