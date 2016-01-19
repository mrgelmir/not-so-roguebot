namespace GridActorSystem
{
	interface IGridActor
	{
		/// <summary>
		/// Does the actor require input?
		/// </summary>
		/// <returns></returns>
		bool RequiresInput();

		/// <summary>
		/// Process the turn in the current frame without requiring input of any sort
		/// </summary>
		void ProcessTurn();

		/// <summary>
		/// Process the current turn on an actor that requires input or a delay
		/// </summary>
		/// <param name="onEndTurn">THe function that will be called when the turn is finished</param>
		void StartTurn(System.Action onEndTurn);
	}
}
