using UnityEngine;
using System.Collections;
using GridUtils;

public class InstantMoveAction : MoveAction
{

	protected override IEnumerator MoveTileRoutine(Direction dir)
	{
		StartCoroutine(actor.MoveToTileRoutine(actor.CurrentTile.GetNeighbour(dir)));
		ActionComplete();
		yield return null;
	}
}
