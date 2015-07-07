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
					if(el.Type == TileType.Walkeable && !el.IsTaken)
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
                tileType = TileType.Walkeable;
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


// TODO move to gridUtils or something, as the dungeon generation uses this
[System.Flags]
public enum Direction
{
	NONE = 0,
	Up = 1<<0,
	Right = 1<<1,
	Down = 1<<2,
	Left = 1<<3,
}

public static class DirectionHelper
{
	// let the getter use its own random to keep seeds valid
	public static Direction GetRandomDirection(System.Random rand)
	{
		Direction dir = Direction.NONE;

		// vertical directions
		int randomNr = rand.Next(0, 3);
		if (randomNr == 0)
		{
			dir.AddDirection(Direction.Right);
		}
		else if (randomNr == 1)
		{
			dir.AddDirection(Direction.Left);
		}

		// horizontal directions
		randomNr = rand.Next(0, 3);
		if (randomNr == 0)
		{
			dir.AddDirection(Direction.Up);
		}
		else if (randomNr == 1)
		{
			dir.AddDirection(Direction.Down);
		}		

		return dir;
	}

	public static Direction GetRandomAxisAlignedDirection(System.Random rand)
	{
		int randomNr = rand.Next(0, 4);
		return (Direction)(1 << randomNr);
	}

	public static void AddDirection(this Direction currentDirection, Direction addedDirection)
	{
		currentDirection |= addedDirection;
	}

	public static void RemoveDirection(this Direction currentDirection, Direction removeDirection)
	{
		currentDirection &= removeDirection;
	}

	public static bool ContainsDirection(this Direction direction, Direction other)
	{
		return (direction & other) == other;
	}

	public static int GetHorizontalDirection(this Direction direction)
	{
		return ((direction & Direction.Right) == Direction.Right) ? 1 : (((direction & Direction.Left) == Direction.Left) ? -1 : 0);
	}

	public static int GetVerticalDirection(this Direction direction)
	{
		return ((direction & Direction.Up) == Direction.Up) ? 1 : (((direction & Direction.Down) == Direction.Down) ? -1 : 0);
	}

	/// <summary>
	/// Rotates the Direction in increments of 45 degrees 
	/// </summary>
	/// <param name="direction to rotate"></param>
	/// <param name="angle in degrees"></param>
	/// <returns>The rotated direction</returns>
	public static Direction RotateBy(this Direction direction, int angle)
	{
		if(angle < 0) // deal with negative angles
		{ angle = 360 - (System.Math.Abs(angle) % 360); }

		int numClockwiseRotations = angle / 45;

		int newDirection = 0;
		int directionInt = (int)direction;

		// check if power of two: if so only a single bit is set
		bool singleBitSet = directionInt != 0 && (directionInt & (directionInt - 1)) == 0;

		for (int i = 0; i < numClockwiseRotations; i++)
		{
			// some binary stuff incoming
			if (singleBitSet)
			{
				// shift left by one, clamp within 4 bits bounds, then OR current direction 
				newDirection = ((directionInt << 1) % 15) | directionInt;
			}
			else
			{
				// shift left by one, clamp within 4 bits bounds, then AND current direction
				newDirection = ((directionInt << 1) % 15) & directionInt;
			}

			directionInt = newDirection;
			singleBitSet = !singleBitSet;
		}

		return (Direction)newDirection;
	}
}