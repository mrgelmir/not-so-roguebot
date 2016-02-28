using System;
using System.Collections.Generic;

namespace GridCode.Generation
{

	// Inspired by and copied from:
	// http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
	public class FilledGenerator
	{

		private Random r = new Random();
		private DungeonGenerationInfo info;
		private List<DungeonRoom> rooms = new List<DungeonRoom>();
		private List<GridPosition> doorPositions = new List<GridPosition>();
		private Stage stage;

		private Dictionary<GridPosition, int> regionDic = new Dictionary<GridPosition, int>();
		private int currentRegion = 0;

		public int WindingPercent = 10;
		public int ExtraConnectorPercent = 10;
		public GridTileType FillTile = GridTileType.None;
		public GridTileType BorderTile = GridTileType.Wall;

		public void Generate(DungeonGenerationInfo info)
		{
			this.info = info;

			// grid size should be odd because of the maze generation
			if (info.Width % 2 == 0 || info.Height % 2 == 0)
			{
				// stage must be odd sized
				throw new ArgumentException("the dungeon should be odd-sized");
			}

			// create stage and fill with empty tiles
			stage = new Stage(info.Width, info.Height, GridTileType.None);


			// create rooms
			GenerateRooms();

			// fill remaining space with mazes
			for (int y = 1; y < info.Height; y += 2)
			{
				for (int x = 1; x < info.Width; x += 2)
				{
					GridPosition pos = new GridPosition(x, y);
					GridTileType tile = stage[pos];
					if (tile != GridTileType.Flat)
					{
						GrowMaze(pos);
					}
				}
			}

			// connect regions
			ConnectRegions();

			// remove dead ends
			RemoveDeadEnds();

			// simplify corridors

			// decorate rooms

			// add walls
			AddWalls();

			// done
		}

		public GridData GetDungeon()
		{
			GridData grid = new GridData(info.Width, info.Height);

			//int roomIndex = 0;

			//foreach (DungeonRoom room in rooms)
			//{
			//	DungeonGenerationHelpers.AddRoomToGrid(ref data, room, ++roomIndex);
			//}

			for (int col = 0; col < stage.Width; col++)
			{
				for (int row = 0; row < stage.Height; row++)
				{
					GridPosition pos = new GridPosition(col, row);
					if (grid.ContainsPosition(pos))
					{
						grid.GetTile(pos).Type = stage[pos];

						if (regionDic.ContainsKey(pos))
							grid.GetTile(pos).RoomIndex = regionDic[pos];

					}
				}
			}

			grid.DoorPositions = doorPositions;

			return grid;
		}

		private void GenerateRooms()
		{
			// Make odd-sized rooms here

			int maxTries = 100;
			do
			{
				// generate room 
				// check for odd size to make sure grid aligns with maze
				int width = r.Next(info.MinRoomWidth / 2, info.MaxRoomWidth / 2) * 2 + 1;
				int height = r.Next(info.MinRoomHeight / 2, info.MaxRoomHeight / 2) * 2 + 1;

				// generate random pos
				int col = r.Next(1, (info.Width - (width + 1)) / 2) * 2 + 1;
				int row = r.Next(1, (info.Height - (height + 1)) / 2) * 2 + 1;

				// create new room 
				DungeonRoom room = new DungeonRoom(col, row, width, height);

				// check overlap with existing rooms
				bool overlaps = room.OverlapsAny(rooms);

				// if overlaps, do not add and decrement max tries
				if (overlaps)
				{
					--maxTries;
				}
				// add room
				else
				{
					rooms.Add(room);
				}
			}
			while (rooms.Count < info.MaxRooms && maxTries > 0);

			foreach (DungeonRoom room in rooms)
			{
				StartRegion();
				for (int col = 0; col < room.Width; col++)
				{
					for (int row = 0; row < room.Height; row++)
					{
						Carve(new GridPosition(room.Column + col, room.Row + row));
					}
				}
			}
		}

		private void ConnectRegions()
		{
			Dictionary<GridPosition, HashSet<int>> connectorRegions = new Dictionary<GridPosition, HashSet<int>>();


			for (int col = 1; col < stage.Width - 1; col++)
			{
				for (int row = 1; row < stage.Height - 1; row++)
				{
					GridPosition pos = new GridPosition(col, row);

					if (stage[pos] == GridTileType.Flat)
						continue;

					// create all regions possibly linked with this tile
					HashSet<int> regions = new HashSet<int>();
					foreach (GridDirection dir in GridDirectionUtil.AxialDirections)
					{
						int region;
						if (regionDic.TryGetValue(pos + dir, out region))
						{
							regions.Add(region);
						}
					}

					// if only one region is found -> this is no possible connector
					if (regions.Count < 2)
						continue;

					connectorRegions[pos] = regions;
				}
			}

			List<GridPosition> connectors = new List<GridPosition>(connectorRegions.Keys);


			// keep track of all merged regions
			Dictionary<int, int> merged = new Dictionary<int, int>();
			HashSet<int> openRegions = new HashSet<int>();
			for (int i = 0; i <= currentRegion; i++)
			{
				merged[i] = i;
				openRegions.Add(i);
			}

			// keep connecting regions until only one is left
			while (openRegions.Count > 1)
			{
				//Log.Write(openRegions.Count);
				if (connectors.Count <= 0)
				{
					break;
				}
				GridPosition connector = connectors[r.Next(connectors.Count)];

				// carve connection
				AddJunction(connector);

				// get merged connected regions
				HashSet<int> regions = new HashSet<int>();
				foreach (int region in connectorRegions[connector])
				{
					regions.Add(merged[region]);
				}

				// the destination region
				int dest = -1;
				// all non-destination regions that need to be merged
				List<int> sources = new List<int>();

				// set destination and others
				foreach (int item in regions)
				{
					if (dest < 0)
						dest = item;
					else
						sources.Add(item);
				}


				// check for already merged regions and merge these too
				for (int i = 0; i <= currentRegion; i++)
				{
					if (sources.Contains(merged[i]))
					{
						merged[i] = dest;
					}
				}

				// remove unneeded sources
				foreach (int region in sources)
				{
					openRegions.Remove(region);
				}

				// remove unneeded connectors
				connectors.RemoveAll((GridPosition pos) =>
				{
					// don't allow adjacent connectors
					if (connector.DistanceTo(pos) < 2)
					{
						return true;
					}

					// allow to stay if connector still connects different regions
					HashSet<int> connectingRegions = new HashSet<int>();
					foreach (int region in connectorRegions[pos])
					{
						connectingRegions.Add(merged[region]);
					}
					if (connectingRegions.Count > 1)
					{
						return false;
					}

					//allow for occasional multiple connections
					if (r.Next(100) < ExtraConnectorPercent)
						AddJunction(pos);

					return true;
				});


			}
		}

