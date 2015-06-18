using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;

public class GridController : MonoBehaviour
{

	[SerializeField] private TileGenerator generator;

	public static readonly float GridSpacing = 1f;

	[SerializeField, HideInInspector] private GridData gridData;

	private List<List<GridTile>> gridElements { get { return gridData.GridElements; } }
	public int columns { get { return gridData.Columns; } }
	public int rows { get { return gridData.Rows; } } 

	public Material WallMaterial;
	public Material FloorMaterial;

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
					if(el.Type == TileType.Flat && !el.IsTaken)
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
		GridTile el = generator.SpawnElementOfType(type);
		el.transform.SetParent(transform);
		el.SetData(this, col, row);
		el.UpdateVisual();

		return el;
	}

	public GridTile GetNeighbour(GridTile el, Directions dir)
	{
		int neighbourRow = el.Row;
		int neighbourCol = el.Column;

		// get actual neighbour position
		if ((dir & Directions.Left) == Directions.Left)
			-- neighbourCol;
		else if ((dir & Directions.Right) == Directions.Right)
			++ neighbourCol;

		if ((dir & Directions.Up) == Directions.Up)
			++ neighbourRow;
		else if ((dir & Directions.Down) == Directions.Down)
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


		if (room.LinkedRooms.Count <= 0)
			return;

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

                    // TODO check for existing tiles
                    gridElements[room.Column + c][room.Row + r] = CreateGridElement(ConvertType(tile.Type), room.Column + c, room.Row + r);
                }
            }
        }
    }

    private void AddCorridor(DungeonCorridor corridor)
    {
		// TODO out of grid checking

		foreach (DungeonPosition pos in corridor.TilePositions)
		{
			if (gridElements[pos.Column][pos.Row] == null)
				gridElements[pos.Column][pos.Row] = CreateGridElement(TileType.Flat, pos.Column, pos.Row);
		}
    }

    private void FillEmptyTiles()
    {
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                if (gridElements[c][r] == null)
                    gridElements[c][r] = CreateGridElement(TileType.NONE, c, r);
            }
        }
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
            case DungeonTileType.Door: // TEMP
                tileType = TileType.Flat;
                break;
            case DungeonTileType.Wall:
                tileType = TileType.Wall;
                break;
        }

        return tileType;
    }
    
	private bool IsBorderTile(int col, int row)
	{
		return col == 0 || col == columns-1 || row == 0 || row == rows-1;
	}

}

[System.Flags]
public enum Directions
{
	NONE = 0,
	Up = 1<<0,
	Right = 1<<1,
	Down = 1<<2,
	Left = 1<<3,
}
