using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridActorSystem
{
	/// <summary>
	/// A GridItem that can move 
	/// </summary>
	interface IMover
	{
		/// <summary>
		/// Move the GridItem to a certain tile
		/// </summary>
		/// <param name="tile">The tile to move to</param>
		/// <returns>true if move is possible/will complete, false if it cannot move to provided tile</returns>
		bool MoveTo(GridTile tile);

		/// <summary>
		/// Checks if a tile can be moved to
		/// </summary>
		/// <param name="tile">The tile to check</param>
		/// <returns>Can the tile be moved to</returns>
		bool ValidateTile(GridTile tile);
	}

	/// <summary>
	/// An actor that is included in the GM's action sequence
	/// - Acts each turn
	/// - has a priority
	/// - processing can take more than one frame (for players/things that require input)
	/// </summary>
	interface IActor
	{
		/// <summary>
		/// Determines if this object's Processing interrupts the current flow
		/// </summary>
		bool InstantProcessing { get; }

		/// <summary>
		/// Makes the Actor process its turn when no input/delay is required
		/// </summary>
		void ProcessTurn();

		/// <summary>
		/// Makes the Actor process its turn when an input/delay is possible
		/// </summary>
		/// <param name="onEndTurn">The function called when the turn is over</param>
		void ProcessTurn(System.Action onEndTurn);
	}

	/// <summary>
	/// Actors that can take damage
	/// - These can have health 
	/// - Need not be destructible
	/// </summary>
	interface IDamageable
	{

	}
}
