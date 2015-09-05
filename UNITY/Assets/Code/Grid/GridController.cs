using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;
using GridUtils;

public class GridController : MonoBehaviour
{

	[SerializeField] private TileGenerator generator;

	public static readonly float GridSpacing = 1f;

	[SerializeField, HideInInspector] private GridData gridData;

	private List<List<GridTile>> gridElements { get { return gridData.GridElements; } }
	public int Columns { get { return gridData.Columns; } }
	public int Rows { get { return gridData.Rows; } } 
	
	[SerializeField] private GridTile startTile = null;
	public GridTile StartTile 
	{ 
		get 
		{ 
			if(startTile == null)
				startTile = RandomFreeTile;

			return startTile;  
		} 
	}

	public GridTile RandomFreeTile 
	{ 
		get 
		{ 
			List<GridTile> freeTiles = new List<GridTile>();

			foreach (List<GridTile> col in gridElements)
			{
				foreach (GridTile el in col)
				{
					if(el.Type.ContainsType(TileType.Walkeable) && !el.IsTaken)
						freeTiles.Add(el);
				}
			}

			if(startTile != null)
				freeTiles.Remove(StartTile);

			if(freeTiles.Count > 0)
				return freeTiles[Random.Range(0, freeTiles.Count)];
			else
				return null;
		} 
	}

	private GridTile CreateGridElement(TileType type, int col, int row)
	{
		return CreateGridElement(type, TileOrientation.NONE, col, row);
	}
	private GridTile CreateGridElement(TileType type, TileOrientation orientation, int col, int row)
	{
		// get correct element from generator
		GridTile el = generator.GetElement(type, orientation);

		// initialize element
		el.transform.SetParent(transform);
		el.SetData(this, col, row);
		el.UpdateVisual();

		return el;
	}

	public GridTile GetNeighbour(GridTile el, Direction dir)
	{
		int neighbourRow = el.Row;
		int neighbourCol = el.Column;

		// get actual neighbour position
		if ((dir & Direction.Left) == Direction.Left)
			-- neighbourCol;
		else if ((dir & Direction.Right) == Direction.Right)
			++ neighbourCol;

		if ((dir & Direction.Up) == Direction.Up)
			++ neighbourRow;
		else if ((dir & Direction.Down) == Direction.Down)
			-- neighbourRow;

		// clamp
		neighbourCol = Mathf.Clamp(neighbourCol, 0, Columns-1);
		neighbourRow = Mathf.Clamp(neighbourRow, 0, Rows-1);

		return gridElements[neighbourCol][neighbourRow];
	}

	public GridTile[] GetNeighbours(GridTile el)
	{
		GridTile[] neighbours = new GridTile[9];

		for (int column = -1; column <= 1; column++)
		{
			for (int row = -1; row <= 1; row++)
			{
				int neighbourColumn = el.Column + column;
				int neighbourRow = el.Row + row;
				if(neighbourColumn > 0 && neighbourColumn < Columns && neighbourRow > 0 && neighbourRow < Rows && !( column == 0 && row == 0 ) )
				{
					int index = (column + 1) * 3 + (row + 1);
					neighbours[index] = gridElements[neighbourColumn][neighbourRow];
				}
			}
		}

		return neighbours;
	}

	public GridTile ClosestTileToPoint(Vector3 point)
	{
		// TODO find more elegant way to do this (probably using tile spacing)
		GridTile closestTile = null;
		float shortestSquaredDistance = float.MaxValue;
		float tempSqrDist = 0f;

		foreach (var col in gridElements) 
		{
			foreach (var el in col) 
			{
				tempSqrDist = Vector3.SqrMagnitude(point - el.transform.position);
				if(shortestSquaredDistance > tempSqrDist)
				{
					closestTile = el;
					shortestSquaredDistance = tempSqrDist;
				}
			}
		}

        // clicking outside of the grid
        if(shortestSquaredDistance > GridSpacing*GridSpacing)
        {
            print("clicked outside of the grid");
            closestTile = null;
        }
        
        return closestTile;
	}

	public void ClearGrid()
	{
		foreach (List<GridTile> col in gridElements) 
		{
			foreach (GridTile el in col)
			{
				if(el == null)
					continue;

				if(Application.isPlaying)
					Destroy(el.gameObject);
				else
					DestroyImmediate(el.gameObject);
			}
		}

		gridElements.Clear();
		startTile = null;
	}

	public bool IsVisible(GridTile start, GridTile end)
	{
		Log.Write("getting visibility between " + start + " and " + end);

		// get line of all tiles between start and end
		var tiles = GetBresenhamLine2(start, end);
		//var tiles = GetBresenhamLine(start, end);

		GetSanderLine(start, end);

		for (int i = 0; i < tiles.Count; i++)
		{
			if (tiles[i].Type.ContainsType(TileType.SightBlocker)) // check for other visibility blockers
			{
				Log.Write("This tile blocks the visibility: " + tiles[i].ToString(), tiles[i]);
				return false;
			}
		}
		return true;
	}

