using GridCode;
using System.Collections.Generic;
using System;

namespace Entities.Model.Components
{
	public enum MoveType
	{
		None,
		Walk,
		WalkSmart,
		Fly,
		Hack,
	}

	public abstract class MovementBehaviour
	{
		// Interface

		public abstract MoveType Type
		{
			get;
		}

		public abstract bool CanEnterTile(TileData tile);

		public abstract bool CanPathThroughTile(TileData tile);

		// factory component

		public static Dictionary<MoveType, MovementBehaviour> moveBehaviours = new Dictionary<MoveType, MovementBehaviour>();

		public static MovementBehaviour GetMoveBehaviour(MoveType moveType)
		{
			MovementBehaviour m;

			if (moveBehaviours.ContainsKey(moveType))
			{
				m = moveBehaviours[moveType];
			}
			else
			{
				switch (moveType)
				{
					default:
					case MoveType.None:
						m = new NotMoveBehaviour();
						break;
					case MoveType.Walk:
						m = new WalkMoveBehaviour(false);
						break;
					case MoveType.WalkSmart:
						m = new WalkMoveBehaviour(true);
						break;
					case MoveType.Fly:
						m = new FlyMoveBehaviour();
						break;
					case MoveType.Hack:
						m = new HackMoveBehaviour();
						break;
				}

				moveBehaviours.Add(moveType, m);
			}


			return m;
		}

		// Private classes to represent different movement behaviours
		// TODO refactor by compressing duplicate functionality

		private class NotMoveBehaviour : MovementBehaviour
		{
			public override MoveType Type
			{
				get
				{
					return MoveType.None;
				}
			}

			public override bool CanEnterTile(TileData tile)
			{
				return false;
			}

			public override bool CanPathThroughTile(TileData tile)
			{
				return false;
			}
		}

		private class WalkMoveBehaviour : MovementBehaviour
		{
			private bool canOpenDoors = false;

			public override MoveType Type
			{
				get
				{
					return MoveType.Walk;
				}
			}

			public WalkMoveBehaviour(bool canOpenDoors)
			{
				this.canOpenDoors = canOpenDoors;
			}

			public override bool CanEnterTile(TileData tile)
			{
				bool canEnter = false;
				switch (tile.Type)
				{
					case GridTileType.Flat:
						canEnter = true;
						break;
					default:
						canEnter = false;
						break;
				}

				foreach (Entity entity in tile.LinkedEntities)
				{

					// TODO make exception for unocked doors if smart
					//if(canOpenDoors)
					//{

					//}

					Position position = entity.GetComponent<Position>();
					if (position != null)
					{
						if (position.Blocking)
						{
							canEnter = false;
							break;
						}
					}
				}

				return canEnter;
			}

			public override bool CanPathThroughTile(TileData tile)
			{
				bool canEnter = false;
				switch (tile.Type)
				{
					case GridTileType.Flat:
						canEnter = true;
						break;
					default:
						canEnter = false;
						break;
				}

				foreach (Entity entity in tile.LinkedEntities)
				{
					PathBlocker blocker = entity.GetComponent<PathBlocker>();
					if (blocker != null && blocker.Block)
						canEnter = false;
				}

				return canEnter;
			}
		}

		private class FlyMoveBehaviour : MovementBehaviour
		{
			public override MoveType Type
			{
				get
				{
					return MoveType.Fly;
				}
			}

			public override bool CanEnterTile(TileData tile)
			{
				bool canEnter = false;
				switch (tile.Type)
				{
					case GridTileType.Flat:
					case GridTileType.Lava:
					case GridTileType.Water:
					case GridTileType.Wall:
					case GridTileType.None:
						canEnter = true;
						break;
					default:
						canEnter = false;
						break;
				}

				foreach (Entity entity in tile.LinkedEntities)
				{

					// TODO make exception for unocked doors if smart
					//if(canOpenDoors)
					//{

					//}

					Position position = entity.GetComponent<Position>();
					if (position != null)
					{
						if (position.Blocking)
						{
							canEnter = false;
							break;
						}
					}
				}

				return canEnter;
			}

			public override bool CanPathThroughTile(TileData tile)
			{
				bool canEnter = false;
				switch (tile.Type)
				{
					case GridTileType.Flat:
					case GridTileType.Lava:
					case GridTileType.Water:
					case GridTileType.Wall:
					case GridTileType.None:
						canEnter = true;
						break;
					default:
						canEnter = false;
						break;
				}

				foreach (Entity entity in tile.LinkedEntities)
				{
					PathBlocker blocker = entity.GetComponent<PathBlocker>();
					if (blocker != null && blocker.Block)
						canEnter = false;
				}

				return canEnter;
			}
		}

		private class HackMoveBehaviour : MovementBehaviour
		{
			public override MoveType Type
			{
				get
				{
					return MoveType.Hack;
				}
			}

			public override bool CanEnterTile(TileData tile)
			{
				return true;
			}

			public override bool CanPathThroughTile(TileData tile)
			{
				return true;
			}
		}
	}

}
