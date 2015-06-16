using UnityEngine;
using System.Collections;

public interface IGameAction 
{
	int ActionCost { get; }

	ActionData Data { get; }

	/// <summary>
	/// Called once to enable linking the action to the actor
	/// </summary>
	/// <param name="attatchedActor">The owner of this action</param>
	void Init(GridActor attatchedActor);

	/// <summary>
	/// Should be called at the end of the actor's turn to provide the behaviour the possibility to reset if needed
	/// </summary>
	void EndTurn();

	/// <summary>
	/// Hands the control over to the action
	/// </summary>
	/// <param name="onActionComplete">Called when action is completed.</param>
	/// <param name="onActionCancelled">Called when action is cancelled.</param>
	void ExecuteAction(System.Action onActionComplete, System.Action onActionCancelled);

    /// <summary>
    /// Cancels the action. This allows the player/behaviour to stop the current action
    /// </summary>
    void CancelAction();
}
