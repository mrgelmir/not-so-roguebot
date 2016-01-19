using UnityEngine;
using System.Collections;
using GridUtils;

[SelectionBase]
public class GridItem : MonoBehaviour
{
	protected GridTileView currentTile;
	public GridTileView CurrentTile { get { return currentTile; } }

    public int HitPoints;

    public System.Action<GridItem> OnDestroyed;

	public bool SetTile(GridTileView tile)
	{
		if(EnterTile(tile))
		{
			transform.position = currentTile.transform.position;
			return true;
		}

		return false;
	}

	protected bool EnterTile(GridTileView tile)
	{
		if(tile != null && !tile.IsTaken)
		{
			LeaveTile();
			currentTile = tile;
			//tile.OnEnterTile(this);
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

    public GridTileView RandomFreeNeighbour()
    {
        // brute force this shit
        GridTileView randomTile = null;
        int counter = 0;
        do
        {
            randomTile = RandomNeighbour();
            ++counter;
        }
        while (counter < 20 && (randomTile.IsTaken || !randomTile.Type.ContainsType(TileType.Walkeable)));

		if (randomTile.IsTaken || !randomTile.Type.ContainsType(TileType.Walkeable))
        {
            randomTile = null;
        }

        return randomTile;
    }

    public GridTileView NeigbourForDirection(Direction dir)
    {
        // TODO fix
        GridTileView tile = null;

        switch (dir)
        {
            case Direction.Up:
                tile = currentTile.Top;
                break;
            case Direction.Up | Direction.Right:
                tile = currentTile.TopRight;
                break;
            case Direction.Right:
                tile = currentTile.Right;
                break;
            case Direction.Right | Direction.Down:
                tile = currentTile.BottomRight;
                break;
            case Direction.Down:
                tile = currentTile.Bottom;
                break;
            case Direction.Down | Direction.Left:
                tile = currentTile.BottomLeft;
                break;
            case Direction.Left:
                tile = currentTile.Left;
                break;
            case Direction.Left | Direction.Up:
                tile = currentTile.TopLeft;
                break;
        }
        return null;
    }

    public GridTileView RandomNeighbour()
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
