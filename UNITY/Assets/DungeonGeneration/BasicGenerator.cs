using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class BasicGenerator
	{
		public static DungeonData GenerateGrid(int width, int height, int maxRooms)
		{
			DungeonData newGrid = new DungeonData(width, height);

			List<DungeonRoom> rooms = new List<DungeonRoom>();

//			int attempt = 0;

			for (int i = 0; i < maxRooms; i++)
			{
//				Debug.Log(attempt.ToString());
//				++attempt;

				DungeonRoom possibleRoom;
				int counter = 10;
				do
				{
					int roomWidth = Random.Range(4, 10);
					int roomHeight = Random.Range(4, 10);
					possibleRoom = new DungeonRoom(Random.Range(0, width - (roomWidth + 1)), Random.Range(0, height - (roomHeight + 1)), roomWidth, roomHeight);
					--counter;
				}
				while(OverlapsAny(rooms, possibleRoom) && counter > 0);

				if(counter > 0)
				{
					rooms.Add(possibleRoom);
				}
			}

			foreach (DungeonRoom room in rooms)
			{
				newGrid.AddRoom(room);
			}
			
			return newGrid;
		}

		private static bool OverlapsAny(List<DungeonRoom> existingRooms, DungeonRoom room)
		{
			foreach (DungeonRoom existingRoom in existingRooms)
			{
				if(room.Overlaps(existingRoom))
				{
				   return true;
				}
			}

			return false;
		}

        // object stuff

        DungeonData data;
        int maxRooms;

        DungeonRoom tempRoom;

        int counter = 0;
        
        public BasicGenerator(int width, int height, int maxRooms)
        {
            this.maxRooms = maxRooms;

            data = new DungeonData(width, height);
        }

        public DungeonData GetCurrentGrid()
        {
            //return a copy with the possible room added
            DungeonData tempData = new DungeonData(data);

            if(tempRoom != null)
            {
                tempData.AddRoom(tempRoom);
            }

            return tempData;
        }

        public bool NextStep()
        {

//			int attempt = 0;

            if (++counter < maxRooms)
            {
                int c = 10;
                do
                {
                    int roomWidth = Random.Range(4, 10);
                    int roomHeight = Random.Range(4, 10);
                    tempRoom = new DungeonRoom(Random.Range(0, data.Columns - (roomWidth + 1)), Random.Range(0, data.Rows - (roomHeight + 1)), roomWidth, roomHeight);
                    --c;
                }
                while (OverlapsAny(data.Rooms, tempRoom) && c > 0);

                if (c > 0)
                {
                    data.AddRoom(tempRoom);
                    tempRoom = null;
                }
                return true;
            }
            else
            {
                tempRoom = null;
                AddCorridors();
                return false;
            }
        }

        private void AddCorridors()
        {
            // pick pick two random rooms and connect them
			for (int i = 0; i < 10; ++i)
			{
				ConnectTwoRandomRooms();
			}
            // keep connecting one of both rooms to a non-connected room (maybe find closest non-connected room?)

            // when complete, connect some random rooms together
        }

		private void ConnectTwoRandomRooms()
		{
			int firstRoom = Random.Range(0, data.Rooms.Count);
			int secondRoom;
			do
			{
				secondRoom = Random.Range(0, data.Rooms.Count);
			}
			while (secondRoom == firstRoom);

			data.AddCorridor(new DungeonCorridor(1, data.Rooms[firstRoom], data.Rooms[secondRoom]));
		}
	}
}
