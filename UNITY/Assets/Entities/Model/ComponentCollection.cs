using Entities.Model.Components;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Model
{
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
			while (nextComponent == null && entityEnumerator.MoveNext())
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