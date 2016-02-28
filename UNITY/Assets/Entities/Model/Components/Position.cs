using GridCode;
using System;

namespace Entities.Model.Components
{
	public class Position : Component
	{

		// Data
		public Action<Entity> OnPositionChanged;
		public Action<Entity> OnOrientationChanged;

		/// <summary>
		/// Gets called when the position gets updated
		/// Position is current state
		/// GridPosition is previous position
		/// </summary>
		public Action<Position, GridPosition> OnPositionUpdated;

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

				GridPosition previousPosition = Pos;

				// TEMP check for orientation change here
				Orientation = GridDirectionUtil.DirectionBetween(position, value);

				position = value;

				// notify listeners
				if (OnPositionChanged != null)
					OnPositionChanged(Entity);

				if (OnPositionUpdated != null)
					OnPositionUpdated(this, previousPosition);
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
		public Position(GridPosition pos, GridDirection dir, bool blocking)
		{
			position = pos;
			direction = dir;
			Blocking = blocking;
		}
	}
}
