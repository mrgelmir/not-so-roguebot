using Entities.Model;
using GridCode;
using GridCode.Generation;
using GridCode.Visuals;
using UnityEngine;
using Entities.Model.Components;
using GridCode.Entities.Model;

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
	private GridEntities entities = null;

	public GridData GridData
	{
		get { return gridData; }
	}

	public GridEntities Entities
	{
		get { return entities; }
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
		if (instance == null || instance == this)
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

	// temp
	Entity playerEntity = null;
    EntityData oldPlayerEntity = null;
	Vector2 startTouchPos;
	float minSwipeDistance = .2f;
	public UnityEngine.UI.Text label;

	protected void Update()
	{
		GridDirection dir = GridDirection.None;

#if UNITY_EDITOR
		if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
		{
			dir = dir.AddDirection(GridDirection.West);
		}
		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
		{
			dir = dir.AddDirection(GridDirection.East);
		}
		if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
		{
			dir = dir.AddDirection(GridDirection.North);
		}
		if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
		{
			dir = dir.AddDirection(GridDirection.South);
		}
#else
		if(Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);

			if(t.phase == TouchPhase.Began)
			{
				startTouchPos = t.position;
			}

			if(t.phase == TouchPhase.Ended)
			{
				Vector2 swipe = t.position - startTouchPos;
				swipe.x /= Mathf.Min(Screen.width, Screen.height);
				swipe.y /= Mathf.Min(Screen.width, Screen.height);

				label.text = swipe.ToString() + " - " + swipe.magnitude;

				if (swipe.sqrMagnitude >= minSwipeDistance * minSwipeDistance)
				{
					if(swipe.x > minSwipeDistance)
					{
						dir = dir.AddDirection(GridDirection.East);
					}
					else if (swipe.x < -minSwipeDistance)
					{
						dir = dir.AddDirection(GridDirection.West);
					}

					if (swipe.y > minSwipeDistance)
					{
						dir = dir.AddDirection(GridDirection.North);
					}
					else if (swipe.y < -minSwipeDistance)
					{
						dir = dir.AddDirection(GridDirection.South);
					}
				}

				startTouchPos = Vector2.zero;
			}
		}
#endif

		if(dir != GridDirection.None)
		{

			playerEntity.GetComponent<Position>().Pos += dir;

			// OLD below

			TileData targetTile = gridData[oldPlayerEntity.Position + dir];

			// validate move before moving
			if(oldPlayerEntity.CanMove(targetTile))
			{
				oldPlayerEntity.MoveTo(targetTile.Position);
			}
			else
			{
				oldPlayerEntity.Direction = GridDirectionHelper.DirectionBetween(oldPlayerEntity.Position, targetTile.Position);
			}
		}

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

		entities = new GridEntities();


		// find a valid spawn position
		GridPosition spawnPos = new GridPosition(gridData.Columns / 2, gridData.Rows / 2);

		// Old player creation
		{
			oldPlayerEntity = EntityPrototype.Player(GridPosition.Zero);
			//entities.AddEntity(oldPlayerEntity);
			oldPlayerEntity.OnPositionChanged += FindObjectOfType<TileFactory>().UpdateEntityVisual;
			oldPlayerEntity.OnOrientationChanged += FindObjectOfType<TileFactory>().UpdateEntityVisual;
		
			oldPlayerEntity.Position = spawnPos;
			FindObjectOfType<FollowCamera>().Target = FindObjectOfType<TileFactory>().GetEntityVisual(oldPlayerEntity).transform;
		}

		// New player creation
		{
			playerEntity = new Entity();

			playerEntity.AddComponent(new EntityName("The Player"));
			playerEntity.AddComponent(new Position(spawnPos, GridDirection.North));
			playerEntity.AddComponent(new EntityVisual("playerVisual"));

			Position playerPos = playerEntity.GetComponent<Position>();
			playerPos.OnPositionChanged += (int id) =>
			{
				Debug.Log("player with id " + id + " is moving");
			};

			playerEntity.GetComponent<Position>().Pos += GridDirection.North;

			entities.AddEntity(playerEntity);
		}


		


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

	[ContextMenu("Generate dungeon")]
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

	public void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}


}
