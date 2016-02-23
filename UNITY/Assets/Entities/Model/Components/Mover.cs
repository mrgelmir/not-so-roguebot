using GridCode;
using System.Collections.Generic;

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

		// tiles per turn this entity can move
		public int Speed;

		// the type of movement8
		public MovementType MoveType;

	}

	public enum MovementType
	{
		None,
		Walk,
		Fly,
	}
}
