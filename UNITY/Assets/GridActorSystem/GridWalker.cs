using System.Collections.Generic;
using UnityEngine;

namespace GridActorSystem
{
	public class GridWalker : GridAction, IMover
	{
		public bool MoveTo(GridTileView tile)
		{
			if(ValidateTile(tile))
			{
				Entity.SetTile(tile);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ValidateTile(GridTileView tile)
		{
			return true; // atm, all tiles are okay
		}
	}
}
