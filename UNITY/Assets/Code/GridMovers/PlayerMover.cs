using UnityEngine;
using System.Collections;

public class PlayerMover : GridActor, ITargeter
{
    [SerializeField] private MeleeAction meleeAction;
    [SerializeField] private ActionPanel actionPanel;

	// IMPLEMENTATION

	public override void Setup ()
	{
		base.Setup ();
		SetTile(grid.StartTile);

        meleeAction.Init(this);
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

	protected override void ProgressTurn ()
	{
        base.ProgressTurn();
		//enable input 
		Enableinput ();
	}

    protected override void FinishTurn()
    {
        base.FinishTurn();

        actionPanel.Reset();
        DisableInput();
    }

	// MESSAGES

	void ActorClicked (GridActor actor)
	{
		// Temp, move to seperate behaviour
		if(actor.CurrentTile.IsNeighbour(currentTile))
		{
            DisableInput();

            meleeAction.SetTarget(actor.CurrentTile);
            ExecuteAction(meleeAction);
		}
	}

	void TileClicked (GridTile tile)
	{
		MakeMove(tile);
	}

    private void ActionButtonClicked(BaseGameAction action)
    {
        DisableInput();
        ExecuteAction(action);
    }


    //Targeting
    System.Action<GridTile> onTargetFound = null;
    public void RequestTargetNeigbourTile(System.Action<GridTile> onTargetFound)
    {
        Debug.Log("PlayerMover - Requesting neigbour target");
        //TODO check meigbouring tiles first: If only one possible target is found -> attack that one

        this.onTargetFound = onTargetFound;

        InputController.Instance.OnActorClicked += OnTargetingClick;
        InputController.Instance.OnTileClicked += OnTargetingClick;
    }

    public void RequestTargetTile(System.Action<GridTile> onTargetFound)
    {
        Debug.Log("PlayerMover - Requesting target");
        this.onTargetFound = onTargetFound;

        InputController.Instance.OnActorClicked += OnTargetingClick;
        InputController.Instance.OnTileClicked += OnTargetingClick;
    }

    private void OnTargetingClick(GridActor actor)
    {
        OnTargetingClick(actor.CurrentTile);
    }

    private void OnTargetingClick(GridTile tile)
    {
        InputController.Instance.OnActorClicked -= OnTargetingClick;
        InputController.Instance.OnTileClicked -= OnTargetingClick;

        if(onTargetFound != null)
        {
            Debug.Log("found target");
            onTargetFound(tile);
        }
    }

	
	// MOVING

	protected void MakeMove( GridTile nextTile )
	{

		if (nextTile != null && nextTile.Type.ContainsType(TileType.Walkeable) && nextTile.IsNeighbour(currentTile))
		{
			DisableInput();
			moveAction.SetMoveTarget(nextTile);

            if(!ExecuteAction(moveAction))
            {
                FinishTurn();
            }
		}
	}

	private IEnumerator MoveRoutine(GridTile nextTile)
	{
		yield return StartCoroutine (MoveToTileRoutine (nextTile));
		FinishTurn ();
	}

	// INPUT

	private void Enableinput ()
	{
        actionPanel.gameObject.SetActive(true);
		InputController.Instance.OnActorClicked += ActorClicked;
		InputController.Instance.OnTileClicked += TileClicked;
	}

	private void DisableInput ()
	{
        actionPanel.gameObject.SetActive(false);
		InputController.Instance.OnActorClicked -= ActorClicked;
		InputController.Instance.OnTileClicked -= TileClicked;
	}
}
