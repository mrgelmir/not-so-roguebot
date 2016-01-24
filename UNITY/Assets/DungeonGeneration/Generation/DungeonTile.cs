using System;

namespace DungeonGeneration.Generation
{
	[Serializable]
	public class DungeonTile
	{
		public DungeonTileType Type;

		private DungeonTile(DungeonTileType type)
		{
			Type = type;
		}

		public static readonly DungeonTile EmptyTile = new DungeonTile(DungeonTileType.None);
		public static readonly DungeonTile FlatTile = new DungeonTile(DungeonTileType.Flat);
		public static readonly DungeonTile WallTile = new DungeonTile(DungeonTileType.Wall);
	}
}
