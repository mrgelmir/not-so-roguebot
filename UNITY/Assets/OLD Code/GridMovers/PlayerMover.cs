﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMover : GridActor, ITargeter
{
	[SerializeField]
	private MeleeAction meleeAction;
	[SerializeField]
	private ActionPanel actionPanel;

	// IMPLEMENTATION

	public override void Setup()
	{
		base.Setup();
		SetTile(grid.StartTile);

		meleeAction.Init(this);

		if (actionPanel == null)
		{
			// this is ugly -> find a way to spawn the actionpanel instead of finding it
			actionPanel = FindObjectOfType<ActionPanel>();
		}


	}

	public override void StartTurn(System.Action<GridActor> done)
	{
		base.StartTurn(done);

		actionPanel.Reset();
		foreach (BaseGameAction action in actions)
		{
			actionPanel.AddActionButton(action, ActionButtonClicked);
		}
	}

	public override void StopTurn()
	{
		base.StopTurn();
		DisableInput();
	}

	protected override void ProgressTurn()
	{
		base.ProgressTurn();

		// check current path
		if (currentPath != null && currentPath.Count > 0)
		{
			bool validMove = MakeMove(currentPath.Dequeue());

			if (!validMove)
			{
				currentPath.Clear();
			}
		}

		//enable input 
		EnableInput();
	}

	protected override void FinishTurn()
	{
		base.FinishTurn();

		actionPanel.Reset();
		DisableInput();
	}

	public override void ActorDestroyed()
	{
		// do not destroy player for now. Awww yeah!
		base.ActorDestroyed();

		//LeaveTile();
		
		FindObjectOfType<GameManager>().FinishGame();
	}

	protected void OnDestroy()
	{
		// temporary
		if(actionPanel != null)
			actionPanel.gameObject.SetActive(true);
	}
	
	// MESSAGES

	void ActorClicked(GridActor actor)
	{
		// Temp, move to seperate behaviour
		if (actor.CurrentTile.IsNeighbour(currentTile))
		{
			DisableInput();

			meleeAction.SetTarget(actor.CurrentTile);
			ExecuteAction(meleeAction);
		}
	}

	void TileClicked(GridTileView tile)
	{
		if (tile.IsNeighbour(currentTile))
		{
			MakeMove(tile);
		}
		else if (tile.Walkeable)
		{
			PathFinder.GetPath(currentTile, tile, SetNewPath);
		}
	}

	private void ActionButtonClicked(BaseGameAction action)
	{
		DisableInput();
		ExecuteAction(action);
	}


	//Targeting
	System.Action<GridTileView> onTargetFound = null;
	public void RequestTargetNeigbourTile(System.Action<GridTileView> onTargetFound)
	{
		//Debug.Log("PlayerMover - Requesting neigbour target");
		//TODO check meigbouring tiles first: If only one possible target is found -> attack that one

		this.onTargetFound = onTargetFound;

		// delayed subscribe to prevent buttons from triggering these events when subscribing
		StartCoroutine(Subscribe());
	}

	public void RequestTargetTile(System.Action<GridTileView> onTargetFound)
	{
		//Debug.Log("PlayerMover - Requesting target");
		this.onTargetFound = onTargetFound;

		StartCoroutine(Subscribe());
	}

	private IEnumerator Subscribe()
	{
		yield return null;

		InputController.Instance.OnActorClicked += OnTargetingClick;
		InputController.Instance.OnTileClicked += OnTargetingClick;
	}

	private void OnTargetingClick(GridActor actor)
	{
		//Debug.Log("found target actor");
		OnTargetingClick(actor.CurrentTile);
	}

	private void OnTargetingClick(GridTileView tile)
	{
		InputController.Instance.OnActorClicked -= OnTargetingClick;
		InputController.Instance.OnTileClicked -= OnTargetingClick;

		//Debug.Log("found target tile");
		if (onTargetFound != null)
		{
			onTargetFound(grid.IsVisible(currentTile, tile) ? tile : null);
		}
	}


	// MOVING

	protected bool MakeMove(GridTileView nextTile)
	{
		if (nextTile != null && nextTile.Type.ContainsType(TileType.Walkeable) && nextTile.IsNeighbour(currentTile))
		{
			DisableInput();
			moveAction.SetMoveTarget(nextTile);

			if (!ExecuteAction(moveAction))
			{
				FinishTurn();
			}

			return true;
		}

		return false;
	}

	private IEnumerator MoveRoutine(GridTileView nextTile)
	{
		yield return StartCoroutine(MoveToTileRoutine(nextTile));
		FinishTurn();
	}

	// remember current path
	private Queue<GridTileView> currentPath;

	private void SetNewPath(IEnumerable<GridTileView> path)
	{
		currentPath = new Queue<GridTileView>();
		foreach (GridTileView tile in path)
		{
			currentPath.Enqueue(tile);
		}

		currentPath.Dequeue(); // temp removing current element

		if (currentPath.Count > 0)
		{
			MakeMove(currentPath.Dequeue());
		}
	}

	// INPUT

	private void EnableInput()
	{
		actionPanel.gameObject.SetActive(true);
		InputController.Instance.OnActorClicked += ActorClicked;
		InputController.Instance.OnTileClicked += TileClicked;
	}

	private void DisableInput()
	{
		actionPanel.gameObject.SetActive(false);
		InputController.Instance.OnActorClicked -= ActorClicked;
		InputController.Instance.OnTileClicked -= TileClicked;
	}
}
