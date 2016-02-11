using GridCode.Generation;
using System;
using System.Collections.Generic;

namespace GridCode.Model
{
	[Serializable]
	public class DungeonCorridor
	{
		public List<DungeonGenerationPosition> TilePositions = new List<DungeonGenerationPosition>();

		public DungeonCorridor(int width, DungeonRoom startRoom, DungeonRoom endRoom)
		{

			startRoom.LinkedRooms.Add(endRoom);
			endRoom.LinkedRooms.Add(startRoom);

			// temp make path from center to center
			DungeonGenerationPosition startPos = startRoom.CenterPos;
			DungeonGenerationPosition endPos = endRoom.CenterPos;

			//// temp go horizontal first, then vertical
			//int horizontalDistance = startPos.Column - endRoom.CenterPos.Column;
			//int verticalDistance = startPos.Row - endRoom.CenterPos.Row;

			// horizontal first, then vertical. for now
			DungeonGenerationPosition cornerPos = new DungeonGenerationPosition(startPos.Column, endPos.Row);

			TilePositions = new List<DungeonGenerationPosition>(Math.Abs(startPos.Column - endPos.Column) + Math.Abs(startPos.Row - endPos.Row) - 1);

			int row = startPos.Row;
			int col = startPos.Column;
			int counter = startPos.Column > endPos.Column ? -1 : 1;
			for (; col != endPos.Column; col += counter)
			{
				TilePositions.Add(new DungeonGenerationPosition(col, row));
			}
			counter = startPos.Row > endPos.Row ? -1 : 1;
			for (; row != endPos.Row; row += counter)
			{
				TilePositions.Add(new DungeonGenerationPosition(col, row));
			}
		}

		public DungeonCorridor(IList<DungeonGenerationPosition> tiles)
		{
			TilePositions = new List<DungeonGenerationPosition>(tiles);
		}

		public bool Overlaps(DungeonGenerationPosition position)
		{
			foreach (DungeonGenerationPosition pos in TilePositions)
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
