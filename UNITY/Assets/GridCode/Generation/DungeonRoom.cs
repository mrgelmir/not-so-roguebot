using GridCode.Model;
using System;
using System.Collections.Generic;

namespace GridCode.Generation
{
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
				if (tiles == null)
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
			if (IsBorderPosition(position) || Overlaps(position))
			{
				if (Doors.Contains(position))
				{
					// door does already exist
					//Log.Write("DungeonRoom::AddDoor - A door already exists at position " + position.ToString());
				}
				else
				{
					// the door is actually outside of the room and does not exist already
					Doors.Add(position);
				}
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
					((position.Row == Row + Height - offset) || (position.Row - offset == Row));
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

		public DungeonPosition BorderPosition(GridDirection dir)
		{
			DungeonPosition p = CenterPos;

			// for now we take borders to the left or right side before top or bottom
			if (dir.ContainsDirection(GridDirection.East))
			{
				p.Column = Column + Width;
			}
			else if (dir.ContainsDirection(GridDirection.West))
			{
				p.Column = Column - 1;
			}
			else
			{
				if (dir.ContainsDirection(GridDirection.North))
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
					//Tiles[c][r] = IsBorderPosition(new DungeonPosition(c, r)) ? DungeonTile.WallTile : DungeonTile.FlatTile;
				}
			}

			// TODO add the door tiles
		}
	}
}
