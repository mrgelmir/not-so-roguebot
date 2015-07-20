using UnityEngine;
using System.Collections;

public class DoorTile : GridTile
{
	[SerializeField]
	private Transform DoorVisual;

	[SerializeField]
	private float VerticalOffset = 1f;

	public override void OnEnterTile(GridItem actor)
	{
		base.OnEnterTile(actor);
		DoorVisual.localPosition = Vector3.up * VerticalOffset;
	}

	public override void OnLeaveTile()
	{
		base.OnLeaveTile();
		DoorVisual.localPosition = Vector3.zero;
	}
}
