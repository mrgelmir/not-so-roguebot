using UnityEngine;
using System.Collections;

public class TempGameManager : MonoBehaviour
{
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

		gc.GenerateGrid(DungeonGeneration.BasicGenerator.GenerateGrid(50, 40, 50));

		int enemiesToSpawn = (RequiredEnemies + 1 ) - FindObjectsOfType<GridActor>().Length;

		for(;enemiesToSpawn > 0; -- enemiesToSpawn)
		{
			Instantiate(EnemyPrefab);
		}

		yield return null;

		gm.StartGame();
	}

}
