using System.Collections.Generic;
using UnityEngine;

namespace GridActorSystem
{
	public class GridWalker : GridAction, IMover
	{
		public bool MoveTo(GridTile tile)
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

		public bool ValidateTile(GridTile tile)
		{
			return true; // atm, all tiles are okay
		}
	}
}
