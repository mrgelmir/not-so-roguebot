using GridCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridCode.Visuals
{
	public class TileCollection : ScriptableObject
	{
		public string CollectionName = "new Tile Collection";
		public GridTileType Type = GridTileType.None;

		/* position name reference
		Nw|N|Ne
		 W|X|E
		Sw|S|Se
		*/

		// solo tile
		public GameObject Tile_X;
		
		// center tile
		public GameObject Tile_NNeESeSwW;

		// Cross tile
		public GameObject Tile_NESW;

		// ends
		public GameObject Tile_N;
		public GameObject Tile_E;
		public GameObject Tile_S;
		public GameObject Tile_W;

		// straights
		public GameObject Tile_NS;
		public GameObject Tile_EW;

		// empty corners
		public GameObject Tile_SW;
		public GameObject Tile_NW;
		public GameObject Tile_NE;
		public GameObject Tile_ES;

		// Filled corners
		public GameObject Tile_SSwW;
		public GameObject Tile_NNwW;
		public GameObject Tile_NNeE;
		public GameObject Tile_ESeS;

		// T's
		public GameObject Tile_ESW;
		public GameObject Tile_NSW;
		public GameObject Tile_NEW;
		public GameObject Tile_NES;
		
	}
}