	public IList<GridTile> GetBresenhamLine(GridTile start, GridTile end)
	{
		// TOOD maybe use the supercover line algorithm for corner cases? 
		// http://lifc.univ-fcomte.fr/home/~ededu/projects/bresenham/
		// or use two / three lines to check


		int col0 = start.Column;
		int row0 = start.Row;
		int col1 = end.Column;
		int row1 = end.Row;

		int expectedCount = 1 + Mathf.Max(Mathf.Abs(col0 - col1), Mathf.Abs(row0 - row1));
		Log.Write("expected count: " + expectedCount);
		List<GridTile> finalLine = new List<GridTile>(expectedCount);

		bool isSteep = Mathf.Abs(row0 - row1) > Mathf.Abs(col1 - col0);
		if (isSteep)
		{
			Swap(ref col0, ref row0);
			Swap(ref col1, ref row1);
		}
		if (col0 > col1)
		{
			Swap(ref col0, ref col1);
			Swap(ref row0, ref row1);
		}

		int deltaX = col1 - col0;
		int deltaY = Mathf.Abs(row1 - row0);
		int error = 0;
		int y = row0;
		int ystep = (row0 < row1) ? 1 : -1;

		for (int x = col0; x <= col1; x++)
		{
			if (isSteep)
			{
				finalLine.Add(gridElements[y][x]);
			}
			else
			{
				finalLine.Add(gridElements[x][y]);
			}
			error += deltaY;
			if (2 * error >= deltaX)
			{
				y += ystep;
				error -= deltaX;
			}
		}
		Log.Write("final count = " + finalLine.Count);
		return finalLine;
	}

	public IList<GridTile> GetBresenhamLine2(GridTile start, GridTile end)
	{

		int x = start.Column;
		int y = start.Row;
		int x2 = end.Column;
		int y2 = end.Row;

		List<GridTile> finalLine = new List<GridTile>();

		int w = x2 - x;
		int h = y2 - y;
		int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
		if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
		if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
		if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
		int longest = Mathf.Abs(w);
		int shortest = Mathf.Abs(h);
		if (!(longest > shortest))
		{
			longest = Mathf.Abs(h);
			shortest = Mathf.Abs(w);
			if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
			dx2 = 0;
		}
		int numerator = longest >> 1;
		for (int i = 0; i <= longest; i++)
		{
			//putpixel(x, y, color);

			finalLine.Add(gridElements[x][y]);

			numerator += shortest;
			if (!(numerator < longest))
			{
				numerator -= longest;
				x += dx1;
				y += dy1;
			}
			else
			{
				x += dx2;
				y += dy2;
			}
		}

		return finalLine;
	}

	public IList<GridTile> GetSanderLine(GridTile start, GridTile end)
	{
		//throw new System.NotImplementedException("TODO fix");
		// 1. check voor horizontale / verticale lijn -> makkelijke edge case

		if(start.Column == end.Column || start.Row == end.Row)
		{
			Log.Write("straight line");
			return null;
		}

		// indien geen rechte lijn -> y = A*x + B functie opstellen ( en converteren naar x ook? x = (y-B)/a )
		float a = (end.Row - start.Row) / (1.0f * (end.Column - start.Column));
		float b = start.Row - a * start.Column;

		Log.Write(a + "*x+" + b);

		// TODO: find good place to check for overlapping cells

		Log.Write("-- Colums --");
		for(float x = Mathf.Min(start.Column, end.Column); x < Mathf.Max(start.Column, end.Column); x += 0.5f)
		{
			Log.Write("x:" + x + " y:" + (a * x + b).ToString());
		}

		Log.Write("-- Rows --");
		for (float y = Mathf.Min(start.Row, end.Row); y < Mathf.Max(start.Row, end.Row); y += 0.5f)
		{
			Log.Write("x:" + ((y-b)/a).ToString() + " y:" + y);
		}

		// get the biggest gap to bridge, or default to one of the two


		return null;
	}

	public void GenerateGrid(DungeonData data)
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
			if (gridElements[pos.Column][pos.Row] == null)
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
		Direction dir = Direction.Up;
		for (int numRotations = 0; numRotations < 7; numRotations++)
		{
			GridTile neighbourTile = gridElements[Mathf.Clamp(c + dir.GetHorizontalDirection(), 0, Columns-1)][Mathf.Clamp(r + dir.GetVerticalDirection(), 0, Rows-1)];
			
			if(neighbourTile != null && neighbourTile.Type != TileType.NONE && neighbourTile.Type != TileType.SightBlocker)
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