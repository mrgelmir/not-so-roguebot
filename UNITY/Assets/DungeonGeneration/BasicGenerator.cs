﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class BasicGenerator
	{
		public static DungeonData GenerateGrid(int width, int height, int maxRooms)
		{
			DungeonData newGrid = new DungeonData(width, height);

			List<DungeonRoom> rooms = new List<DungeonRoom>(maxRooms);

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
//					Debug.Log("Added room");
				}
//				else
//				{
//					Debug.Log("Dumped room");
//				}
			}

			foreach (DungeonRoom room in rooms)
			{
				newGrid.AddRoom(room);
			}

//			newGrid.AddRoom(new DungeonRoom(1, 1, Random.Range(4, 6), Random.Range(4, 6)));

//			foreach (List<DungeonTile> column in newGrid.Tiles)
//			{
//				string col = "";
//				foreach (DungeonTile tile in column)
//				{
//					col += tile.Type.ToString() + " ";
//				}
//				Debug.Log(col);
//			}

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

        int width;
        int height;
        int maxRooms;

        List<DungeonRoom> rooms;
        DungeonRoom tempRoom;

        int counter = 0;
        
        public BasicGenerator(int width, int height, int maxRooms)
        {

            this.width = width;
            this.height = height;
            this.maxRooms = maxRooms;

            rooms = new List<DungeonRoom>(maxRooms);
        }

        public DungeonData GetCurrentGrid()
        {
            DungeonData newGrid = new DungeonData(width, height);

            foreach (DungeonRoom room in rooms)
            {
                newGrid.AddRoom(room);
            }

            if(tempRoom != null)
            {
                newGrid.AddRoom(tempRoom);
            }

            return newGrid;
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
                    tempRoom = new DungeonRoom(Random.Range(0, width - (roomWidth + 1)), Random.Range(0, height - (roomHeight + 1)), roomWidth, roomHeight);
                    --c;
                }
                while (OverlapsAny(rooms, tempRoom) && c > 0);

                if (c > 0)
                {
                    rooms.Add(tempRoom);
                    tempRoom = null;
                }
                return true;
            }
            else
            {
                tempRoom = null;   
                return false;
            }
        }
	}
}
