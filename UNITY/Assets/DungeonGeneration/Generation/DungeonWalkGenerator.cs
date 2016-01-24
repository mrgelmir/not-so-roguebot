using DungeonGeneration.Model;
using System;
using System.Collections.Generic;

namespace DungeonGeneration.Generation
{
	public class DungeonWalkGenerator : IDungeonGenerator
	{
		private DungeonGenerationData workingData;
		private int maxRooms;

		private Action NextStep = null;

		private Random r;

		DungeonGenerationInfo dInfo;

		public void Setup(DungeonGenerationInfo info)
		{
			dInfo = info;
			r = new Random(); // maybe use a seed here?

			maxRooms = info.MaxRooms;
			workingData = new DungeonGenerationData(info.Width, info.Height);

			// set start step
			NextStep = GenerateRoom;
		}

		public DungeonGenerationData GenerateDungeon()
		{
			while (NextGenerationStep())
			{
				// empty body
			}

			//// temp add a target in a random room
			//DungeonRoom randomRoom = workingData.Rooms[r.Next(workingData.Rooms.Count)];
			//randomRoom.Tiles[r.Next(randomRoom.Width)][r.Next(randomRoom.Height)] = DungeonTile.TargetTile;

			return GetCurrentGrid();
		}

		public DungeonGenerationData GetCurrentGrid()
		{
			// Return a copy of the grid
			return new DungeonGenerationData(workingData);
		}

		public bool NextGenerationStep()
		{
			if (NextStep != null)
			{
				NextStep();
				return true;
			}
			else
			{
				// clear working data here if needed
				return false;
			}
		}

		// room generation variables
		private int attemptedRooms = 0;
		private DungeonRoom tempRoom = null;

		private void GenerateRoom()
		{
			++attemptedRooms;

			int c = 10;
			do
			{
				int roomWidth = r.Next(dInfo.MinRoomWidth, dInfo.MaxRoomWidth + 1);
				int roomHeight = r.Next(dInfo.MinRoomHeight, dInfo.MaxRoomHeight + 1);
				tempRoom = new DungeonRoom(r.Next(1, workingData.Columns - (roomWidth + 2)), r.Next(1, workingData.Rows - (roomHeight + 2)), roomWidth, roomHeight);
				--c;
			}
			while (tempRoom.OverlapsAny(workingData.Rooms) && c > 0);

			if (c > 0)
			{
				workingData.AddRoom(tempRoom);
				tempRoom = null;
			}

			// set next step here
			// - if maximum number of rooms or maximum number of generation attempts are not exceeded : continue
			// - otherwise start the corridor generation
			if (workingData.Rooms.Count >= maxRooms || attemptedRooms > maxRooms * 3)
			{
				// move over to corridor generation
				NextStep = GenerateCorridor;
			}
		}

		private const int maxCorridorLength = 300;

		private int consecutiveFailedCorridors = 0;

		private void GenerateCorridor()
		{
			// bookkeeping
			abortCorridor = false;

			// check for no rooms

			// pick a random room and random side to begin from
			// TODO : prefer rooms without connections
			DungeonRoom startRoom = null;

			foreach (DungeonRoom room in workingData.Rooms)
			{
				if (room.Doors.Count <= 0)
				{
					startRoom = room;
					break;
				}
			}

			if (startRoom == null)
			{
				startRoom = workingData.Rooms[r.Next(0, workingData.Rooms.Count)];
			}

			GridDirection currentDirection = GridDirectionHelper.GetRandomAxisAlignedDirection(r);
			while (currentDirection == GridDirection.None)
			{
				currentDirection = GridDirectionHelper.GetRandomAxisAlignedDirection(r);
			}

			DungeonPosition startPos = startRoom.BorderPosition(currentDirection);
			DungeonPosition currentPos = startPos;



			// go on a walk with a changing pseudorandom direction (biased towards current direction and max 90 degree turns) until a room or the edge has been reached
			List<DungeonPosition> pathTiles = new List<DungeonPosition>();
			pathTiles.Add(currentPos);
			do
			{
				currentPos.MoveBy(currentDirection);
				currentDirection = NextDirection(currentDirection);
				pathTiles.Add(currentPos);

			}
			while (!CheckTileCollision(currentPos) && pathTiles.Count < maxCorridorLength);

			// check if we can use this corridor
			if (EvaluateCorridor(pathTiles, startRoom))
			{
				workingData.AddCorridor(new DungeonCorridor(pathTiles));
				consecutiveFailedCorridors = 0;
			}
			else
			{
				++consecutiveFailedCorridors;
			}

			// set next step here
			// TODO: determine when to end walks (when all rooms are connected?)

			bool allRoomsConnected = true;
			int connectedRoomCount = 0;
			foreach (DungeonRoom room in workingData.Rooms)
			{
				if (room.Doors.Count > 0)
				{
					++connectedRoomCount;
				}
			}
			allRoomsConnected = connectedRoomCount == workingData.Rooms.Count;

			if (allRoomsConnected && (workingData.Corridors.Count > maxRooms * 2 || consecutiveFailedCorridors > 100))
			{
				if (workingData.Corridors.Count > maxRooms * 2)
					Log.Write("too many corridors (" + workingData.Corridors.Count + " corridors vs. " + maxRooms + " max rooms");
				else if (consecutiveFailedCorridors > 100)
					Log.Write("too many failed corridors");

				NextStep = null;
			}
			else
			{
				NextStep = GenerateCorridor;
			}
		}

		private int straightBias = 25;
		private bool abortCorridor = false;

		private GridDirection NextDirection(GridDirection direction)
		{
			int number = r.Next(0, straightBias + 2);
			if (number == 0)
			{
				// turn left
				return direction.RotateBy(-90);
			}
			else if (number == 1)
			{
				// turn right
				return direction.RotateBy(90);
			}
			return direction;
		}

		private bool EvaluateCorridor(List<DungeonPosition> currentPath, DungeonRoom startRoom)
		{

			DungeonPosition lastPosition = currentPath[currentPath.Count - 1];
			// path too long or out of bounds
			if (currentPath.Count <= 0 || currentPath.Count >= maxCorridorLength || !workingData.ContainsPosition(lastPosition))
			{
				return false;
			}

			// collision with room
			DungeonRoom endRoom = lastPosition.GetOverlappingRoom(workingData.Rooms);
			if (endRoom != null)
			{
				if (endRoom == startRoom)
				{
					return false;
				}

				if (endRoom.IsOuterCorner(lastPosition))
				{
					return false;
				}
				else
				{
					// add door to end room (evaluation fails when door cannot be placed)
					if (endRoom.IsBorderPosition(lastPosition))
					{
						if (!endRoom.AddDoor(lastPosition))
							return false;
					}

					// add door to start room
					startRoom.AddDoor(currentPath[0]);

					return true;
				}
			}
			else
			{
				//collision with corridor -> only add start door
				startRoom.AddDoor(currentPath[0]);
				return true;
			}
		}

		private bool CheckTileCollision(DungeonPosition position)
		{
			// check out of grid			
			if (!workingData.ContainsPosition(position))
			{
				return true;
			}

			// check rooms
			if (position.OverlapsAny(workingData.Rooms))
			{
				// check edge case, I mean corner case. he he he ...
				//abortCorridor = (position.GetOverlappingRoom(workingData.Rooms).IsOuterCorner(position));
				return true;
			}

			foreach (DungeonCorridor corridor in workingData.Corridors)
			{
				if (corridor.Overlaps(position))
				{
					return true;
				}
			}

			return false;
		}
	}

}
