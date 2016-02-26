using System;

namespace GridCode
{
	// TODO move to flyweight pattern
	// http://gameprogrammingpatterns.com/flyweight.html

	[Flags]
	public enum GridTileType
	{
		None = 0x1,
		Flat = 0x2,
		Wall = 0x4,
		Block = 0x8,

		// examples: aren't used... yet
		Water = 0x10,
		Lava = 0x20,
	}

	public static class GridTileTypeHelper
	{

		public static bool ContainsTileType(this GridTileType tileType, GridTileType other)
		{
			return (tileType & other) == other;
		}

	}

}
