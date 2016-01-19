using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileGenerator : MonoBehaviour
{
	[SerializeField] private List<GridTileView> gridElements;
	[SerializeField] private TileSet tileSet;

	// TODO
	// - make this a decent factory
	// - add pooling

	// Very basic implementation
	public GridTileView GetElement(TileType type, TileOrientation orientation)
	{
		GridTileView tilePrefab = null;

		if (type != TileType.Walkeable)
		{
			tilePrefab = gridElements.Find((GridTileView obj) => obj.Type == type);
		}
		else
		{
			tilePrefab = TileForOrientation(orientation);
		}

		if(tilePrefab == null)
		{
			//Log.Warning("tile for type " + type.ToString() + " and orientation " + orientation.ToString() + " is not found");
			return null;
		}

		GridTileView spawnedTile = Instantiate(tilePrefab);

		return spawnedTile;
	}

	private GridTileView TileForOrientation(TileOrientation orientation)
	{
		GridTileView tile = null;
		switch (orientation)
		{
			case TileOrientation.NONE:
				tile = tileSet.Center;
				break;
			case TileOrientation.Top:
				tile = tileSet.Top;
				break;
			case TileOrientation.Top | TileOrientation.Right:
				tile = tileSet.TopRight;
				break;
			case TileOrientation.Top | TileOrientation.Left:
				tile = tileSet.TopLeft;
				break;
			case TileOrientation.Right:
				tile = tileSet.Right;
				break;
			case TileOrientation.Bottom:
				tile = tileSet.Bottom;
				break;
			case TileOrientation.Bottom | TileOrientation.Right:
				tile = tileSet.BottomRight;
				break;
			case TileOrientation.Bottom | TileOrientation.Left:
				tile = tileSet.BottomLeft;
				break;
			case TileOrientation.Left:
				tile = tileSet.Left;
				break;
			default:
				break;
		}
		return tile;
	}
}

// these types are stackable 
// http://unitypatterns.com/enums-and-flags/

[System.Flags]
public enum TileOrientation
{
	NONE = 0,
	Top = 1 << 0,
	Right = 1 << 1,
	Bottom = 1 << 2,
	Left = 1 << 3,
}

// TODO: wrap in separate class with helper functions
// ie Walkeable + Trapped (type.IsTrapped)
[System.Flags]
public enum TileType
{
	NONE = 0,
	Walkeable = 1 << 0,
	SightBlocker = 1 << 1,
	Interactable = 1 << 2,
	Door = 1 << 3 | 1 << 0,
	Target = 1 << 4 | 1 << 0,	
}

public static class TileTypeHelper
{
	public static bool ContainsType(this TileType ownType, TileType type)
	{
		return (ownType & type) == type;
	}
}