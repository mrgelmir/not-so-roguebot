using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridUtils;

public class TempGameManager : MonoBehaviour
{
    public bool slowGeneration = false;
	public bool onlyGeneration = false;

	public GameObject overviewCamera;

	public GridMoveManager gm;
	public GridController gc;

	public GridActor EnemyPrefab;
	public int RequiredEnemies = 2;

	public GridContainer customGridData = null;

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
		overviewCamera.SetActive(true);
		gm.EndGame();


        //gc.GenerateGrid(DungeonGeneration.BasicGenerator.GenerateGrid(50, 40, 50));

        yield return StartCoroutine(SpawnInspectionRoutine());

		overviewCamera.SetActive(onlyGeneration);
		if(onlyGeneration)
		{
			yield break;
		}

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
		if(customGridData != null)
		{
			gc.GenerateGrid(customGridData.Data);
			yield break;
		}

		// TODO move the selection of dungeon generator elsewhere
		DungeonGeneration.DungeonGenerationInfo info = new DungeonGeneration.DungeonGenerationInfo()
		{
			Width = 50,
			Height = 40,
			MaxRooms = 20,

			MinRoomWidth = 5,
			MinRoomHeight = 4,
			MaxRoomWidth = 10,
			MaxRoomHeight = 8,
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

	public void SetSlowGeneration(bool b)
	{
		slowGeneration = b;
	}
	public void SetOnlyGeneration(bool b)
	{
		onlyGeneration = b;
	}
}
