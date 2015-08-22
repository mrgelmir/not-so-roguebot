namespace GridUtils 
{
	[System.Flags]
	public enum Direction
	{
		NONE = 0,
		Up = 1 << 0,
		UpRight = 1 << 0 | 1 << 1,
		Right = 1 << 1,
		DownRight = 1 << 2 | 1 << 1,
		Down = 1 << 2,
		DownLeft = 1 << 2 | 1 << 3,
		Left = 1 << 3,
		UpLeft = 1 << 0 | 1 << 3,
	}

	public static class DirectionHelper
	{
		// let the getter use its own random to keep seeds valid
		public static Direction GetRandomDirection(System.Random rand)
		{
			Direction dir = Direction.NONE;

			// vertical directions
			int randomNr = rand.Next(0, 3);
			if (randomNr == 0)
			{
				dir.AddDirection(Direction.Right);
			}
			else if (randomNr == 1)
			{
				dir.AddDirection(Direction.Left);
			}

			// horizontal directions
			randomNr = rand.Next(0, 3);
			if (randomNr == 0)
			{
				dir.AddDirection(Direction.Up);
			}
			else if (randomNr == 1)
			{
				dir.AddDirection(Direction.Down);
			}

			return dir;
		}

		public static Direction GetRandomAxisAlignedDirection(System.Random rand)
		{
			int randomNr = rand.Next(0, 8);
			randomNr -= randomNr % 2; // make axis aligned
			return (Direction)(1 << randomNr);
		}

		public static void AddDirection(this Direction currentDirection, Direction addedDirection)
		{
			currentDirection |= addedDirection;
		}

		public static void RemoveDirection(this Direction currentDirection, Direction removeDirection)
		{
			currentDirection &= removeDirection;
		}

		public static bool ContainsDirection(this Direction direction, Direction other)
		{
			return (direction & other) == other;
		}

		public static int GetHorizontalDirection(this Direction direction)
		{
			return direction.ContainsDirection(Direction.Right) ? 1 : (direction.ContainsDirection(Direction.Left) ? -1 : 0);
		}

		public static int GetVerticalDirection(this Direction direction)
		{
			return direction.ContainsDirection(Direction.Up) ? 1 : (direction.ContainsDirection(Direction.Down) ? -1 : 0);
		}

		/// <summary>
		/// Rotates the Direction in increments of 45 degrees 
		/// </summary>
		/// <param name="direction to rotate"></param>
		/// <param name="angle in degrees"></param>
		/// <returns>The rotated direction</returns>
		public static Direction RotateBy(this Direction direction, int angle)
		{
			if (angle < 0) // deal with negative angles
			{ angle = 360 - (System.Math.Abs(angle) % 360); }

			int numClockwiseRotations = angle / 45;

			int newDirection = 0;
			int directionInt = (int)direction;

			// check if power of two: if so only a single bit is set
			// TODO rotation of more than dual direction (ie. top, right and left)
			bool singleBitSet = directionInt != 0 && (directionInt & (directionInt - 1)) == 0;

			for (int i = 0; i < numClockwiseRotations; i++)
			{
				// some binary stuff incoming
				if (singleBitSet)
				{
					// shift left by one, clamp within 4 bits bounds, then OR current direction 
					newDirection = ((directionInt << 1) % 15) | directionInt;
				}
				else
				{
					// shift left by one, clamp within 4 bits bounds, then AND current direction
					newDirection = ((directionInt << 1) % 15) & directionInt;
				}

				directionInt = newDirection;
				singleBitSet = !singleBitSet;
			}

			return (Direction)newDirection;
		}
	}
}
