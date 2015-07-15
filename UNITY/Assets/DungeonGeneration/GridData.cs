using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; // TODO remove this dependence by using a custom editor for the gridcontainer

namespace DungeonGeneration
{
	[Serializable]
	public class DungeonData
	{
		[SerializeField]
		private int columns;
		[SerializeField]
		private int rows;

		[SerializeField]
        private List<DungeonRoom> rooms = new List<DungeonRoom>();
		[SerializeField]
        private List<DungeonCorridor> corridors = new List<DungeonCorridor>();

        public int Columns { get { return columns; } }
        public int Rows { get { return rows; } }

        public List<DungeonRoom> Rooms { get { return rooms; } }
        public List<DungeonCorridor> Corridors { get { return corridors; } }

		public DungeonData(int columns, int rows)
		{
			this.columns = columns;
			this.rows = rows;
		}

        public DungeonData(DungeonData other)
        {
            this.columns = other.columns;
            this.rows = other.rows;

            this.rooms = new List<DungeonRoom>(other.rooms);
            this.corridors = new List<DungeonCorridor>(other.corridors);
        }

		public bool AddRoom(DungeonRoom room)
		{
			if(room.Row >= 0 && room.Column >= 0 && (room.Column + room.Width) <  columns && (room.Row + room.Height) < rows)
			{
                rooms.Add(room);

				return true;
			}

			return false;
		}

        public bool AddCorridor(DungeonCorridor corridor)
        {
            // TODO check if corridor is within grid
            corridors.Add(corridor);

            return false;
        }

		public bool ContainsPosition(DungeonPosition position)
		{
			return (position.Column > 0 && position.Row > 0 && position.Column < columns && position.Row < rows);
		}
	}

