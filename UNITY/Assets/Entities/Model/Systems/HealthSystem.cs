using Entities.Model.Components;
using System.Collections.Generic;

namespace Entities.Model.Systems
{
	public class HealthSystem
	{

		private readonly GridEntities entities;

		public HealthSystem(GridEntities entities)
		{
			this.entities = entities;
		}

		public void HandleHealth()
		{
			// Loop through all health components and remember which to remove - the Grim reaper

			List<int> destroyList = new List<int>();

			foreach (HitPoints hitPoints in entities.Components<HitPoints>())
			{
				if (hitPoints.CurrentHitpoints <= 0)
				{
					destroyList.Add(hitPoints.Entity.ID);
				}
			}

			// remove all entities marked for deletion
			foreach (int id in destroyList)
			{
				entities.RemoveEntity(id);
			}
		}
	}
}