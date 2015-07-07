using UnityEngine;
using System.Collections;

public class TempGameManager : MonoBehaviour
{
    public bool slowGeneration = false;

	public GridMoveManager gm;
	public GridController gc;

	public GridActor EnemyPrefab;
	public int RequiredEnemies = 2;

	protected void Start()
	{
		RestartGame();
	}

	public void RestartGame()
	{
		StartCoroutine(RestartGameRoutine());
	}

	private IEnumerator RestartGameRoutine()
	{
//		gm.EndGame();
//        if (Random.value > .1f)
//            gc.GenerateRandomGrid();
//        else
//            gc.GenerateRoom();

        //gc.GenerateGrid(DungeonGeneration.BasicGenerator.GenerateGrid(50, 40, 50));

        yield return StartCoroutine(SpawnInspectionRoutine());

		// dungeon generation only
		yield break;

		int enemiesToSpawn = (RequiredEnemies + 1 ) - FindObjectsOfType<GridActor>().Length;

		for(;enemiesToSpawn > 0; -- enemiesToSpawn)
		{
			Instantiate(EnemyPrefab);
		}

		yield return null;

		gm.StartGame();
	}

    private IEnumerator SpawnInspectionRoutine()
    {
		// TODO move the selection of dungeon generator elsewhere
		DungeonGeneration.DungeonGenerationInfo info = new DungeonGeneration.DungeonGenerationInfo()
		{
			Width = 50,
			Height = 40,
			MaxRooms = 20,

			MinRoomWidth = 4,
			MinRoomHeight = 4,
			MaxRoomWidth = 8,
			MaxRoomHeight = 6,
		};

		DungeonGeneration.IDungeonGenerator g = new DungeonGeneration.DungeonWalkGenerator();
		//DungeonGeneration.IDungeonGenerator g = new DungeonGeneration.BasicGenerator();
		g.Setup(info);
		
        yield return null;

        while (g.NextGenerationStep())
        {
            if (slowGeneration)
            {
                gc.GenerateGrid(g.GetCurrentGrid());
				yield return new WaitForSeconds(.1f);
				//yield return null;
            }
        }

        gc.GenerateGrid(g.GetCurrentGrid());

    }

}
