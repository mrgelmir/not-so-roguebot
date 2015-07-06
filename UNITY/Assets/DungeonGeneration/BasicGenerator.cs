using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class BasicGenerator : IDungeonGenerator
	{
		private DungeonData data;
		private int maxRooms;

        private DungeonRoom tempRoom = null;

        int counter = 0;
        
        public BasicGenerator()
        {
            
        }

		public void Setup(DungeonGenerationInfo info)
		{
			this.maxRooms = info.MaxRooms;
			data = new DungeonData(info.Width, info.Height);
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

        public bool NextGenerationStep()
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
				while (tempRoom.OverlapsAny(data.Rooms) && c > 0);

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
