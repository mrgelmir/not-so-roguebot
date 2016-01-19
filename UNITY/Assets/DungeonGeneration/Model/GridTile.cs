using System;

namespace DungeonGeneration
{
	public class GridTile
	{
		public GridTile(int column, int row)
		{
			this.Column = column;
			this.Row = row;
			type = DungeonTileType.None;
		}

		// grid position etc
		private int column;
		private int row;

		// determines the functionality of the tile
		private DungeonTileType type = DungeonTileType.None;

		// to group tiles together
		private int roomIndex = -1;

		public DungeonTileType Type
		{
			get { return type; }
			set { type = value; } // TODO add changed callback
		}

		public int RoomIndex
		{
			get { return roomIndex; }
			set { roomIndex = value; }
		}

		public int Column
		{
			get { return column; }
			set { column = value; }
		}

		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		// TODO determine the style of the tile?

		public override string ToString()
		{
			return Type.ToString() + " Tile - " + Column + ":" + Row;
		}
	}


}
