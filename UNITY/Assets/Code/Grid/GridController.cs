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
	public int columns { get { return gridData.Columns; } }
	public int rows { get { return gridData.Rows; } } 
	
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
		neighbourCol = Mathf.Clamp(neighbourCol, 0, columns-1);
		neighbourRow = Mathf.Clamp(neighbourRow, 0, rows-1);

		return gridElements[neighbourCol][neighbourRow];
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


        if (room.Row >= 0 && room.Column >= 0 && (room.Column + room.Width) < columns && (room.Row + room.Height) < rows)
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
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
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
			GridTile neighbourTile = gridElements[Mathf.Clamp(c + dir.GetHorizontalDirection(), 0, columns-1)][Mathf.Clamp(r + dir.GetVerticalDirection(), 0, rows-1)];
			
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
		return col == 0 || col == columns-1 || row == 0 || row == rows-1;
	}

}