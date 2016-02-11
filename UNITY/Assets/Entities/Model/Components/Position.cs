using GridCode;
using System;

namespace Entities.Model.Components
{
	public class Position : Component
	{
		// TODO pass entity or entity ID ??
		public Action<int> OnPositionChanged;
		public Action<int> OnOrientationChanged;

		private GridPosition position;
		private GridDirection direction;
		private GridTileType validTiles;

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
					OnPositionChanged(entityID);
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
						OnOrientationChanged(entityID);
				}
			}
		}

		public Position(GridPosition pos, GridDirection dir)
		{
			position = pos;
			direction = dir;
			validTiles = GridTileType.Flat;
		}

		public Position(GridPosition pos, GridDirection dir, GridTileType validTiles)
		{
			position = pos;
			direction = dir;
			this.validTiles = validTiles;
		}

		public bool IsValidTile(GridTileType tileType)
		{
			return validTiles.ContainsTileType(tileType);
		}
	}
}
