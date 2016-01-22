using UnityEngine;
using System.Collections;
using DungeonGeneration;

public class InstantMoveAction : MoveAction
{

	protected override IEnumerator MoveTileRoutine(GridDirection dir)
	{
		StartCoroutine(actor.MoveToTileRoutine(actor.CurrentTile.GetNeighbour(dir)));
		ActionComplete();
		yield return null;
	}
}
