using UnityEngine;
using System.Collections;

public class MoveAction : BaseGameAction 
{
	[SerializeField] private int movementCost;

	public GridTile TargetTile;

	private Directions nextDir = Directions.NONE;

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
			nextDir = Directions.NONE;
		}
	}

	public override void ExecuteAction (System.Action onActionComplete, System.Action onActionCancelled)
	{
		base.ExecuteAction (onActionComplete, onActionCancelled);

		// keep on moving until the target position is reached

		if(nextDir != Directions.NONE)
		{
			StartCoroutine(MoveTileRoutine(nextDir));
		}
		else
		{
			ActionCancelled();
		}

	}

	private IEnumerator MoveTileRoutine(Directions dir)
	{
		// Do the base move here
		yield return StartCoroutine(actor.MoveToTileRoutine(actor.CurrentTile.GetNeigbour(dir)));
		ActionComplete();
	}

	public override void EndTurn ()
	{
		base.EndTurn ();
	}
}
