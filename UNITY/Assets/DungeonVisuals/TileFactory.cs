using UnityEngine;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Model;

namespace DungeonVisuals
{
	public class TileFactory : MonoBehaviour
	{
		public List<TileCollection> TileCollections;

		// TODO find out how to fix doors
		public GameObject Door_NS;
		public GameObject Door_EW;

		// a collection of tile collections, by tile type
		private Dictionary<DungeonTileType, TileCollection> tileCollectionsMap = null;

		// a reference to the grid data object to figure out orientations
		// TODO once a tile knows of the gridData, remove this reference
		private GridData gridData = null;

		// temp styling
		Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

		protected void Awake()
		{
			UpdateCollections();
		}

		public void SetGrid(GridData gridData)
		{
			this.gridData = gridData;
        }

		public GameObject GetTileVisual(TileData tileData)
		{
			if (tileCollectionsMap == null || tileCollectionsMap.Keys.Count <= 0)
			{
				Log.Warning("TileFactory::GetTileVisual - tile collections map is not yet initialized or null");
				return null;
			}
			
			// catch special type cases here
			switch (tileData.Type)
			{
				case DungeonTileType.None:
				case DungeonTileType.Target:
					// No need for a tile visual -> silent fail
					return null;
				default:
				case DungeonTileType.Wall:
				case DungeonTileType.Flat:
					return SpawnTile(tileData, GetRegularTileVisual);
				case DungeonTileType.Door:
					return SpawnTile(tileData, GetDoorTileVisual);
			}

		}

		private GameObject SpawnTile(TileData tileData, System.Func<TileData, GameObject> resourceGetter)
		{
			// get the prefab from the provided function 
			if(resourceGetter == null)
			{
				Log.Error("TileFactory::SpawnTile - resourceGetter is null");
			}
			GameObject tilePrefab = resourceGetter(tileData);

			if (tilePrefab == null)
			{
				Log.Warning("TileFactory::GetTileVisual - resourceGetter did not return a valid prefab");
				return null;
			}
			else
			{
				// Instantiate the Tile and set to correct position
				// TODO implement pooling here somewhere
				GameObject tileVisual = Instantiate<GameObject>(tilePrefab);
				tileVisual.name = tileData.ToString();
				tileVisual.transform.position = new Vector3(tileData.Column, 0f, tileData.Row);
				tileVisual.transform.SetParent(transform, false);

				// perform some temporary styling things
				Color tileColor = Color.white;
				if (!colorMap.ContainsKey(tileData.RoomIndex))
				{
					tileColor = new Color(Random.value, Random.value, Random.value);
					colorMap.Add(tileData.RoomIndex, tileColor);
				}
				else
				{
					tileColor = colorMap[tileData.RoomIndex];
				}

				SpriteRenderer sr = tileVisual.GetComponentInChildren<SpriteRenderer>();
				if (sr != null)
				{
					sr.color = tileColor;
				}
				else
				{
					Renderer r = tileVisual.GetComponentInChildren<Renderer>();
					if (r != null)
					{
						r.material.color = tileColor;
					}
				}

				// done
				return tileVisual;
			}
		}

		private GameObject GetRegularTileVisual(TileData tileData)
		{
			// get matching tileCollection
			// TODO determine styles?
			TileCollection collection;
			tileCollectionsMap.TryGetValue(tileData.Type, out collection);

			if (collection == null)
			{
				Log.Warning("TileFactory::GetTileVisual - tile collection does not exist for TileType " + tileData.Type.ToString());
				return null;
			}

			// get directions for all neighbouring tiles
			GridDirection orientation = GridDirection.None;
			DungeonTileType tileType = tileData.Type;

			TileData neighbour_North = gridData.GetTile(tileData.Column, tileData.Row + 1);
			TileData neighbour_South = gridData.GetTile(tileData.Column, tileData.Row - 1);
			TileData neighbour_East = gridData.GetTile(tileData.Column + 1, tileData.Row);
			TileData neighbour_West = gridData.GetTile(tileData.Column - 1, tileData.Row);

			if (neighbour_North != null && neighbour_North.Type == tileType)
				orientation.AddDirection(GridDirection.North);
			if (neighbour_South != null && neighbour_South.Type == tileType)
				orientation.AddDirection(GridDirection.South);
			if (neighbour_East != null && neighbour_East.Type == tileType)
				orientation.AddDirection(GridDirection.East);
			if (neighbour_West != null && neighbour_West.Type == tileType)
				orientation.AddDirection(GridDirection.West);

			GameObject prefab = TileForOrientation(orientation, collection);

			if(prefab == null)
			{
				Log.Warning("TileFactory::GetRegularTileVisual - tile collection does not contain tile for orientation " + orientation.ToString());
			}

			return prefab;
		}

		private GameObject GetDoorTileVisual(TileData tileData)
		{
			// check which sides of this tile are wall or door

			// North - South are walls/doors -> passage EW
			TileData neighbour_North = gridData.GetTile(tileData.Column, tileData.Row + 1);
			TileData neighbour_South = gridData.GetTile(tileData.Column, tileData.Row - 1);

			if (neighbour_North != null && (neighbour_North.Type == DungeonTileType.Wall || neighbour_North.Type == DungeonTileType.Door)
				&& neighbour_South != null && (neighbour_South.Type == DungeonTileType.Wall || neighbour_South.Type == DungeonTileType.Door))
				return Door_EW;

			// East - West are walls/doors -> passage NS
			TileData neighbour_East = gridData.GetTile(tileData.Column + 1, tileData.Row);
			TileData neighbour_West = gridData.GetTile(tileData.Column - 1, tileData.Row);

			if (neighbour_East != null && (neighbour_East.Type == DungeonTileType.Wall || neighbour_East.Type == DungeonTileType.Door)
				&& neighbour_West != null && (neighbour_West.Type == DungeonTileType.Wall || neighbour_West.Type == DungeonTileType.Door))
				return Door_NS;

			// return this door until the algorithm doesn't derp out on doors anymore
			return Door_NS;
		}

		public void ReleaseTileVisual(GameObject visual)
		{
			// TODO add pooling here
			Destroy(visual);
		}

		private void UpdateCollections()
		{
			// move collections from list to dictionary
			tileCollectionsMap = new Dictionary<DungeonTileType, TileCollection>(TileCollections.Count);

			foreach (TileCollection collection in TileCollections)
			{
				tileCollectionsMap.Add(collection.Type, collection);
			}
		}

		private GameObject TileForOrientation(GridDirection orientation, TileCollection collection)
		{
			if (collection == null) return null;

			GameObject visual = null;
			// first try for perfect match
			// TEMP this gets skipped because diagonal neighbours already get skipped before			

			// then try without the diagonal matches

			// DO THIS NOW

			// if all else fails: use single tile
			visual = collection.Tile_X;

			if(visual == null)
			{
				Debug.LogWarning("TileFactory::TileForOrientation - The default tile (Tile_X) is not set for the collection " + collection.CollectionName);
			}

			return visual;
		}
	}
}
