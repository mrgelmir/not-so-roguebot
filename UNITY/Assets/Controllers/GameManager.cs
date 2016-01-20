using UnityEngine;
using System.Collections;
using GridActorSystem;
using DungeonGeneration;
using DungeonVisuals;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	[Header("Dungeon Generation")]
	[SerializeField]
	private DungeonGenerationInfo dungeonInfo;

	[Header("Project References")]
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
		// generate dungeon
		IDungeonGenerator dungeonGenerator = new DungeonWalkGenerator();
		dungeonGenerator.Setup(dungeonInfo);
		GridData gridData = dungeonGenerator.GenerateDungeon().GetFlattenedGrid();

		// temp assign color to each index
		Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

		foreach (GridTile tile in gridData)
		{
			GameObject tilePrefab = null;
			if (tile.Type == DungeonTileType.Flat || tile.Type == DungeonTileType.Target)
			{
				tilePrefab = tileCollection.Center;
			}
			else if (tile.Type == DungeonTileType.Wall)
			{
				tilePrefab = tileCollection.Wall;
			}
			else if (tile.Type == DungeonTileType.Door)
			{
				tilePrefab = tileCollection.DoorTop;
			}

			if (tilePrefab != null)
			{
				GameObject tileVisual = Instantiate(tilePrefab);
				tileVisual.transform.position = new Vector3(tile.Column, 0f, tile.Row);
				tileVisual.transform.SetParent(transform, false);
				tileVisual.name = tile.ToString();

				// assign color per room

				Color tileColor = Color.white;
				if (!colorMap.ContainsKey(tile.RoomIndex))
				{
					tileColor = new Color(Random.value, Random.value, Random.value);
					colorMap.Add(tile.RoomIndex, tileColor);
				}
				else
				{
					tileColor = colorMap[tile.RoomIndex];
				}

				SpriteRenderer sr = tileVisual.GetComponentInChildren<SpriteRenderer>();
				if (sr != null)
				{
					sr.color = tileColor;
				}
				else
				{
					Renderer r = tileVisual.GetComponentInChildren<Renderer>();
					if (r != null)
					{
						r.material.color = tileColor;
					}
				}
			}
		}

		//gridMoveManager.OnEndRound += EndOfTurn;
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
