﻿using GridCode;
using System.Collections.Generic;
using System;

namespace Entities.Model.Components
{

	/// <summary>
	/// Contains info about how the entity will move
	/// </summary>
	public class Mover : Component
	{
		// the path the entity will move along (if empty or null, ignore)
		// TODO change this to Queue of GridPosition
		public Queue<TileData> Path = null;

		// TODO queue for rooms too? (so a full path needn't be calculated)

		/// <summary>
		/// The position component for this mover
		/// </summary>
		public Position Pos;

		// the type of movement
		//public MovementType MoveType;
		public MovementBehaviour MoveBehaviour;
    }

	// TODO write some proper way to access these instead of creating new references each time
	public abstract class MovementBehaviour
	{
		protected MovementBehaviour() { }

		public abstract bool CanEnterTile(TileData tile);
	}

	public class WalkMoveBehaviour : MovementBehaviour
	{
		internal WalkMoveBehaviour() { }

		public override bool CanEnterTile(TileData tile)
		{
			switch (tile.Type)
			{
				case GridTileType.Flat:
					return true;
				default:
					return false;
			}
		}
	}

	public class HackMoveBahaviour : MovementBehaviour
	{
		public override bool CanEnterTile(TileData tile)
		{
			return true;
		}
	}

	// TODO: move this class elsewhere if file gets too big
	public static class MoverFunctionality
	{
		/// <summary>
		/// Warning: this does not take other actors into account!
		/// </summary>
		/// <param name="mover"></param>
		/// <param name="tile"></param>
		/// <returns></returns>
		public static bool CanEnterTile(this Mover mover, TileData tile)
		{

			return mover.MoveBehaviour.CanEnterTile(tile);

			//bool canEnter = false;

			//// check if Mover can enter tile
			//if (mover.MoveType == MovementType.Hack)
			//{
			//	return true;
			//}
			//else if (mover.MoveType == MovementType.None)
			//{
			//	return false;
			//}
			//else if (mover.MoveType == MovementType.Walk)
			//{
			//	switch (tile.Type)
			//	{
			//		case GridTileType.Flat:
			//			canEnter = true;
			//			break;
			//		default:
			//			break;
			//	}
			//}
			//else if (mover.MoveType == MovementType.Fly)
			//{
			//	switch (tile.Type)
			//	{
			//		case GridTileType.None:
			//		case GridTileType.Flat:
			//		case GridTileType.Water:
			//		case GridTileType.Lava:
			//			canEnter = true;
			//			break;
			//		default:
			//			break;
			//	}
			//}

			//// check if Tile is occupied
			//if (canEnter)
			//{


			//}

			//return canEnter;
		}

		public static void InvalidatePath(this Mover mover)
		{
			mover.Path = null;
		}

	}

	public enum MovementType
	{
		None,
		Hack,
		Walk,
		Fly,
	}
}
