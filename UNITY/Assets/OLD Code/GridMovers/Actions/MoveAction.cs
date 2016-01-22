using DungeonGeneration;
using DungeonGeneration.Model;
using System.Collections;
using UnityEngine;

public class MoveAction : BaseGameAction 
{
	[SerializeField] private int movementCost;

	[HideInInspector]
	public GridTileView TargetTile;

	private GridDirection nextDir = GridDirection.None;

	public virtual void SetMoveTarget(GridTileView target)
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
			nextDir = GridTileView.GetDirection(actor.CurrentTile, target);
		}
		else
		{
			nextDir = GridDirection.None;
		}
	}

	public override void ExecuteAction (System.Action onActionComplete, System.Action onActionCancelled)
	{
		base.ExecuteAction (onActionComplete, onActionCancelled);

		// keep on moving until the target position is reached

		if(nextDir != GridDirection.None)
		{
			StartCoroutine(MoveTileRoutine(nextDir));
		}
		else
		{
			ActionCancelled();
		}

	}

	protected virtual IEnumerator MoveTileRoutine(GridDirection dir)
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
