using System;
using Entities.Model.Components;
using GridCode;

namespace Entities.Model.Systems
{
	/// <summary>
	/// Keeps data in sync between the entities and grid
	/// </summary>
	public class GridLink
	{
		private GridData grid;

		public GridLink(GridEntities entities, GridData grid)
		{
			this.grid = grid;
			entities.SubscribeOnComponents(OnComponentAdded, OnComponentRemoved, true);
		}

		private void OnComponentAdded(Component component)
		{
			Position position = component as Position;

			if (position != null)
			{
				position.OnPositionUpdated += EntityPositionUpdated;
				EntityPositionUpdated(position, GridPosition.Zero);
			}
		}

		private void OnComponentRemoved(Component component)
		{
			Position position = component as Position;
			if (position != null)
			{
				position.OnPositionUpdated -= EntityPositionUpdated;
				grid[position.Pos].LinkedEntities.Remove(position.Entity);
			}
		}

		private void EntityPositionUpdated(Position position, GridPosition previousPosition)
		{
			grid[previousPosition].LinkedEntities.Remove(position.Entity);
			grid[position.Pos].LinkedEntities.Add(position.Entity);
		}
	}
}
