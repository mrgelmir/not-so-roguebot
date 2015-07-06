﻿using UnityEngine;
using System.Collections;

[SelectionBase]
public class GridItem : MonoBehaviour
{
	protected GridTile currentTile;
	public GridTile CurrentTile { get { return currentTile; } }

    public int HitPoints;

    public System.Action<GridItem> OnDestroyed;

	public bool SetTile(GridTile tile)
	{
		if(EnterTile(tile))
		{
			transform.position = currentTile.transform.position;
			return true;
		}
		return false;
	}

	protected bool EnterTile(GridTile tile)
	{
		if(tile != null && !tile.IsTaken)
		{
			LeaveTile();
			currentTile = tile;
			tile.OnEnterTile(this);
			return true;
		}
		return false;
	}

	protected void LeaveTile()
	{
		if(currentTile != null)
		{
			currentTile.OnLeaveTile();
			currentTile = null;
		}
	}

    // DAMAGE

    public virtual void Damage(GridItem item, int damage)
    {
        item.TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        if ((HitPoints -= damage) <= 0)
        {
            ActorDestroyed();
        }
    }

    public virtual void ActorDestroyed()
    {
        // destruction animation

        LeaveTile();
        Destroy(gameObject);

        if (OnDestroyed != null)
        {
            OnDestroyed(this);
        }
    }

    // HELPER FUNCTIONS

    public GridTile RandomFreeNeighbour()
    {
        // brute force this shit
        GridTile randomTile = null;
        int counter = 0;
        do
        {
            randomTile = RandomNeighbour();
            ++counter;
        }
        while (counter < 20 && (randomTile.IsTaken || randomTile.Type != TileType.Walkeable));

        if (randomTile.IsTaken || randomTile.Type != TileType.Walkeable)
        {
            randomTile = null;
        }

        return randomTile;
    }

    public GridTile NeigbourForDirection(Directions dir)
    {
        // TODO fix
        GridTile tile = null;

        switch (dir)
        {
            case Directions.Up:
                tile = currentTile.Top;
                break;
            case Directions.Up | Directions.Right:
                tile = currentTile.TopRight;
                break;
            case Directions.Right:
                tile = currentTile.Right;
                break;
            case Directions.Right | Directions.Down:
                tile = currentTile.BottomRight;
                break;
            case Directions.Down:
                tile = currentTile.Bottom;
                break;
            case Directions.Down | Directions.Left:
                tile = currentTile.BottomLeft;
                break;
            case Directions.Left:
                tile = currentTile.Left;
                break;
            case Directions.Left | Directions.Up:
                tile = currentTile.TopLeft;
                break;
        }
        return null;
    }

    public GridTile RandomNeighbour()
    {
        switch (Random.Range(0, 8))
        {
            default:
            case 0:
                return currentTile.Top;
            case 1:
                return currentTile.TopRight;
            case 2:
                return currentTile.Right;
            case 3:
                return currentTile.BottomRight;
            case 4:
                return currentTile.Bottom;
            case 5:
                return currentTile.BottomLeft;
            case 6:
                return currentTile.Left;
            case 7:
                return currentTile.TopLeft;
        }
    }

}
