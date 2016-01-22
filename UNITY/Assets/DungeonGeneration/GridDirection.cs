﻿namespace DungeonGeneration
{
	/// <summary>
	/// Enum containing 8 main directions
	/// Warning: Adding N and E does not result in Ne
	/// Nw|N|Ne
	///	 W|X|E
	/// Sw|S|Se
	/// </summary>
	[System.Flags]
	public enum GridDirection
	{
		None = 0,
		North = 0x1,
		NorthEast = 0x2,
		East = 0x4,
		SouthEast = 0x8,
		South = 0x10,
		SouthWest = 0x20,
		West = 0x40,
		NorthWest = 0x80,
	}

	public static class GridDirectionHelper
	{
		// let the getter use its own random to keep seeds valid
		public static GridDirection GetRandomDirection(System.Random rand)
		{
			GridDirection dir = GridDirection.None;

			dir = (GridDirection)rand.Next(0x100);

			return dir;
		}

		public static GridDirection GetRandomAxisAlignedDirection(System.Random rand)
		{
			GridDirection dir = GridDirection.None;

			int randomNr = rand.Next(3);
			if (rand.Next(2) == 1)
			{
				// vertical directions
				if (randomNr == 0)
				{
					dir = dir.AddDirection(GridDirection.East);
				}
				else if (randomNr == 1)
				{
					dir = dir.AddDirection(GridDirection.West);
				}
			}
			else
			{
				if (randomNr == 0)
				{
					dir = dir.AddDirection(GridDirection.North);
				}
				else if (randomNr == 1)
				{
					dir = dir.AddDirection(GridDirection.South);
				}
			}

			return dir;
		}

		public static GridDirection AddDirection(this GridDirection currentDirection, GridDirection addedDirection)
		{
			return currentDirection | addedDirection;
		}

		public static GridDirection RemoveDirection(this GridDirection currentDirection, GridDirection removeDirection)
		{
			return currentDirection & removeDirection;
		}

		public static bool ContainsDirection(this GridDirection direction, GridDirection other)
		{
			return (direction & other) == other;
		}

		public static int GetHorizontalDirection(this GridDirection direction)
		{
			return direction.ContainsDirection(GridDirection.East) ? 1 : (direction.ContainsDirection(GridDirection.West) ? -1 : 0);
		}

		public static int GetVerticalDirection(this GridDirection direction)
		{
			return direction.ContainsDirection(GridDirection.North) ? 1 : (direction.ContainsDirection(GridDirection.South) ? -1 : 0);
		}

		private const int bitMask = 0xFF;
		private const int overflowCount = 8;

		/// <summary>
		/// Rotates the Direction in increments of 45 degrees 
		/// </summary>
		/// <param name="direction to rotate"></param>
		/// <param name="angle in degrees"></param>
		/// <returns>The rotated direction</returns>
		public static GridDirection RotateBy(this GridDirection direction, int angle)
		{
			if (angle < 0) // deal with negative angles
			{ angle = 360 - (System.Math.Abs(angle) % 360); }

			int numClockwiseRotations = angle / 45;
			//Log.Write("rotate by " + numClockwiseRotations);

			int directionInt = (int)direction;
			int part1 = directionInt << numClockwiseRotations; // get the bits shifted to the left
			int part2 = directionInt >> overflowCount - numClockwiseRotations; // get the overflow for 4 bytes
			return (GridDirection)((part1 | part2) & bitMask); // throw the bits together and mask
		}
	}
}
