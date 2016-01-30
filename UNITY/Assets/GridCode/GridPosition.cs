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

		public override bool Equals(object obj)
		{
			return obj is GridPosition ? Equals((GridPosition)obj) : false;
		}

		public bool Equals(GridPosition other)
		{
			return Column == other.Column && Row == other.Row;
		}

		public static bool operator == (GridPosition pos1, GridPosition pos2)
		{
			return pos1.Equals(pos2);
		}

		public static bool operator !=(GridPosition pos1, GridPosition pos2)
		{
			return !pos1.Equals(pos2);
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