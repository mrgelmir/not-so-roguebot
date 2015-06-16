using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;

public class GridController : MonoBehaviour
{

	[SerializeField] private TileGenerator generator;

	public static readonly float GridSpacing = 1f;

	public GridData Data;

	private List<List<GridTile>> gridElements { get { return Data.GridElements; } }
	public int columns { get { return Data.Columns; } }
	public int rows { get { return Data.Rows; } } 

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

	// TODO remove this
	#region generation

	private TileType[] tileTypes = new TileType[] {TileType.NONE, TileType.Flat, TileType.Flat, TileType.Flat, TileType.Flat, TileType.Wall};

	[ContextMenu("Clear grid")]
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

//		columns = data.Tiles.Count;
//		if(columns > 0)
//		{
//			rows = data.Tiles[0].Count;
//		}
//		else
//		{
//			Debug.LogError("not enoughelements in grid");
//			return null;
//		}

		for (int c = 0; c < data.Tiles.Count; c++)
		{
			gridElements.Add(new List<GridTile>(columns));
			for (int r = 0; r < data.Tiles[c].Count; r++) 
			{
				if(data.Tiles[c][r] == null)
				{
					Debug.Log("tile does not exist");
					continue;
				}

				TileType tileType;

				switch (data.Tiles[c][r].Type) 
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

				gridElements[c].Add(CreateGridElement(tileType, c, r));
			}
		}
	}

	[ContextMenu("Generate random grid")]
	public void GenerateRandomGrid()
	{
		ClearGrid();
		// temp generate random grid

		for(int col = 0; col < columns; ++col)
		{
			gridElements.Add(new List<GridTile>(columns));
			for(int row = 0; row < rows; ++row)
			{
				TileType randomType = tileTypes[Random.Range(0, tileTypes.Length)];
				gridElements[col].Add(CreateGridElement(randomType, col, row));
			}
		}
	}

	[ContextMenu("Generate Single room")]
	public void GenerateRoom()
	{
		ClearGrid();
		// temp generate random grid
		
		for(int col = 0; col < columns; ++col)
		{
			gridElements.Add(new List<GridTile>(columns));
			for(int row = 0; row < rows; ++row)
			{
				TileType tileType = IsBorderTile(col, row)? TileType.Wall: TileType.Flat;
				gridElements[col].Add(CreateGridElement(tileType, col, row));
			}
		}

		if(gridElements[columns/2][rows/2].Type == TileType.Flat)
			startTile = gridElements[columns/2][rows/2];
	}

#endregion

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
