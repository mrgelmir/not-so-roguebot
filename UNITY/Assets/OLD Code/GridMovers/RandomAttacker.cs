using UnityEngine;
using System.Collections;

public class RandomAttacker : GridActor, ITargeter
{
	[SerializeField] 
	private MeleeAction meleeAction;

	private int possibleAttempts = 5;
	private int attemptCounter = 0;

	public override void Setup ()
	{
		base.Setup ();
		SetTile(grid.RandomFreeTile);

		meleeAction.Init(this);
	}

	public override void StartTurn (System.Action<GridActor> done)
	{
		base.StartTurn (done);
		attemptCounter = 0;
	}

	protected override void ProgressTurn ()
	{
		GridTileView t = null;
		foreach (GridTileView tile in currentTile.Neighbours)
		{
			//if (tile.Actor as PlayerMover != null)
			//{
			//	t = tile;
			//	break;
			//}
		}

		if (t != null) // attack neigbouring actor
		{
			ExecuteAction(meleeAction);
		}
		else // do a random move
		{
			if (++attemptCounter > possibleAttempts)
			{
				FinishTurn();
			}

			GridTileView nextTile = RandomFreeNeighbour();
			moveAction.SetMoveTarget(nextTile);
			ExecuteAction(moveAction);
		}

		

	}

	public void RequestTargetTile(System.Action<GridTileView> onTargetFound)
	{
		RequestTargetNeigbourTile(onTargetFound);
	}

	public void RequestTargetNeigbourTile(System.Action<GridTileView> onTargetFound)
	{
		// check if player is besides us
		GridTileView t = null;
		foreach (GridTileView tile in currentTile.Neighbours)
		{
			t = tile;
			if (tile.Actor != null)
			{
				break;
			}
		}

		// return random neighbour tile
		onTargetFound(t);
	}
}
