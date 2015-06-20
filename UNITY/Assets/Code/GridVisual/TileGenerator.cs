using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
	[SerializeField] private List<GridTile> gridElements;

	// Very basic implementation
	public GridTile SpawnElementOfType(TileType type)
	{
		GridTile prefab = gridElements.Find((GridTile obj) => obj.Type == type);

		GridTile el = Instantiate<GridTile>(prefab);

		return el;
	}

}
