using DungeonGeneration;
using DungeonGeneration.Generation;
using DungeonVisuals;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	[Header("Dungeon Generation")]
	[SerializeField]
	private DungeonGenerationInfo dungeonInfo;

	[Header("Project References")]
	[SerializeField]
	private TileFactory tileFactory;
	[SerializeField]
	private TileCollection tileCollection;

	[Header("Scene References")]
	[SerializeField]
	protected GridMoveManager gridMoveManager;
	[SerializeField]
	private FollowCam cam;
	[SerializeField]
	private TextPopup textPopup;

	//[Header("Tweakables")]

	private GameObject[,] gridVisuals;

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
	/// - This method should call the StartGame() function
	/// </summary>
	public virtual void InitializeGame()
	{
		// generate dungeon
		IDungeonGenerator dungeonGenerator = new DungeonWalkGenerator();
		dungeonGenerator.Setup(dungeonInfo);
		GridData gridData = dungeonGenerator.GenerateDungeon().GetFlattenedGrid();

		// prepare the visuals
		gridVisuals = new GameObject[gridData.Columns, gridData.Rows];

		// set up factory
		tileFactory.SetGrid(gridData);

		foreach (TileData tileData in gridData)
		{
			// create all tile visuals and subscribe for further visual updates
			gridVisuals[tileData.Column, tileData.Row] = tileFactory.GetTileVisual(tileData);
			tileData.OnTileChanged += UpdateTileVisual;
			UpdateTileVisual(tileData);
		}

		//gridMoveManager.OnEndRound += EndOfTurn;

		//StartGame();
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

	private void UpdateTileVisual(TileData tileData)
	{
		// get current visual
		GameObject visual = gridVisuals[tileData.Column, tileData.Row];

		if (visual != null)
		{
			// release visual
			tileFactory.ReleaseTileVisual(visual); 
		}

		gridVisuals[tileData.Column, tileData.Row] = tileFactory.GetTileVisual(tileData);
    }
}
