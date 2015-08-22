using UnityEngine;
using System.Collections;
using GridUtils;

public class MoveAction : BaseGameAction 
{
	[SerializeField] private int movementCost;

	[HideInInspector]
	public GridTile TargetTile;

	private Direction nextDir = Direction.NONE;

	public virtual void SetMoveTarget(GridTile target)
	{
		if(target == null)
			return;

		TargetTile = target;

		 // TODO get path

		// temp implementation
		// -> if neigbouring tile do the move
		// -> else cancel

		if(target.IsNeighbour(actor.CurrentTile))
		{
			nextDir = GridTile.GetDirection(actor.CurrentTile, target);
		}
		else
		{
			nextDir = Direction.NONE;
		}
	}

	public override void ExecuteAction (System.Action onActionComplete, System.Action onActionCancelled)
	{
		base.ExecuteAction (onActionComplete, onActionCancelled);

		// keep on moving until the target position is reached

		if(nextDir != Direction.NONE)
		{
			StartCoroutine(MoveTileRoutine(nextDir));
		}
		else
		{
			ActionCancelled();
		}

	}

	protected virtual IEnumerator MoveTileRoutine(Direction dir)
	{
		// Do the base move here
		yield return StartCoroutine(actor.MoveToTileRoutine(actor.CurrentTile.GetNeighbour(dir)));
		ActionComplete();
	}

	public override void EndTurn ()
	{
		base.EndTurn ();
	}
}
