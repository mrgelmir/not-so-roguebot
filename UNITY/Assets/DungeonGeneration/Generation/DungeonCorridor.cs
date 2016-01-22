using DungeonGeneration.Generation;
using System;
using System.Collections.Generic;

namespace DungeonGeneration.Model
{
	[Serializable]
	public class DungeonCorridor
	{
		public List<DungeonPosition> TilePositions = new List<DungeonPosition>();

		public DungeonCorridor(int width, DungeonRoom startRoom, DungeonRoom endRoom)
		{

			startRoom.LinkedRooms.Add(endRoom);
			endRoom.LinkedRooms.Add(startRoom);

			// temp make path from center to center
			DungeonPosition startPos = startRoom.CenterPos;
			DungeonPosition endPos = endRoom.CenterPos;

			//// temp go horizontal first, then vertical
			//int horizontalDistance = startPos.Column - endRoom.CenterPos.Column;
			//int verticalDistance = startPos.Row - endRoom.CenterPos.Row;

			// horizontal first, then vertical. for now
			DungeonPosition cornerPos = new DungeonPosition(startPos.Column, endPos.Row);

			TilePositions = new List<DungeonPosition>(Math.Abs(startPos.Column - endPos.Column) + Math.Abs(startPos.Row - endPos.Row) - 1);

			int row = startPos.Row;
			int col = startPos.Column;
			int counter = startPos.Column > endPos.Column ? -1 : 1;
			for (; col != endPos.Column; col += counter)
			{
				TilePositions.Add(new DungeonPosition(col, row));
			}
			counter = startPos.Row > endPos.Row ? -1 : 1;
			for (; row != endPos.Row; row += counter)
			{
				TilePositions.Add(new DungeonPosition(col, row));
			}
		}

		public DungeonCorridor(IList<DungeonPosition> tiles)
		{
			TilePositions = new List<DungeonPosition>(tiles);
		}

		public bool Overlaps(DungeonPosition position)
		{
			foreach (DungeonPosition pos in TilePositions)
			{
				if (pos.Overlaps(position))
				{
					return true;
				}
			}
			return false;
		}
	}
}
