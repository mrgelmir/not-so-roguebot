using UnityEngine;
using System.Collections;

namespace GridActorSystem
{
	/// <summary>
	/// Base of all things grid related
	/// - Knows its positon in a grid
	/// - Can block a tile
	/// - Holds a reference to the grid
	/// - Is like John Snow, it knows nothing
	/// </summary>
	[SelectionBase]
	public class GridEntity : MonoBehaviour
	{
		private GridTile currentTile;
		public GridTile CurrentTile { get; private set; }

		public int ID;

		public bool BlocksTile = true;

		public void SetTile(GridTile tile)
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

		}

		private void EnterTile(GridTile tile)
		{
			currentTile = tile;
			//tile.OnEnterTile();
		}

		private void LeaveTile()
		{

		}
		
	}
}