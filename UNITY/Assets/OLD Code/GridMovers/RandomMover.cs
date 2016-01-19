using UnityEngine;
using System.Collections;

public class RandomMover : GridActor
{

	private int possibleAttempts = 5;
	private int attemptCounter = 0;

	public override void Setup ()
	{
		base.Setup ();
		SetTile(grid.RandomFreeTile);
	}

	public override void StartTurn (System.Action<GridActor> done)
	{
		base.StartTurn (done);
		attemptCounter = 0;
	}

	protected override void ProgressTurn ()
	{
		if(++attemptCounter > possibleAttempts)
		{
			FinishTurn();
		}

		GridTileView nextTile = RandomFreeNeighbour();
		moveAction.SetMoveTarget(nextTile);
        ExecuteAction(moveAction);

	}
}
