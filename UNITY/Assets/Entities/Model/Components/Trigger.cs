using GridCode;
using System;

namespace Entities.Model.Components
{
	class Trigger : Component
	{
		/// <summary>
		/// The position to check
		/// this can be an entity's own position or a totally different one
		/// </summary>
		public GridPosition position;

		// callbacks
		public Action<Entity> OnTriggerEnter;
		public Action<Entity> OnTriggerExit;
		public Action<Entity> OnTriggerStay;
	}
}