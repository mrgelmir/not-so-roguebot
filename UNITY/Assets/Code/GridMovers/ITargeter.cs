using UnityEngine;
using System;

interface ITargeter
{
    void RequestTargetTile(System.Action<GridTile> onTargetFound);
    void RequestTargetNeigbourTile(System.Action<GridTile> onTargetFound);
}
