﻿using DungeonGeneration;
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

		DungeonGenerationInfo dInfo;

		public void Setup(DungeonGenerationInfo info)
		{
			dInfo = info;
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
				int roomWidth = r.Next(dInfo.MinRoomWidth, dInfo.MaxRoomWidth + 1);
				int roomHeight = r.Next(dInfo.MinRoomHeight, dInfo.MaxRoomHeight + 1);
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
			if (workingData.Rooms.Count >= maxRooms || attemptedRooms > 20)
			{
				// move over to corridor generation
				NextStep = GenerateCorridor;
			}
		}

		private void GenerateCorridor()
		{
			// bookkeeping
			abortCorridor = false;

			// check for no rooms

			// pick a random room and random side to begin from
			// TODO : prefer rooms without connections
			DungeonRoom startRoom = workingData.Rooms[r.Next(0, workingData.Rooms.Count)];

			Direction currentDirection = DirectionHelper.GetRandomAxisAlignedDirection(r);
			DungeonPosition startPos = startRoom.BorderPosition(currentDirection);
			DungeonPosition currentPos = startPos;

			while(currentDirection == Direction.NONE)
			{
				currentDirection = DirectionHelper.GetRandomAxisAlignedDirection(r);
			}

			// go on a walk with a changing pseudorandom direction (biased towards current direction and max 90 degree turns) until a room or the edge has been reached

			int maxCorridorLength = 20;
			List<DungeonPosition> pathTiles = new List<DungeonPosition>();
			pathTiles.Add(currentPos);
			do
			{
				currentPos.MoveBy(currentDirection);
				currentDirection = NextDirection(currentDirection);
				pathTiles.Add(currentPos);

			}
			while (!Collision(currentPos) && pathTiles.Count < maxCorridorLength);

			if (abortCorridor || pathTiles.Count > maxCorridorLength) // check if needed to abort (end of map, reached max length ... )
			{

			}
			else // hit something
			{
				// create the corridor and add to dungeon
				workingData.AddCorridor(new DungeonCorridor(pathTiles));

				// add doors to start room

				// add door to other room or to other corridor
			}


			// set next step here

			// TODO: determine when to end walks (when all rooms are connected? -> temp 10 corridors)

			if (workingData.Corridors.Count > 30)
			{
				NextStep = null;
			}
			else
			{
				NextStep = GenerateCorridor;
			}
		}

		private int corridorCounter = 0;
		private int straightBias = 20;
		private bool abortCorridor = false;

		private Direction NextDirection(Direction direction)
		{
			int number = r.Next(0, straightBias + 2);
			if (number == 0)
			{
				// turn left
				return direction.RotateBy(-90);
			}
			else if(number == 1)
			{
				// turn right
				return direction.RotateBy(90);
			}			
			return direction;
		}

		private bool Collision(DungeonPosition position)
		{
			// check grid			
			if(!workingData.ContainsPosition(position))
			{
				abortCorridor = true;
				return true;
			}

			if (position.OverlapsAny(workingData.Rooms))
			{
				abortCorridor = false;
				return true;
			}

			return false;
		}
	}

}
