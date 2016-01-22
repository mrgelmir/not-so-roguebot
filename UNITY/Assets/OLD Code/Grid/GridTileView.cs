using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DungeonGeneration;
using GridActorSystem;
using DungeonGeneration.Model;

[SelectionBase]
public class GridTileView : MonoBehaviour, IPathFindeable
{
	[SerializeField] private TileType type = TileType.NONE;
	[SerializeField] private int row;
	[SerializeField] private int column;
	[SerializeField] private GameObject visual;
    [SerializeField] private GridEntity actor;

	[SerializeField] private GridController grid;

	public TileType Type { get { return type; } }
	public int Row { get { return row; } }
    public int Column { get { return column; } }
    public GridEntity Actor { get { return actor; } }

	public GridTileView Left { get { return grid.GetNeighbour(this, GridDirection.West); } }
	public GridTileView Right { get { return grid.GetNeighbour(this, GridDirection.East); } }
	public GridTileView Top { get { return grid.GetNeighbour(this, GridDirection.North); } }
	public GridTileView Bottom { get { return grid.GetNeighbour(this, GridDirection.South); } }	
	public GridTileView TopRight { get { return grid.GetNeighbour(this, GridDirection.North | GridDirection.East); } }
	public GridTileView TopLeft { get { return grid.GetNeighbour(this, GridDirection.North | GridDirection.West); } }
	public GridTileView BottomRight { get { return grid.GetNeighbour(this, GridDirection.South | GridDirection.East); } }
	public GridTileView BottomLeft { get { return grid.GetNeighbour(this, GridDirection.South | GridDirection.West); } }

	private bool isTaken = false;
	public bool IsTaken{ get { return isTaken; } }

	public DungeonPosition Position
	{
		get
		{
			return new DungeonPosition(column, row);
		}
		set
		{
			column = value.Column;
			row = value.Row;
		}
	}

	private int uniqueIndex = -1;

	public bool IsNeighbour(GridTileView el)
	{
		if(el == null)
			return false;

		bool isColNeighbour = el.column == column+1 || el.column == column-1 || el.column == column;
		bool isRowNeighbour = el.row == row+1 || el.row == row-1 || el.row == row;
		bool isSelf = el.column == column && el.row == row;
		
		return !isSelf && isColNeighbour && isRowNeighbour;
	}

	public GridTileView GetNeighbour(GridDirection dir)
	{
		return grid.GetNeighbour(this, dir);
	}

	public GridTileView[] GetNeighbours()
	{
		return grid.GetNeighbours(this);
	}

	public void SetData( GridController grid, int column, int row)
	{
		this.grid = grid;
		this.column = column;
		this.row = row;

		// generate this index for pathfinding purposes
		uniqueIndex = column * (grid.Rows + 1) + row;
	}

	public virtual void UpdateVisual()
	{
		transform.localPosition = new Vector3(column * GridController.GridSpacing, 0f, row * GridController.GridSpacing);
		gameObject.name = column + "-" + row + "|" + type.ToString();
	}

	public virtual void OnEnterTile(GridEntity actor)
	{
        this.actor = actor;
		isTaken = true;
	}

	public virtual void OnLeaveTile()
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

	protected void OnDrawGizmosSelected()
	{
		Color gizmoColor = Color.white;
		switch (type)
		{
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
		Gizmos.DrawSphere(transform.position, .3f);
	}

	public static GridDirection GetDirection(GridTileView from, GridTileView to)
	{
		GridDirection horizontal = GridDirection.None;
		GridDirection vertical = GridDirection.None;

		if(from.Column < to.Column)
		{
			horizontal = GridDirection.East;
		}
		else if (from.Column > to.Column)
		{
			horizontal = GridDirection.West;
		}

		if(from.Row < to.Row)
		{
			vertical = GridDirection.North;
		}
		else if (from.Row > to.Row)
		{
			vertical = GridDirection.South;
		}

		return horizontal | vertical;
	}

	// IPathfindeable implementation

	public int HeuristicDistance(IPathFindeable other)
	{
		GridTileView otherTile = other as GridTileView;
		if (otherTile == null)
			return int.MaxValue;

		return Mathf.Abs(Column - otherTile.Column) + Mathf.Abs(Row - otherTile.Row);
	}

	private const int straightMovementCost = 10;
	private const int diagonalMovementCost = 15;

	public int MovementCostFrom(IPathFindeable other)
	{
		// check if not Neighbours 
		GridTileView otherTile = other as GridTileView;
		if (otherTile == null || !IsNeighbour(otherTile))
			return int.MaxValue;
		
		return ((column == otherTile.column)^(row == otherTile.row)) ? straightMovementCost : diagonalMovementCost;
	}

	public IEnumerable<IPathFindeable> Neighbours
	{
		get
		{
			List<IPathFindeable> neighbours = new System.Collections.Generic.List<IPathFindeable>(8);
			foreach (GridTileView neighbour in GetNeighbours())
			{
				if (neighbour != null)
					neighbours.Add(neighbour);
			}
			return neighbours;
		}
	}

	public override string ToString()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("Tile col:");
		sb.Append(Column);
		sb.Append(" row:");
		sb.Append(Row);

		// add type?

		return sb.ToString();
	}

	public bool Walkeable { get { return type.ContainsType(TileType.Walkeable); } }
	public int UniqueIndex { get { return uniqueIndex; } }

}