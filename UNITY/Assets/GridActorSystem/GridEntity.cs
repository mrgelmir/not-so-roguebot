﻿using UnityEngine;
using System.Collections;

namespace GridActorSystem
{
	/// <summary>
	/// Base of all things grid related
	/// - Knows its positon in a grid
	/// - Can block a tile
	/// - Holds a reference to the grid
	/// - Is like John Snow; it knows nothing
	/// </summary>
	[SelectionBase]
	public class GridEntity : MonoBehaviour
	{
		private GridTileView currentTile;
		public GridTileView CurrentTile { get; private set; }

		public int ID;

		public bool BlocksTile = true;

		public void SetTile(GridTileView tile)
		{
			// only enter the tile if this is a non-blocking item or if the tile is not taken
			if (!BlocksTile || !tile.IsTaken)
			{
				// leave current tile
				if(currentTile != null)
				{
					LeaveTile();
				}
				EnterTile(tile);
			}
		}

		public void Remove()
		{
			// TODO: add to pool or remove via a manager
			Destroy(gameObject);
		}

		private void EnterTile(GridTileView tile)
		{
			currentTile = tile;
			tile.OnEnterTile(this);
		}

		private void LeaveTile()
		{
			currentTile.OnLeaveTile();
		}
		
	}
}