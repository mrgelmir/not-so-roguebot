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
        yield return null;
        DungeonGeneration.BasicGenerator g = new DungeonGeneration.BasicGenerator(50, 40, 100);


        while (g.NextStep())
        {
            if (slowGeneration)
            {
                gc.GenerateGrid(g.GetCurrentGrid());
                yield return new WaitForSeconds(.1f);
            }
        }

        gc.GenerateGrid(g.GetCurrentGrid());

    }

}
