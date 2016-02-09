using System;

namespace GridCode.Entities.Model.Components
{
	public class Position : Component
	{
		// TODO pass entity or entity ID ??
		public Action<int> OnPositionChanged;
		public Action<int> OnOrientationChanged;

		private GridPosition position;
		private GridDirection direction;

		public GridPosition Pos
		{
			get { return position; }
			set
			{
				// only change if needed
				if (position.Column == value.Column && position.Row == value.Row)
					return;

				// TEMP check for orientation change here
				Direction = GridDirectionHelper.DirectionBetween(position, value);

				position = value;
				if (OnPositionChanged != null)
					OnPositionChanged(ID);
			}
		}

		public GridDirection Direction
		{
			get { return direction; }
			set
			{
				if (direction != value)
				{
					direction = value;
					if (OnOrientationChanged != null)
						OnOrientationChanged(ID);
				}
			}
		}

		public Position(GridPosition pos, GridDirection dir)
		{
			position = pos;
			direction = dir;
		}
	}
}
