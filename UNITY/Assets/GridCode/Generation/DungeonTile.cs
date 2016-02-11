using System;

namespace GridCode.Generation
{
	[Serializable]
	public class DungeonTile
	{
		public GridTileType Type;

		private DungeonTile(GridTileType type)
		{
			Type = type;
		}

		public static readonly DungeonTile EmptyTile = new DungeonTile(GridTileType.None);
		public static readonly DungeonTile FlatTile = new DungeonTile(GridTileType.Flat);
		public static readonly DungeonTile WallTile = new DungeonTile(GridTileType.Wall);
	}
}
