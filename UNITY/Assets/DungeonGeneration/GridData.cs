using GridUtils;
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

		public List<DungeonPosition> Doors = new List<DungeonPosition>();

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


			// TODO deduce width and height from the nested Tiles list?
			Width = width;
			Height = height;

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

		public bool AddDoor(DungeonPosition position)
		{
			// return false if the door isn't on a border
			if(IsBorderPosition(position) || Overlaps(position))
			{
				// the door is actually outside of the room
				Doors.Add(position);
				return true;
			}
			else
			{
				return false;
			}
			
		}

        private const int minRoomDistance = 1; // keep a gap of 1 for the walls

		public bool Overlaps(DungeonPosition position)
		{
			int offset = -minRoomDistance + 1;

			bool horizontalOverlap = Column - 1 <= position.Column - offset && Column + Width - offset >= position.Column;
			bool verticalOverlap = Row - 1 <= position.Row - offset && Row + Height - offset >= position.Row;

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

		public bool IsBorderPosition(DungeonPosition position)
		{
			int offset = -minRoomDistance + 1;
			return ((position.Column == Column + Width - offset) || (position.Column - offset == Column)) &&
					((position.Row == Row + Height - offset) || (position.Row - offset == Row ));
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

		public bool IsOuterCorner(DungeonPosition position)
		{
			int offset = -minRoomDistance + 1;

			// used for catching edge case
			bool verticalBorder = Row - 1 == position.Row - offset || Row + Height - offset == position.Row;
			bool horizontalBorder = Column - 1 == position.Column - offset || Column + Width - offset == position.Column;

			return verticalBorder && horizontalBorder;
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

		public bool Overlaps(DungeonPosition position)
		{
			foreach (DungeonPosition pos in TilePositions)
			{
				if(pos.Overlaps(position))
				{
					return true;
				}
			}
			return false;
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
	}

	public enum DungeonTileType
	{
		None = 0,
		Flat = 1,
		Wall = 2,
		Door = 3,
	}

}
