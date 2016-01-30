using System;

namespace GridCode.Entities.Model
{
	public class EntityData
	{
		// position and direction on the grid
		private GridPosition position;
		private GridDirection direction;

		// this string is used to draw this actor/entity (null means no visual)
		// TODO fix this
		// TODO maybe use a visual component
		private string visual = null;

		public Action<EntityData> OnPositionChanged;

		public int Column
		{
			get { return position.Column; }
		}

		public int Row
		{
			get { return position.Row; }
		}

		public GridPosition Position
		{
			get { return position; }
			set
			{
				// only change if needed
				if (position.Column == value.Column && position.Row == value.Row)
					return;

				// TEMP check for orientation change here
				direction = GridDirectionHelper.DirectionBetween(position, value);

				position = value;
				if (OnPositionChanged != null)
					OnPositionChanged(this);
			}
		}

		public GridDirection Direction
		{
			get { return direction; }
			// TODO set
		}

		public bool Move(GridDirection dir)
		{
			// TODO check for possible blocking elements

			Position += dir;

			return true;
		}
	}
}
