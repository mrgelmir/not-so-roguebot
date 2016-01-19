using GridUtils;
using System;
using System.Collections.Generic;

namespace DungeonGeneration
{

	public enum DungeonTileType
	{
		None = 0,
		Flat = 1,
		Wall = 2,
		Door = 3,
		Target = 4, // TODO make this an object on top of the grid instead of a tiletype
	}
	
	public struct DungeonPosition
	{
		public DungeonPosition(int column, int row)
		{
			this.column = column;
			this.row = row;
		}
		
		private int column;

		public int Column
		{
			get { return column; }
			set { column = value; }
		}
		
		private int row;

		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		public void MoveBy(Direction direction, int distance = 1)
		{
			column += direction.GetHorizontalDirection() * distance;
			row += direction.GetVerticalDirection() * distance;
		}

		public bool OverlapsAny(IList<DungeonRoom> rooms)
		{
			return GetOverlappingRoom(rooms) != null;
		}

		public DungeonRoom GetOverlappingRoom(IList<DungeonRoom> rooms)
		{
			foreach (DungeonRoom room in rooms)
			{
				if (room.Overlaps(this))
				{
					return room;
				}
			}
			return null;
		}

		public bool Overlaps(DungeonPosition position)
		{
			// a tile with the same position is also counted as an overlap
			return position.column == column && position.row == row;
		}

		public bool Neigbours(DungeonPosition position)
		{
			// a tile with the same position is also counted as an overlap
			return Math.Abs(position.column - column) <= 1 && Math.Abs(position.row - row) <= 1;
		}

		public override string ToString()
		{
			return string.Format("Column : {0} Row : {1}", column, row);
		}
	}
}
