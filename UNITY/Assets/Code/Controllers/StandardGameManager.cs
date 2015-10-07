using UnityEngine;
using System.Collections;
using GridUtils;

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

		// spawn player (atm. player wil place itself in the grid)
		cam.Target = Instantiate(playerPrefab).transform;

		// spawn enemies at random (TODO get these form rooms/spawner object)
		for (int i = 0; i < enemyCount; i++)
		{
			Instantiate(enemyPrefab);
		}

		// pass on to start game
		StartGame();
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

		Log.Write("End of turn " + gridMoveManager.CurrentRound);
    }

	public override void FinishGame()
	{
		base.FinishGame();

		// just return to home for now
		Log.Write("Finished Game");
		Application.LoadLevel(0); // temp
	}
}

