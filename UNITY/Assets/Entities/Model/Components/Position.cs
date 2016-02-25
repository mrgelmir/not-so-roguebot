using GridCode;
using System;

namespace Entities.Model.Components
{
	public class Position : Component
	{

		// Data
		public Action<Entity> OnPositionChanged;
		public Action<Entity> OnOrientationChanged;

		public bool Blocking;

		private GridPosition position;
		private GridDirection direction;


		// Accessors
		public GridPosition Pos
		{
			get { return position; }
			set
			{
				// only change if needed
				if (position.Column == value.Column && position.Row == value.Row)
					return;

				// TEMP check for orientation change here
				Orientation = GridDirectionHelper.DirectionBetween(position, value);

				position = value;
				if (OnPositionChanged != null)
					OnPositionChanged(Entity);
			}
		}

		public GridDirection Orientation
		{
			get { return direction; }
			set
			{
				if (direction != value)
				{
					direction = value;
					if (OnOrientationChanged != null)
						OnOrientationChanged(Entity);
				}
			}
		}


		// Constructors
		public Position(GridPosition pos, GridDirection dir)
		{
			position = pos;
			direction = dir;
		}

		public Position(GridPosition pos, GridDirection dir, GridTileType validTiles)
		{
			position = pos;
			direction = dir;
		}
	}
}
