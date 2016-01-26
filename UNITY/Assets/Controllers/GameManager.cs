using DungeonGeneration;
using DungeonGeneration.Generation;
using DungeonVisuals;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	[Header("Dungeon Generation")]
	[SerializeField]
	private DungeonGenerationInfo dungeonInfo = null;

	[Header("Project References")]
	[SerializeField]
	private TileFactory tileFactory = null;
	[SerializeField]
	private TileCollection tileCollection;

	[Header("Scene References")]
	[SerializeField]
	private TextPopup textPopup = null;

	//[Header("Tweakables")]

	private GridData gridData = null;

	public GridData GridData
	{
		get { return gridData; }
	}

	private static GameManager instance = null;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
				Log.Warning("GameManager::Instance - the game manager instance is null, so it either has not been created, or you're accessing it from the Awake or OnEnable");
			return instance;
		}
	}


	protected void Awake()
	{
		// singleton stuff
		if(instance == null || instance == this)
		{
			instance = this;
		}
		else
		{
			Log.Warning("GameManager::Awake - an instance of GameManager already exists, this should NOT happen");
			Destroy(this); // Don't destroy gameObject, because other scripts may be attached to it.
		}
		
		InitializeGame();
	}

	/// <summary>
	/// Do the setup of the game here
	/// - generate/set up grid
	/// - create targets/goals
	/// - spawn characters/items/enemies
	/// - show intro text/cinematic
	/// - This method should call the StartGame() function
	/// </summary>
	public virtual void InitializeGame()
	{
		GenerateDungeon();

		// set up visual factory
		//tileFactory.SetGrid(gridData);

		//StartGame();
	}

	/// <summary>
	/// Use this to actually start the game
	/// - give the player control (indirect through the gm)
	/// - start level ticking
	/// </summary>
	public virtual void StartGame()
	{

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

	}

	public void GenerateDungeon()
	{
		// generate dungeon
		IDungeonGenerator dungeonGenerator = new DungeonWalkGenerator();
		dungeonGenerator.Setup(dungeonInfo);
		gridData = dungeonGenerator.GenerateDungeon().GetFlattenedGrid();
	}

	public void MessWithDungeon()
	{

		int count = 1000;
		foreach (TileData tile in gridData)
		{
			tile.Type = (tile.Type == DungeonTileType.Wall) ? DungeonTileType.Flat : DungeonTileType.Wall;
			if (--count <= 0)
				break;
		}
	}



}
