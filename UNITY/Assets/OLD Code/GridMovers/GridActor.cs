using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridActor : GridItem
{
	[Header("Data")]
	public MoverData moverData;

	[Header("BookKeeping")]
	// used by the GridMoveManager
	public int InitiativeCountDown;
	public int ActionPoints;

	[Header("Local references")]
	[SerializeField] protected MoveAction moveAction;
	[SerializeField] protected List<BaseGameAction> actions;

	// scene reference
	protected GridController grid;

	// This is private, so no inheriting classes can cheat
	private bool onTurn = false;
	public bool OnTurn { get { return onTurn; } }

	[HideInInspector] public bool Remove;

	
	protected void OnEnable()
	{
		if(grid == null)
		{
			grid = FindObjectOfType<GridController>();

			if(grid == null)
				Debug.LogWarning("GridActor - No grid found", gameObject);
		}

		if(moveAction == null)
		{
			moveAction = GetComponent<MoveAction>();

			if(moveAction == null)
				Debug.LogWarning("GridActor - no move action found", gameObject);
		}

		foreach (BaseGameAction action in GetComponentsInChildren<BaseGameAction>())
		{
			if(action != moveAction && !actions.Contains(action))
			{
				actions.Add(action);
			}
		}
	}

	// BASE METHODS

	private System.Action<GridActor> OnFinishTurn;

	public virtual void Setup() // this is always called after the awake function
	{
		// Set initiative
		InitiativeCountDown = moverData.Initiative;
        HitPoints = moverData.Hitpoints;
        ActionPoints = moverData.ActionPoints;

		if(moveAction != null)
		{
			moveAction.Init(this);
		}
		foreach (BaseGameAction action in actions)
		{
			action.Init(this);
		}
	}

	public virtual void StartTurn(System.Action<GridActor> done)
    {
        ActionPoints = moverData.ActionPoints;

		OnFinishTurn = done;
		onTurn = true;
		ProgressTurn();
	}

	protected virtual void ProgressTurn()
	{
        if(ActionPoints <= 0)
        {
            FinishTurn();
        }
		// Do all the thinking in here


	}

	protected virtual void FinishTurn()
	{
        // Inform the actions of the end of turn
        moveAction.EndTurn();
        foreach (BaseGameAction action in actions)
        {
            action.EndTurn();
        }

		onTurn = false;
		if(OnFinishTurn != null)
		{
			OnFinishTurn(this);
			OnFinishTurn = null;
		}
	}
	
	public virtual void StopTurn ()
	{
		transform.position = Vector3.down * 50f;

        // TODO find out what to do
		StopAllCoroutines();
		currentTile = null;
		onTurn = false;
	}

	// ACTIONS

    // TODO find nicer way to reimburse action points
    private int lastActionCost = 0;
	protected bool ExecuteAction(BaseGameAction action)
	{
        bool hasExecuted = false;
        lastActionCost = 0;

        if(action.ActionCost <= 0)
        {
            Debug.LogWarning("GridActor::ExecuteAction - The action cost for this action is 0!");
        }

        if (ActionPoints - action.ActionCost >= 0)
        {
            lastActionCost = action.ActionCost;
            ActionPoints -= action.ActionCost;
            System.Action onComplete;
            if (ActionPoints > 0)
            {
                onComplete = ProgressTurn;
            }
            else // equals zero -> end turn
            {
                onComplete = FinishTurn;
            }
            action.ExecuteAction(onComplete, ActionCancelled);
            hasExecuted = true;
        }
        else
        {
            Debug.Log("GridActor::ExecuteAction - Not enough Action points to execute action : " + action.Data.ActionName);
            ActionCancelled();
        }

        return hasExecuted;
	}

    private void ActionCancelled()
    {
        ActionPoints += lastActionCost;
        lastActionCost = 0;
        ProgressTurn();
    }

	// MOVEMENT


	// TODO move this to the move action?
	public IEnumerator MoveToTileRoutine(GridTileView el)
	{
		if(!EnterTile(el))
		{
			yield break;
		}

		onTurn = false;
		Vector3 start = transform.position;
		Vector3 end = el.transform.position;

		Debug.DrawLine(start, end, Color.blue, 1f);

		for(float p = 0f; p <= 1f; p += Time.deltaTime/.2f)
		{
			transform.position = Vector3.Lerp(start, end, p);
			yield return null;
		}

		transform.position = end;
		onTurn = true;
	}
}
