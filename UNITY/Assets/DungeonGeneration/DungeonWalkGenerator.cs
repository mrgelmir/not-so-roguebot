using DungeonGeneration;
using System;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class DungeonWalkGenerator : IDungeonGenerator
	{
		private DungeonData workingData;
		private int maxRooms;

		private Action NextStep = null;

		private Random r;

		public void Setup(DungeonGenerationInfo info)
		{
			r = new Random(); // maybe use a seed here?

			this.maxRooms = info.MaxRooms;
			workingData = new DungeonData(info.Width, info.Height);

			// set start step
			NextStep = GenerateRoom;
		}

		public DungeonData GetCurrentGrid()
		{
			// Return a copy of the grid
			return new DungeonData(workingData);
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
				int roomWidth = r.Next(4, 10);
				int roomHeight = r.Next(4, 10);
				tempRoom = new DungeonRoom(r.Next(0, workingData.Columns - (roomWidth + 1)), r.Next(0, workingData.Rows - (roomHeight + 1)), roomWidth, roomHeight);
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
			if (workingData.Rooms.Count >= maxRooms || attemptedRooms > 10)
			{
				// move over to corridor generation
				NextStep = GenerateCorridor;
			}
		}

		private void GenerateCorridor()
		{
			// check for no rooms

			// pick a random room and random side to begin from
			DungeonRoom startRoom = workingData.Rooms[r.Next(0, workingData.Rooms.Count)];

			DungeonPosition startPos = startRoom.CenterPos;
			DungeonPosition currentPos = startPos;
			Directions currentDirection = DirectionHelper.GetRandomAxisAlignedDirection(r);

			while(currentDirection == Directions.NONE)
			{
				currentDirection = DirectionHelper.GetRandomAxisAlignedDirection(r);
			}

			// go on a walk with a changing pseudorandom direction (biased towards current direction and max 90 degree turns) until a room or the edge has been reached

			int maxCorridorLength = 100;
			List<DungeonTile> pathTiles = new List<DungeonTile>();
			do
			{
				currentPos.MoveBy(currentDirection);

			}
			while (false && pathTiles.Count < maxCorridorLength);

			// hit something
			{
				// check if needed to abort (end of map ... )

				// add doors to start room

				// add door to other room or to other corridor
			}


			// set next step here

			// TODO: determine when to end walks (when all rooms are connected?)
			NextStep = null;
		}
	}

}
