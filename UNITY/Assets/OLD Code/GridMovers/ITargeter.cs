using UnityEngine;
using System;

interface ITargeter
{
    void RequestTargetTile(System.Action<GridTileView> onTargetFound);
    void RequestTargetNeigbourTile(System.Action<GridTileView> onTargetFound);

	// TODO add a cancel function
}
