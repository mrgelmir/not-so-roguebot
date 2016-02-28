using Entities.Model;
using GridCode;
using GridCode.Generation;
using GridCode.Visuals;
using UnityEngine;
using Entities.Model.Components;
using GridCode.Entities.Model;
using System.Collections;
using Entities.Model.Systems;
using PathFinding;

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
	Vector2 startTouchPos;
	float minSwipeDistance = .12f;
	public UnityEngine.UI.Text label;

	private const float inputDelay = .3f;
	private float currentInputDelay = 0f;

	protected void Update()
	{
		GridDirection dir = GridDirection.None;

#if UNITY_EDITOR

		if (Input.anyKeyDown)
		{
			currentInputDelay = inputDelay;
		}

		if (Input.anyKey && (currentInputDelay -= Time.deltaTime) < 0f)
		{
			currentInputDelay = inputDelay;

			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				dir = dir.AddDirection(GridDirection.West);
			}
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				dir = dir.AddDirection(GridDirection.East);
			}
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
			{
				dir = dir.AddDirection(GridDirection.North);
			}
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
			{
				dir = dir.AddDirection(GridDirection.South);
			}
		}

		if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
		{
			currentInputDelay = inputDelay;
			dir = dir.AddDirection(GridDirection.West);
		}
		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
		{
			currentInputDelay = inputDelay;
			dir = dir.AddDirection(GridDirection.East);
		}
		if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
		{
			currentInputDelay = inputDelay;
			dir = dir.AddDirection(GridDirection.North);
		}
		if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
		{
			currentInputDelay = inputDelay;
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

		//// temp moving around code
		//if (dir != GridDirection.None)
		//{

		//	Position playerPos = playerEntity.GetComponent<Position>();

		//	if (playerPos.IsValidTile(gridData[playerPos.Pos + dir].Type))
		//	{
		//		playerPos.Pos += dir;
		//	}
		//	else
		//	{
		//		playerPos.Orientation = dir;
		//	}
		//}

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

		// -- setup --

		entities = new GridEntities();
		GenerateDungeon();
		actorSystem = new ActorSystem(entities, gridData);
		healthSystem = new HealthSystem(entities);
		gridLink = new GridLink(entities, gridData);
        InputController.Instance.Grid = gridData;

		// -- spawning -- 

		// find a valid spawn position
		GridPosition spawnPos = gridData.GetRandomTile(GridTileType.Flat).Position;


		// New player creation
		playerEntity = EntityPrototype.Player("The Player", spawnPos);
		entities.AddEntity(playerEntity);
		
		// dirty, dirty temp -> refactor to camera component
		StartCoroutine(SetCameraTargetDelayed());
		
		// temp enemy creation
		for (int i = 0; i < 10; i++)
		{
			spawnPos = gridData.GetRandomTile(GridTileType.Flat).Position;
			Entity enemy = EntityPrototype.Enemy(spawnPos);
			entities.AddEntity(enemy);

		}

		// temp triggers
		for (int i = 0; i < 30; i++)
		{
			spawnPos = gridData.GetRandomTile(GridTileType.Flat).Position;

			Entity e = EntityPrototype.TestTrigger(spawnPos);

			entities.AddEntity(e);
		}


		// -- game start -- 

		StartGame();
		
	}

	private IEnumerator SetCameraTargetDelayed()
	{
		yield return null;
		FindObjectOfType<FollowCamera>().Target = FindObjectOfType<Entities.Visual.EntityRenderer>().GetEntityVisual(playerEntity.ID).transform;
	}

	/// <summary>
	/// Use this to actually start the game
	/// - give the player control (indirect through the gm)
	/// - start level ticking
	/// </summary>
	public virtual void StartGame()
	{
		StartTurn();
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



	private ComponentEnumerator<Actor> actorEnumerator;
	private ActorSystem actorSystem;
	private HealthSystem healthSystem;
	private GridLink gridLink;

	private void StartTurn()
	{
		// start of turn setup
		actorEnumerator = entities.GetComponentEnumerator<Actor>();
		actorSystem.OnResume += ContinueTurn;


		ContinueTurn();
	}

	private void ContinueTurn()
	{
		// TODO move this logic to the ActorSystem or another System
		while (actorEnumerator.MoveNext())
		{
			Actor actor = actorEnumerator.Current;

			//Log.Write("Start turn for actor " + actor, Color.green);
			if (!actorSystem.HandleActor(actor))
			{
				// break processing this turn and wait for OnResume
				//Log.Write("Pause turn for actor " + actor, Color.yellow);
				return;
			}
			//Log.Write("End turn for actor " + actor, Color.red);
		}

		// end of turn cleanup
		actorEnumerator.Dispose();
		actorSystem.OnResume -= ContinueTurn;

		// remove all destroyed entities
		healthSystem.HandleHealth();

		EndOfTurn();
	}

	/// <summary>
	/// Intermediary callback to catch a the turn progress
	/// - step-based levels?
	/// - check for victory conditions
	/// </summary>
	public virtual void EndOfTurn()
	{

		//wait for a while, then start next turn
		StartCoroutine(StartTurndelayed(.1f));
	}

	private IEnumerator StartTurndelayed(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartTurn();
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

		// TODO re-create room objects (flood-fill) and determine doors
		// ie. a door is added when a gap with width 1 (or 2) is first found
		
		foreach (GridPosition doorPos in gridData.DoorPositions)
		{
			entities.AddEntity(EntityPrototype.SimpleDoor(doorPos));
		}
		
		gridData.ConstructTileGraph();

		// temp grid editor visualisation
		foreach (Node<TileData> node in gridData.Graph.Nodes.Values)
		{
			foreach (Edge<TileData> edge in node.Edges)
			{
				Vector3 ray = GridDirectionHelper.DirectionBetween(node.Data.Position, edge.Node.Data.Position).ToDirection();
				Debug.DrawRay(node.Data.Position.ToWorldPos(), ray * .3f, Color.green, float.MaxValue);
			}
		}
	}

	public void TestButton()
	{
		foreach (EntityName nameComponent in entities.Components<EntityName>())
		{
			if(nameComponent.NameString == "The Player")
			{
				Mover m = nameComponent.Entity.GetComponent<Mover>();
				
				if(m.MoveBehaviour.Type == MoveType.Walk)
				{
					m.MoveBehaviour = MovementBehaviour.GetMoveBehaviour(MoveType.Hack);
				}
				else
				{
					m.MoveBehaviour = MovementBehaviour.GetMoveBehaviour(MoveType.Walk);
				}

			}
		}
	}

	public void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}


}
