using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class DungeonData
	{
		private int columns;
		private int rows;

		private List<List<DungeonTile>> tiles;
		public List<List<DungeonTile>> Tiles { get { return tiles; } }

		public DungeonData(int columns, int rows)
		{
			this.columns = columns;
			this.rows = rows;

			tiles = new List<List<DungeonTile>>(columns);
			for (int c = 0; c < columns; c++)
			{
				Tiles.Add(new List<DungeonTile>(rows));
				for (int r = 0; r < rows; r++)
				{
					// empty object
					Tiles[c].Add(DungeonTile.EmptyTile);
				}
			}
		}

		public bool AddRoom(DungeonRoom room)
		{
			if(room.Row >= 0 && room.Column >= 0 && (room.Column + room.Width) <  columns && (room.Row + room.Height) < rows)
			{
				// add the room here

				for (int c = 0; c < room.Width; c++)
				{
					for (int r = 0; r < room.Height; r++)
					{
						tiles[room.Column + c][room.Row + r] = room.Tiles[c][r];
					}
				}

				return true;
			}

			return false;
		}


	}

	public class DungeonTile
	{
		public DungeonTileType Type;

		private DungeonTile(DungeonTileType type)
		{
			Type = type;
		}
		
		public static readonly DungeonTile EmptyTile = new DungeonTile(DungeonTileType.None);
		public static readonly DungeonTile FlatTile = new DungeonTile(DungeonTileType.Flat);
		public static readonly DungeonTile WallTile = new DungeonTile(DungeonTileType.Wall);
	}

	public class DungeonRoom
	{
		public int Column;
		public int Row;
		public int Width;
		public int Height;

		public List<List<DungeonTile>> Tiles;

		public DungeonRoom(int column, int row, int width, int height)
		{
			Column = column;
			Row = row;
			// TODO deduce width and height from the nested Tiles list?
			Width = width;
			Height = height;

			Tiles = new List<List<DungeonTile>>(Width);
			for (int c = 0; c < width; c++)
			{
				Tiles.Add(new List<DungeonTile>(height));
				for (int r = 0; r < height; r++)
				{
					// empty object
					Tiles[c].Add(DungeonTile.FlatTile);
				}
			}

			SetTileTypes();
		}

		public bool AddDoor(int column, int row)
		{

			// return false if the door isn't on a border
			return false;
		}

		public bool Overlaps(DungeonRoom other)
		{
			bool horizontalOverlap = Column <= other.Column + other.Width -1  && Column + Width - 1 >= other.Column;
			bool verticalOverlap = Row <= other.Row + other.Height - 1 && Row + Height - 1 >= other.Row;

			return horizontalOverlap || verticalOverlap;
		}

		private bool IsBorder(int column, int row)
		{
			//only supports rectangular rooms now
			return (column == 0 || column == Width-1) || (row == 0 || row == Height-1);
		}

		private void SetTileTypes()
		{
			//temp: sides are walls, rest are flat
			for (int c = 0; c < Width; c++)
			{
				for (int r = 0; r < Height; r++)
				{
					// empty object
					Tiles[c][r] = IsBorder(c,r)? DungeonTile.WallTile: DungeonTile.FlatTile;
				}
			}
		}
	}

	public class DungeonCorridor
	{

	}

	public enum DungeonTileType
	{
		None = 0,
		Flat = 1,
		Wall = 2,
		Door = 3,
	}
}