	[Serializable]
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
        public static readonly DungeonTile DoorTile = new DungeonTile(DungeonTileType.Door);
	}

	[Serializable]
	public class DungeonRoom
	{
		public int Column;
		public int Row;
		public int Width;
		public int Height;

		private List<List<DungeonTile>> tiles = null;
		public List<List<DungeonTile>> Tiles
		{
			get
			{
				if(tiles == null)
				{
					tiles = new List<List<DungeonTile>>(Width);
					for (int c = 0; c < Width; c++)
					{
						tiles.Add(new List<DungeonTile>(Height));
						for (int r = 0; r < Height; r++)
						{
							// empty object
							Tiles[c].Add(DungeonTile.FlatTile);
						}
					}

					SetTileTypes();
				}
				return tiles;
			}
			private set
			{
				tiles = value;
			}
		}

		[NonSerialized]
        public List<DungeonRoom> LinkedRooms = new List<DungeonRoom>();

		public DungeonPosition CenterPos
		{
			get { return new DungeonPosition(Column + Width / 2, Row + Height / 2); }
		}

		public DungeonRoom(int column, int row, int width, int height)
		{
			Column = column;
			Row = row;


			//// TODO deduce width and height from the nested Tiles list?
			//Width = width;
			//Height = height;

			//Tiles = new List<List<DungeonTile>>(Width);
			//for (int c = 0; c < width; c++)
			//{
			//	Tiles.Add(new List<DungeonTile>(height));
			//	for (int r = 0; r < height; r++)
			//	{
			//		// empty object
			//		Tiles[c].Add(DungeonTile.FlatTile);
			//	}
			//}

			//SetTileTypes();
		}

		public bool AddDoor(int column, int row)
		{
			// return false if the door isn't on a border
			return false;
		}

        private const int minRoomDistance = 1; // keep a gap of 1 for the walls

		public bool Overlaps(DungeonPosition pos)
		{
			int offset = -minRoomDistance + 1;

			bool horizontalOverlap = Column - 1 <= pos.Column - offset && Column + Width - offset >= pos.Column;
			bool verticalOverlap = Row - 1 <= pos.Row - offset && Row + Height - offset >= pos.Row;

			return horizontalOverlap && verticalOverlap;
		}

		public bool Overlaps(DungeonRoom other)
		{
			// TODO maybe use the overlap with two dungeon positions to reduce duplicate code? + define a min/max for the room
            int offset = -minRoomDistance + 1;

            bool horizontalOverlap = Column <= other.Column + other.Width - offset && Column + Width - offset >= other.Column;
            bool verticalOverlap = Row <= other.Row + other.Height - offset && Row + Height - offset >= other.Row;

			return horizontalOverlap && verticalOverlap;
		}

		public bool OverlapsAny(IList<DungeonRoom> otherRooms)
		{
			foreach (DungeonRoom existingRoom in otherRooms)
			{
				if (this.Overlaps(existingRoom))
				{
					return true;
				}
			}

			return false;
		}

		public DungeonPosition BorderPosition(Direction dir)
		{
			DungeonPosition p = CenterPos;

			// for now we take borders to the left or right side before top or bottom
			if(dir.ContainsDirection(Direction.Right))
			{
				p.Column = Column + Width;
			}
			else if (dir.ContainsDirection(Direction.Left))
			{
				p.Column = Column - 1;
			}
			else
			{
				if(dir.ContainsDirection(Direction.Up))
				{
					p.Row = Row + Height;
				}
				else // this means that the Direction.NONE ends up on the bottom
				{
					p.Row = Row - 1;
				}
			}

			return p;
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
                    Tiles[c][r] = DungeonTile.FlatTile;

                    // put walls in room itself?
                    //Tiles[c][r] = IsBorder(c, r) ? DungeonTile.WallTile : DungeonTile.FlatTile;
				}
			}

			// TODO add the door tiles
		}
	}

	[Serializable]
	public class DungeonCorridor
	{
		public List<DungeonPosition> TilePositions = new List<DungeonPosition>();

        public DungeonCorridor(int width, DungeonRoom startRoom, DungeonRoom endRoom)
        {

            startRoom.LinkedRooms.Add(endRoom);
            endRoom.LinkedRooms.Add(startRoom);

            // temp make path from center to center
			DungeonPosition startPos = startRoom.CenterPos;
			DungeonPosition endPos = endRoom.CenterPos;

			//// temp go horizontal first, then vertical
			//int horizontalDistance = startPos.Column - endRoom.CenterPos.Column;
			//int verticalDistance = startPos.Row - endRoom.CenterPos.Row;

			// horizontal first, then vertical. for now
			DungeonPosition cornerPos = new DungeonPosition(startPos.Column, endPos.Row);

			TilePositions = new List<DungeonPosition>(Math.Abs(startPos.Column - endPos.Column) + Math.Abs(startPos.Row - endPos.Row) - 1);

			int row = startPos.Row;
			int col = startPos.Column;
			int counter = startPos.Column > endPos.Column ? -1 : 1;
			for (; col != endPos.Column; col += counter)
			{
				TilePositions.Add(new DungeonPosition(col, row));
			}
			counter = startPos.Row > endPos.Row ? -1 : 1;
			for (; row != endPos.Row; row += counter)
			{
				TilePositions.Add(new DungeonPosition(col, row));
			}
		}
		
		public DungeonCorridor(IList<DungeonPosition> tiles)
		{
			TilePositions = new List<DungeonPosition>(tiles);
		}
    }

	[Serializable]
    public struct DungeonPosition
    {
		public DungeonPosition(int column, int row)
		{
			this.column = column;
			this.row = row;
		}

		[SerializeField]
		private int column;

		public int Column
		{
			get { return column; }
			set { column = value; }
		}

		[SerializeField]
		private int row;

		public int Row
		{
			get { return row; }
			set { row = value; }
		}

		public override string ToString()
		{
			return string.Format("Column : {0} Row : {1}", column, row);
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
    }

	public enum DungeonTileType
	{
		None = 0,
		Flat = 1,
		Wall = 2,
		Door = 3,
	}

}

