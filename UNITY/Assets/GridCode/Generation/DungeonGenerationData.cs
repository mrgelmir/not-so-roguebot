using GridCode.Model;
using System;
using System.Collections.Generic;
using UnityEngine; // TODO remove this dependence by using a custom editor for the gridcontainer

namespace GridCode.Generation
{
	[Serializable]
	public class DungeonGenerationData
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

		public DungeonGenerationData(int columns, int rows)
		{
			this.columns = columns;
			this.rows = rows;
		}

		public DungeonGenerationData(DungeonGenerationData other)
		{
			this.columns = other.columns;
			this.rows = other.rows;

			this.rooms = new List<DungeonRoom>(other.rooms);
			this.corridors = new List<DungeonCorridor>(other.corridors);
		}

		public bool AddRoom(DungeonRoom room)
		{
			if (room.Row >= 0 && room.Column >= 0 && (room.Column + room.Width) < columns && (room.Row + room.Height) < rows)
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

		public bool ContainsPosition(DungeonGenerationPosition position)
		{
			return (position.Column > 0 && position.Row > 0 && position.Column < columns && position.Row < rows);
		}

		public GridData GetFlattenedGrid()
		{
			// Create an empty grid
			GridData grid = new GridData(Columns, Rows);

			// Add all rooms
			for (int i = 0; i < Rooms.Count; ++i)
			{
				DungeonGenerationHelpers.AddRoomToGrid(ref grid, Rooms[i], i);
			}

			// Add all corridors
			for (int i = 0; i < Corridors.Count; i++)
			{
				DungeonGenerationHelpers.AddCorridorToGrid(ref grid, Corridors[i]);
			}


			return grid;
		}

		
	}

	public static class DungeonGenerationHelpers
	{
		public static GridPosition ToGridPos(this DungeonGenerationPosition dgPos)
		{
			return new GridPosition(dgPos.Column, dgPos.Row);
		}

		public static void AddRoomToGrid(ref GridData grid, DungeonRoom room, int roomIndex)
		{

			// add grid tiles
			if (grid.ContainsPosition(room.Column, room.Row) && grid.ContainsPosition(room.Column + room.Width, room.Row + room.Height))
			{
				for (int col = 0; col < room.Width; col++)
				{
					for (int row = 0; row < room.Height; row++)
					{
						TileData tile = grid.GetTile(room.Column + col, room.Row + row);
						tile.Type = GridTileType.Flat;
						//tile.Type = room.Tiles[col][row].Type;
						tile.RoomIndex = roomIndex;
					}
				}
			}

			foreach (DungeonGenerationPosition doorPos in room.Doors)
			{
				if (grid.ContainsPosition(doorPos.ToGridPos()))
				{
					grid.DoorPositions.Add(doorPos.ToGridPos());
					//TileData tile = grid.GetTile(doorPos.Column, doorPos.Row);
					//if (tile.Type == GridTileType.None || tile.Type == GridTileType.Wall)
					//{
					//	tile.Type = GridTileType.Flat;
					//	tile.RoomIndex = roomIndex;

					//	// TODO find more decent way to do this
					//	tile.AddObject(new TileObjectData("door"));
					//}
					//else
					//{
					//	Log.Write("DungeonGenerationData::AddRoomToGrid - trying to add a door tile to a tile of type " + tile.Type.ToString() + " at position " + tile.Column + ":" + tile.Row);
					//}
				}
			}

			// TEMP: add walls around the room (move this functionality to the dungeon generation)
			for (int col = room.Column - 1; col < room.Column + room.Width + 1; col++) // top and bottom wall
			{
				if (grid.ContainsPosition(col, room.Row - 1))
				{
					TileData tile = grid.GetTile(col, room.Row - 1);
					if (tile.Type == GridTileType.None)
					{
						tile.Type = GridTileType.Wall;
					}
				}
				if (grid.ContainsPosition(col, room.Row + room.Height))
				{
					TileData tile = grid.GetTile(col, room.Row + room.Height);
					if (tile.Type == GridTileType.None)
					{
						tile.Type = GridTileType.Wall;
					}
				}
			}

			for (int row = room.Row - 1; row < room.Row + room.Height + 1; row++) // left and right wall
			{
				if (grid.ContainsPosition(room.Column - 1, row))
				{
					TileData tile = grid.GetTile(room.Column - 1, row);
					if (tile.Type == GridTileType.None)
					{
						tile.Type = GridTileType.Wall;
					}
				}
				if (grid.ContainsPosition(room.Column + room.Width, row))
				{
					TileData tile = grid.GetTile(room.Column + room.Width, row);
					if (tile.Type == GridTileType.None)
					{
						tile.Type = GridTileType.Wall;
					}
				}
			}
		}

		public static void AddCorridorToGrid(ref GridData grid, DungeonCorridor corridor)
		{
			//Log.Write("adding corridor with length " + corridor.TilePositions.Count);
			foreach (DungeonGenerationPosition position in corridor.TilePositions)
			{
				if (grid.ContainsPosition(position.Column, position.Row))
				{
					TileData tile = grid.GetTile(position.Column, position.Row);
					if (tile.Type == GridTileType.None || tile.Type == GridTileType.Wall) // replace only empty and wall tiles
					{
						tile.Type = GridTileType.Flat;
						tile.RoomIndex = -1; // TODO figure out how to handle corridors
					}
					else
					{
						//Log.Write("AddCorridorToGrid -> trying to add a corridor tile to a tile of type " + tile.Type.ToString() + " at position " + tile.Column + "-" + tile.Row);
					}
				}
			}
		}
	}
}
