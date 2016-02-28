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

		public bool Neighbours(GridPosition other, bool includeDiagonal = false)
		{
			// vertical - horizontal only
			// this (ab-)uses the fact that the difference between axial adjacent tiles is always 1
			// if we check for diagonals check for diagonals, both the Col and Row are exactly one apart
			return (1 == (Math.Abs(Column - other.Column) + Math.Abs(Row - other.Row)) ||
				(includeDiagonal && (1 == Math.Abs(Column - other.Column) && 1 == Math.Abs(Row - other.Row))));
		}

		public override string ToString()
		{
			return Column + ":" + Row;
		}

		public override int GetHashCode()
		{
			return Column.GetHashCode() ^ Row.GetHashCode();
		}

		// move to extensions?
		public int DistanceTo(TileData otherTile)
		{
			return DistanceTo(otherTile.Position);
		}
		
		// move to extensions?
		public int DistanceTo(GridPosition otherPosition)
		{
			return Math.Abs(Column - otherPosition.Column) + Math.Abs(Row - otherPosition.Row);
		}

		public override bool Equals(object obj)
		{
			return obj is GridPosition ? Equals((GridPosition)obj) : false;
		}

		public bool Equals(GridPosition other)
		{
			return Column == other.Column && Row == other.Row;
		}

		public static bool operator ==(GridPosition pos1, GridPosition pos2)
		{
			return pos1.Equals(pos2);
		}

		public static bool operator !=(GridPosition pos1, GridPosition pos2)
		{
			return !pos1.Equals(pos2);
		}

		public static GridPosition operator +(GridPosition pos, GridDirection dir)
		{
			return new GridPosition(pos.Column + dir.GetHorizontalDirection(), pos.Row + dir.GetVerticalDirection());
		}

		public static GridPosition operator -(GridPosition pos, GridDirection dir)
		{
			return new GridPosition(pos.Column - dir.GetHorizontalDirection(), pos.Row - dir.GetVerticalDirection());
		}

		
	}

	public static class GridPositionExtensions
	{
		public static GridPosition MoveBy(this GridPosition pos, GridDirection dir, int v)
		{
			for (int i = 0; i < Math.Abs(v); i++)
			{
				pos += dir;
			}
			return pos;
		}
	}
}