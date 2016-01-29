using System;

namespace GridCode.Entities.Model
{
	public class EntityData
	{
		// position on the grid
		private GridPosition position;

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

				position = value;
				if (OnPositionChanged != null)
					OnPositionChanged(this);
			}
		}

		public bool Move(GridDirection dir)
		{

			return false;
		}
	}
}
