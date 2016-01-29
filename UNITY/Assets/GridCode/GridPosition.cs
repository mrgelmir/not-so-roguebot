using System;

namespace GridCode
{
	[Serializable]
	public struct GridPosition
	{
		public int Column;
		public int Row;

		public GridPosition(int col, int row)
		{
			Column = col;
			Row = row;
		}

		public static GridPosition Zero
		{
			get { return new GridPosition { Column = 0, Row = 0 }; }
		}

		public static GridPosition operator + (GridPosition pos, GridDirection dir)
		{
			return new GridPosition(pos.Column + dir.GetHorizontalDirection(), pos.Row + dir.GetVerticalDirection());
		}

		public static GridPosition operator -(GridPosition pos, GridDirection dir)
		{
			return new GridPosition(pos.Column - dir.GetHorizontalDirection(), pos.Row - dir.GetVerticalDirection());
		}
	}
}