		// Implementation of the "growing tree" algorithm from here:
		// http://www.astrolog.org/labyrnth/algrithm.htm
		private void GrowMaze(GridPosition start)
		{
			List<GridPosition> cells = new List<GridPosition>();
			GridDirection lastDir = GridDirection.None;


			StartRegion();
			Carve(start);

			cells.Add(start);
			while (cells.Count > 0)
			{
				GridPosition cell = cells[cells.Count - 1];

				List<GridDirection> unmadeCells = new List<GridDirection>();

				foreach (GridDirection dir in GridDirectionUtil.AxialDirections)
				{
					if (CanCarve(cell, dir))
						unmadeCells.Add(dir);
				}

				if (unmadeCells.Count > 0)
				{
					GridDirection dir;

					// see if we go straight, or if we turn
					if (unmadeCells.Contains(lastDir) && r.Next(100) > WindingPercent)
					{
						dir = lastDir;
					}
					else
					{
						dir = unmadeCells[r.Next(unmadeCells.Count)];
					}

					Carve(cell + dir);
					Carve(cell.MoveBy(dir, 2));

					cells.Add(cell.MoveBy(dir, 2));
					lastDir = dir;
				}
				else
				{
					// no adjacent cells to carve
					cells.RemoveAt(cells.Count - 1);

					// path has ended
					lastDir = GridDirection.None;
				}
			}

		}

		private void RemoveDeadEnds()
		{
			bool done = false;

			while (!done)
			{
				done = true;

				// deflated double loop
				for (int col = 1; col < stage.Width - 1; col++)
				{
					for (int row = 1; row < stage.Height - 1; row++)
					{
						GridPosition pos = new GridPosition(col, row);
						if (stage[pos] != GridTileType.Flat)
							continue;

						// if only one exit -> dead end
						int exits = 0;
						foreach (GridDirection dir in GridDirectionUtil.AxialDirections)
						{
							if (stage[pos + dir] == GridTileType.Flat)
								++exits;
						}

						if (exits != 1)
							continue;

						done = false;
						SetTile(pos, FillTile);
					}
				}
			}
		}

		private void AddWalls()
		{
			for (int col = 0; col < stage.Width; col++)
			{
				for (int row = 0; row < stage.Height; row++)
				{
					GridPosition pos = new GridPosition(col, row);

					if (stage[pos] == FillTile)
					{
						// if one of the neighbours is a flat tile, add a border tile
						foreach (GridDirection dir in GridDirectionUtil.DiagonalDirections)
						{
							if (stage[pos + dir] == GridTileType.Flat)
							{
								stage[pos] = BorderTile;
								break;
							}
						}
					}
				}
			}
		}

		// helper functions

		private void StartRegion()
		{
			currentRegion++;
		}

		private bool CanCarve(GridPosition pos, GridDirection dir)
		{
			if (!stage.Contains(pos.MoveBy(dir, 3)))
				return false;

			return stage[pos.MoveBy(dir, 2)] != GridTileType.Flat;
		}

		private void Carve(GridPosition pos, GridTileType type = GridTileType.Flat)
		{
			SetTile(pos, type);
			regionDic[pos] = currentRegion;
		}

		private void AddJunction(GridPosition pos)
		{
			SetTile(pos, GridTileType.Flat);
			doorPositions.Add(pos);
		}

		private void SetTile(GridPosition pos, GridTileType type)
		{
			stage[pos] = type;
		}

		// temp data structure for dungeon
		private class Stage
		{
			private GridTileType[,] tiles;

			public int Width
			{
				get { return tiles.GetLength(0); }
			}
			public int Height
			{
				get { return tiles.GetLength(1); }
			}

			public Stage(int width, int height, GridTileType fillType)
			{
				tiles = new GridTileType[width, height];
				FillWithTile(fillType);
			}

			public void FillWithTile(GridTileType tiletype)
			{
				for (int col = 0; col < tiles.GetLength(0); col++)
				{
					for (int row = 0; row < tiles.GetLength(1); row++)
					{
						tiles[col, row] = tiletype;
					}
				}
			}

			public bool Contains(GridPosition pos)
			{
				return
					(pos.Column > 0 && pos.Column < tiles.GetLength(0))
					&& (pos.Row > 0 && pos.Row < tiles.GetLength(1));
			}

			public GridTileType this[GridPosition pos]
			{
				get
				{
					return Contains(pos) ? tiles[pos.Column, pos.Row] : GridTileType.None;
				}
				set
				{
					if (Contains(pos))
						tiles[pos.Column, pos.Row] = value;
				}
			}
		}
	}
}
