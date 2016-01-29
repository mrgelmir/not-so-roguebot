using System;
using System.Collections;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	class GridEntities : IEnumerable<EntityData>
	{
		private List<EntityData> entities = new List<EntityData>();
		
		public void AddEntity(EntityData entity)
		{
			entities.Add(entity);
		}

		// TODO
		public IEnumerator<EntityData> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
