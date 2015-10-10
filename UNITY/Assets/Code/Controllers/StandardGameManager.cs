using UnityEngine;
using System.Collections;
using GridUtils;
using System;
using System.Collections.Generic;

class StandardGameManager: BaseGameManager
{
	[Header("Dungeon Generation")]
	[SerializeField]
	private DungeonGeneration.DungeonGenerationInfo dungeonInfo;

	[Header("Project References")]
	// temporary hardlinked references
	[SerializeField]
	private PlayerMover playerPrefab;

	[SerializeField]
	private GridActor enemyPrefab;

	[Header("Scene References")]
	[SerializeField]
	private FollowCam cam;
	[SerializeField]
	private TextPopup textPopup;

	[Header("Tweakables")]
	public int enemyCount = 5;
	
	public override void InitializeGame()
	{
		base.InitializeGame();

		// generate grid
		DungeonGeneration.IDungeonGenerator gridGenerator = new DungeonGeneration.DungeonWalkGenerator();
		gridGenerator.Setup(dungeonInfo);
		
		gridGenerator.GenerateDungeon();

		gridController.DrawGrid(gridGenerator.GetCurrentGrid());

		// spawn player (atm. player wil place itself in the grid) and make camera follow it
		Instantiate(playerPrefab, Vector3.up * -10f, Quaternion.identity);

		// spawn enemies at random (TODO get these form rooms/spawner object)
		for (int i = 0; i < enemyCount; i++)
		{
			Instantiate(enemyPrefab, Vector3.up * -10f, Quaternion.identity);
		}

		textPopup.Show("Start game", "Avoid Enemies and find the exit of the maze to win the game. \nMay the odds be ever in your favour", StartGame);


		//// pass on to start game
		//StartGame();
	}

	public override void StartGame()
	{
		cam.Target = FindObjectOfType<PlayerMover>().transform;

		base.StartGame();

		StartCoroutine(TemporaryCheck());
	}

	private IEnumerator TemporaryCheck()
	{
		yield return null;

		PathFinder.GetPath(FindObjectOfType<PlayerMover>().CurrentTile, FindObjectOfType<EndTile>(), PahtFindingDone);
	}

	private void PahtFindingDone(IEnumerable<GridTile> path)
	{
		GridTile targetTile = FindObjectOfType<EndTile>();
        foreach (GridTile tile in path)
		{
			if(tile == targetTile)
			{
				return;
			}
		}

		Restart(false);
	}

	private IEnumerator SpawnRoutine(DungeonGeneration.IDungeonGenerator gridGenerator)
	{
		while (gridGenerator.NextGenerationStep())
		{
			gridController.DrawGrid(gridGenerator.GetCurrentGrid());
			yield return new WaitForSeconds(.2f);
		}
	}

	public override void EndOfTurn()
	{
		base.EndOfTurn();

		//Log.Write("End of turn " + gridMoveManager.CurrentRound);
    }

	public override void StopGame()
	{
		base.StopGame();

		textPopup.Show("Disappointed", "Sorry that this level probably wasn't possible to finish :(", GoToMain);
	}

	public override void FinishGame()
	{
		base.FinishGame();

		// stop game move manager
		gridMoveManager.EndGame();

		// check if player is still alive 
		PlayerMover player = FindObjectOfType<PlayerMover>();
		if (player != null && player.HitPoints > 0)
		{
			textPopup.Show("Finished Game", "Congratulations on finishing the game, you did really well.\nGood luck in the next level", Restart);
		}
		else
		{
			textPopup.Show("So sad", "Sorry you didn't make it this time.\nBetter luck next time", GoToMain);
		}
	}

	private void GoToMain()
	{
		Application.LoadLevel(0);
	}

	private void Restart()
	{
		Restart(true);
	}

	private void Restart(bool increment)
	{
		// clear all enemies
		foreach (GridItem item in FindObjectsOfType<GridItem>())
		{
			Destroy(item.gameObject);
		}

		if(increment)
			++enemyCount;

		//restart
		Invoke("InitializeGame", .1f);
	}
}

