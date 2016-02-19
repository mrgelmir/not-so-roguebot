using System;

namespace Entities.Model.Components
{
	public enum AIType
	{
		None,
		Random,
		Follow, // ?
	}

	public class Actor : Component
	{
		/// <summary>
		/// Tells the system about the duration of the action of this actor
		/// true: Processes instantly 
		///		- AI
		///		- player with an action queue
		/// false: processes over more than one turn 
		///		- player waiting for input
		/// </summary>
		public bool InstantActor;

		// Gives systems an idea of how this actor acts
		public AIType Type;
	}
}
