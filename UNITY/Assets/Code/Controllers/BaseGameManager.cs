using UnityEngine;
using System.Collections;

public class BaseGameManager : MonoBehaviour
{
	[Header("Basic References")]
	[SerializeField]
	protected GridMoveManager gridMoveManager;

	[SerializeField]
	protected GridController gridController;

	private void Start()
	{
		InitializeGame();
	}

	/// <summary>
	/// Do the setup of the game here
	/// - generate/set up grid
	/// - create targets/goals
	/// - spawn characters/items/enemies
	/// - show intro text/cinematic
	/// - This method should call thhe StartGame() function
	/// </summary>
	public virtual void InitializeGame()
	{
		gridMoveManager.OnEndRound += EndOfTurn;
	}

	/// <summary>
	/// Use this to actually start the game
	/// - give the player control (indirect through the gm)
	/// - start level ticking
	/// </summary>
	public virtual void StartGame()
	{
		gridMoveManager.StartGame();
	}

	/// <summary>
	/// Game is stopped by player
	/// - save game
	/// - go back to main menu
	/// </summary>
	public virtual void StopGame()
	{
		// TODO see which functionality should overlap with FinishGame?
	}

	/// <summary>
	/// Intermediary callback to catch a the turn progress
	/// - step-based levels?
	/// - check for victory conditions
	/// </summary>
	public virtual void EndOfTurn()
	{
		
	}

	/// <summary>
	/// End of the game has been reached
	/// - Cleanup logic
	/// - Inform player of victory/defeat
	/// </summary>
	public virtual void FinishGame()
	{
		gridMoveManager.OnEndRound -= EndOfTurn;
	}
}
