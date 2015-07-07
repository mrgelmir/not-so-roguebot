using UnityEngine;
using System.Collections;

[SelectionBase]
public class GridTile : MonoBehaviour
{
	[SerializeField] private TileType type = TileType.NONE;
	[SerializeField] private int row;
	[SerializeField] private int column;
	[SerializeField] private GameObject visual;
    [SerializeField] private GridItem actor;

	[SerializeField] private GridController grid;

	public TileType Type { get { return type; } }
	public int Row { get { return row; } }
    public int Column { get { return column; } }
    public GridItem Actor { get { return actor; } }

	public GridTile Left { get { return grid.GetNeighbour(this, Direction.Left); } }
	public GridTile Right { get { return grid.GetNeighbour(this, Direction.Right); } }
	public GridTile Top { get { return grid.GetNeighbour(this, Direction.Up); } }
	public GridTile Bottom { get { return grid.GetNeighbour(this, Direction.Down); } }	
	public GridTile TopRight { get { return grid.GetNeighbour(this, Direction.Up | Direction.Right); } }
	public GridTile TopLeft { get { return grid.GetNeighbour(this, Direction.Up | Direction.Left); } }
	public GridTile BottomRight { get { return grid.GetNeighbour(this, Direction.Down | Direction.Right); } }
	public GridTile BottomLeft { get { return grid.GetNeighbour(this, Direction.Down | Direction.Left); } }

	private bool isTaken = false;
	public bool IsTaken{ get { return isTaken; } }

	public bool IsNeighbour(GridTile el)
	{
		if(el == null)
			return false;

		bool isColNeighbour = el.column == column+1 || el.column == column-1 || el.column == column;
		bool isRowNeighbour = el.row == row+1 || el.row == row-1 || el.row == row;
		bool isSelf = el.column == column && el.row == row;
		
		return !isSelf && isColNeighbour && isRowNeighbour;
	}

	public GridTile GetNeigbour(Direction dir)
	{
		return grid.GetNeighbour(this, dir);
	}

	public void SetData( GridController grid, int column, int row)
	{
		this.grid = grid;
		this.column = column;
		this.row = row;
	}

	public void UpdateVisual()
	{
		transform.localPosition = new Vector3(column * GridController.GridSpacing, 0f, row * GridController.GridSpacing);
		gameObject.name = column + "-" + row + "|" + type.ToString();
	}

	public void OnEnterTile(GridItem actor)
	{
        this.actor = actor;
		isTaken = true;
	}

	public void OnLeaveTile()
    {
        this.actor = null;
		isTaken = false;
	}


	protected void OnDrawGizmos()
	{
		Color gizmoColor = Color.white;
		switch (type) {
		default:
		case TileType.NONE:
			gizmoColor = Color.white;
			break;
		case TileType.Walkeable:
			gizmoColor = Color.grey;
			break;
		case TileType.SightBlocker:
			gizmoColor = Color.black;
			break;
		}

		Gizmos.color = gizmoColor;
		Gizmos.DrawSphere(transform.position, .1f);
	}

	public static Direction GetDirection(GridTile from, GridTile to)
	{
		Direction horizontal = Direction.NONE;
		Direction vertical = Direction.NONE;

		if(from.Column < to.Column)
		{
			horizontal = Direction.Right;
		}
		else if (from.Column > to.Column)
		{
			horizontal = Direction.Left;
		}

		if(from.Row < to.Row)
		{
			vertical = Direction.Up;
		}
		else if (from.Row > to.Row)
		{
			vertical = Direction.Down;
		}

		return horizontal | vertical;
	}
}