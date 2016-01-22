using DungeonGeneration;
using DungeonGeneration.Generation;
using DungeonGeneration.Model;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{

	[SerializeField] private TileGenerator generator;

	public static readonly float GridSpacing = 1f;

	[SerializeField, HideInInspector]
	private GridReference gridData;

	private List<List<GridTileView>> gridElements { get { return gridData.GridElements; } }
	public int Columns { get { return gridElements.Count; } }
	public int Rows { get { return gridElements.Count > 0 ? gridElements[0].Count : 0; } } 
	
	[SerializeField] private GridTileView startTile = null;
	public GridTileView StartTile 
	{ 
		get 
		{ 
			if(startTile == null)
				startTile = RandomFreeTile;

			return startTile;  
		} 
	}

	public GridTileView RandomFreeTile
	{ 
		get
		{
			List<GridTileView> freeTiles = new List<GridTileView>();

			foreach (List<GridTileView> col in gridElements)
			{
				foreach (GridTileView el in col)
				{
					if (el.Type.ContainsType(TileType.Walkeable) && !el.IsTaken)
						freeTiles.Add(el);
				}
			}

			if (startTile != null)
				freeTiles.Remove(StartTile);

			if (freeTiles.Count > 0)
				return freeTiles[Random.Range(0, freeTiles.Count)];
			else
				return null;
		}
	}

	private GridTileView CreateGridElement(TileType type, int col, int row)
	{
		return CreateGridElement(type, TileOrientation.NONE, col, row);
	}

	private GridTileView CreateGridElement(TileType type, TileOrientation orientation, int col, int row)
	{
		// get correct element from generator
		GridTileView el = generator.GetElement(type, orientation);

		if(el == null)
		{
			return null;
		}

		// initialize element
		el.transform.SetParent(transform);
		//el.SetData(this, col, row);
		//el.UpdateVisual();

		return el;
	}

	public GridTileView GetNeighbour(GridTileView el, GridDirection dir)
	{
		int neighbourRow = el.Row;
		int neighbourCol = el.Column;

		// get actual neighbour position
		if ((dir & GridDirection.West) == GridDirection.West)
			--neighbourCol;
		else if ((dir & GridDirection.East) == GridDirection.East)
			++neighbourCol;

		if ((dir & GridDirection.North) == GridDirection.North)
			++neighbourRow;
		else if ((dir & GridDirection.South) == GridDirection.South)
			--neighbourRow;

		// clamp
		neighbourCol = Mathf.Clamp(neighbourCol, 0, Columns - 1);
		neighbourRow = Mathf.Clamp(neighbourRow, 0, Rows - 1);

		return gridElements[neighbourCol][neighbourRow];
	}

	public GridTileView[] GetNeighbours(GridTileView el)
	{
		GridTileView[] neighbours = new GridTileView[9];

		for (int column = -1; column <= 1; column++)
		{
			for (int row = -1; row <= 1; row++)
			{
				int neighbourColumn = el.Column + column;
				int neighbourRow = el.Row + row;
				if (neighbourColumn > 0 && neighbourColumn < Columns && neighbourRow > 0 && neighbourRow < Rows && !(column == 0 && row == 0))
				{
					int index = (column + 1) * 3 + (row + 1);
					neighbours[index] = gridElements[neighbourColumn][neighbourRow];
				}
			}
		}

		return neighbours;
	}

	public GridTileView ClosestTileToPoint(Vector3 point)
	{
		// TODO find more elegant way to do this (probably using tile spacing)
		GridTileView closestTile = null;
		float shortestSquaredDistance = float.MaxValue;
		float tempSqrDist = 0f;

		foreach (var col in gridElements)
		{
			foreach (var el in col)
			{
				tempSqrDist = Vector3.SqrMagnitude(point - el.transform.position);
				if (shortestSquaredDistance > tempSqrDist)
				{
					closestTile = el;
					shortestSquaredDistance = tempSqrDist;
				}
			}
		}

		// clicking outside of the grid
		if (shortestSquaredDistance > GridSpacing * GridSpacing)
		{
			print("clicked outside of the grid");
			closestTile = null;
		}

		return closestTile;
	}

	[ContextMenu("Clear Grid")]
	public void ClearGrid()
	{
		foreach (List<GridTileView> col in gridElements)
		{
			foreach (GridTileView el in col)
			{
				if (el == null)
					continue;

				if (Application.isPlaying)
					Destroy(el.gameObject);
				else
					DestroyImmediate(el.gameObject);
			}
		}

		gridElements.Clear();
		startTile = null;
	}

	public bool IsVisible(GridTileView start, GridTileView end)
	{
		Log.Write("getting visibility between " + start + " and " + end);

		// temp
		GetLine(start, end);

		// visible until proven invisible
		bool inVisible = false;

		// iterate over tiles until a blocker is found
		IterateGridLine(start.Position, end.Position, (int column, int row) =>
		{
			// add other types/cases here
			return inVisible = gridElements[column][row].Type.ContainsType(TileType.SightBlocker);
		});

		return !inVisible;
	}

	public IList<GridTileView> GetLine(GridTileView start, GridTileView end)
	{

		// get total line lenght (duplicated from IterateGridLine =/)
		int xDist = Mathf.Abs(end.Column - start.Column);
		int yDist = Mathf.Abs(end.Row - start.Row);
		int tileCount = 1 + xDist + yDist;

		// initialize list with correct amount of elements
		List<GridTileView> lineTiles = new List<GridTileView>(tileCount);

		IterateGridLine(start.Position, end.Position, (int column, int row) =>
		{
			lineTiles.Add(gridElements[column][row]);
			return false;
		});

		return lineTiles;
	}

	private delegate bool VisitTile(int column, int row);

	private void IterateGridLine(DungeonPosition from, DungeonPosition to, VisitTile onVisit)
	{
		// TODO simpler horizontal/vertical implementation here?

		// get the total distance
		int xDist = Mathf.Abs(to.Column - from.Column);
		int yDist = Mathf.Abs(to.Row - from.Row);

		// get total line lenght
		int tileCount = 1 + xDist + yDist;

		// start position to iterate from
		int x = from.Column;
		int y = from.Row;

		// the column and row direction 
		int xStep = (to.Column > from.Column) ? 1 : -1;
		int yStep = (to.Row > from.Row) ? 1 : -1;

		int error = xDist - yDist;

		// adjust to allow pure integer math
		xDist *= 2;
		yDist *= 2;

		for (; tileCount > 0; --tileCount)
		{
			// if onVisit returns true, we no longer need to continue
			if (onVisit(x, y))
				break;

			if (error > 0)
			{
				x += xStep;
				error -= yDist;
			}
			else
			{
				y += yStep;
				error += xDist;
			}
		}
	}

	public void DrawGrid(GridData data)
	{
		
		foreach (TileData tile in data)
		{
			gridElements[tile.Column][tile.Row] = CreateGridElement(ConvertType(tile.Type), tile.Column, tile.Row);
		}
	}

	public void DrawGrid(DungeonGenerationData data)
	{
		ClearGrid();

		// 1. create empty grid
		gridData.MakeEmptyGrid(data.Columns, data.Rows);

		// 2. add all rooms
		foreach (DungeonRoom room in data.Rooms)
		{
			AddRoom(room);
		}

		// 3. add all corridors
		foreach (DungeonCorridor corridor in data.Corridors)
		{
			AddCorridor(corridor);
		}

		// 4. make all null tiles empty tiles
		FillEmptyTiles();

	}

	private void AddRoom(DungeonRoom room)
    {
        // TODO get room type/data for special rooms and different tilesets


        if (room.Row >= 0 && room.Column >= 0 && (room.Column + room.Width) < Columns && (room.Row + room.Height) < Rows)
        {
            // add the room here

            for (int c = 0; c < room.Width; c++)
            {
                for (int r = 0; r < room.Height; r++)
                {
                    DungeonTile tile = room.Tiles[c][r];
                    if(tile == null || tile.Type == DungeonTileType.None)
                        continue;

					TileOrientation verticalOrientation;
					if (c <= 0)
					{
						verticalOrientation = TileOrientation.Left;
					}
					else if (c >= room.Width - 1)
					{
						verticalOrientation = TileOrientation.Right;
					}
					else
					{
						verticalOrientation = TileOrientation.NONE;
					}

					TileOrientation horizontalOrientation;
					if (r <= 0)
					{
						horizontalOrientation = TileOrientation.Bottom;
					}
					else if (r >= room.Height - 1)
					{
						horizontalOrientation = TileOrientation.Top;
					}
					else
					{
						horizontalOrientation = TileOrientation.NONE;
					}

                    // TODO check for existing tiles
                    gridElements[room.Column + c][room.Row + r] = CreateGridElement(ConvertType(tile.Type), verticalOrientation|horizontalOrientation, room.Column + c, room.Row + r);
                }
            }
        }

		foreach (DungeonPosition doorTile in room.Doors)
		{
			if(gridElements[doorTile.Column][doorTile.Row] == null)
			{
				gridElements[doorTile.Column][doorTile.Row] = CreateGridElement(TileType.Door, doorTile.Column, doorTile.Row);
			}
		}
    }

    private void AddCorridor(DungeonCorridor corridor)
    {
		// TODO out of grid checking

		foreach (DungeonPosition pos in corridor.TilePositions)
		{
			if (pos.Column < gridElements.Count && pos.Row < gridElements[0].Count && gridElements[pos.Column][pos.Row] == null)
				gridElements[pos.Column][pos.Row] = CreateGridElement(TileType.Walkeable, pos.Column, pos.Row);
		}
    }

    private void FillEmptyTiles()
    {
        for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r < Rows; r++)
			{
                if (gridElements[c][r] == null)
				{
                    gridElements[c][r] = CreateGridElement(GetFillTileType(c,r), c, r);
				}
            }
        }
    }

	private TileType GetFillTileType(int c, int r)
	{
		GridDirection dir = GridDirection.North;
		for (int numRotations = 0; numRotations < 8; numRotations++)
		{
			GridTileView neighbourTile = gridElements[Mathf.Clamp(c + dir.GetHorizontalDirection(), 0, Columns - 1)][Mathf.Clamp(r + dir.GetVerticalDirection(), 0, Rows - 1)];

			if (neighbourTile != null && neighbourTile.Type != TileType.NONE && neighbourTile.Type != TileType.SightBlocker)
			{
				return TileType.SightBlocker;
			}

			dir = dir.RotateBy(45);
		}

		return TileType.NONE;
	}

    private TileType ConvertType(DungeonTileType type)
    {
        TileType tileType;

        switch (type)
        {
            default:
            case DungeonTileType.None:
                tileType = TileType.NONE;
                break;
            case DungeonTileType.Flat:
                tileType = TileType.Walkeable;
				break;
            case DungeonTileType.Door:
                tileType = TileType.Door;
                break;
            case DungeonTileType.Wall:
                tileType = TileType.SightBlocker;
				break;
			case DungeonTileType.Target:
				tileType = TileType.Target;
				break;
		}

        return tileType;
    }
    
	private bool IsBorderTile(int col, int row)
	{
		return col == 0 || col == Columns-1 || row == 0 || row == Rows-1;
	}

	private void Swap<T>(ref T a, ref T b)
	{
		T c = a;
		a = b;
		b = c;
	}

}