using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridCode.Model;

namespace GridCode.Generation
{
	public class BasicGenerator : IDungeonGenerator
	{
		private DungeonGenerationData data;
		private DungeonGenerationInfo info;

        private DungeonRoom tempRoom = null;

        int counter = 0;
        
        public BasicGenerator()
        {
            
        }

		public void Setup(DungeonGenerationInfo info)
		{
			data = new DungeonGenerationData(info.Width, info.Height);
			this.info = info;
		}

		public DungeonGenerationData GenerateDungeon()
		{
			while(NextGenerationStep())
			{
				// empty body
			}

			return GetCurrentGrid();
		}

        public DungeonGenerationData GetCurrentGrid()
        {
            //return a copy with the possible room added
            DungeonGenerationData tempData = new DungeonGenerationData(data);

            if(tempRoom != null)
            {
                tempData.AddRoom(tempRoom);
            }

            return tempData;
        }

        public bool NextGenerationStep()
        {

//			int attempt = 0;

            if (++counter < info.MaxRooms)
            {
                int c = 10;
                do
                {
                    int roomWidth = Random.Range(info.MinRoomWidth, info.MaxRoomWidth);
                    int roomHeight = Random.Range(info.MinRoomHeight, info.MaxRoomHeight);
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
			for (int i = 0; i < data.Rooms.Count; ++i)
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
