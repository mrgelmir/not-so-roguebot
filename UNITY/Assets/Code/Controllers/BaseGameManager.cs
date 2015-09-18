using UnityEngine;
using System.Collections;

public class BaseGameManager : MonoBehaviour
{
	[SerializeField]
	private GridMoveManager gm;

	[SerializeField]
	private GridController gc;

	[SerializeField]
	private DungeonGeneration.DungeonGenerationInfo dungeonInfo;

	/// <summary>
	/// Do the setup of the game here
	/// - generate/set up grid
	/// - create targets/goals
	/// - spawn characters/items/enemies
	/// - show intro text/cinematic
	/// </summary>
	public void InitializeGame()
	{
		// setup grid

		DungeonGeneration.IDungeonGenerator generator = new DungeonGeneration.DungeonWalkGenerator();
		generator.Setup(dungeonInfo);

		gc.GenerateGrid(generator.GenerateDungeon());
		
		// spawn player


	}

	/// <summary>
	/// Use this to actually start the game
	/// - give the player control
	/// - start level ticking
	/// </summary>
	public void StartGame()
	{
		
	}

	/// <summary>
	/// Game is stopped by player
	/// - save game
	/// - go back to main menu
	/// </summary>
	public void StopGame()
	{

	}

	/// <summary>
	/// Intermediary callback to catch a the turn progress
	/// - step-based levels?
	/// - check for victory conditions
	/// </summary>
	public void EndOfTurn()
	{

	}

	/// <summary>
	/// End of the game has been reached
	/// - Cleanup logic
	/// - Inform player of victory
	/// </summary>
	public void FinishGame()
	{

	}

	/// <summary>
	/// Level has failed
	/// - player died
	/// - goal was not met
	/// </summary>
	public void FailGame()
	{

	}
}
