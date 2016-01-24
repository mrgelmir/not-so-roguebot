using UnityEngine;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Model;

namespace DungeonVisuals
{
	public class TileFactory : MonoBehaviour
	{
		public List<TileCollection> TileCollections;

		// TODO object collections
		public GameObject Door_NS;
		public GameObject Door_EW;

		// a collection of tile collections, by tile type
		private Dictionary<DungeonTileType, TileCollection> tileCollectionsMap = null;

		// a reference to the grid data object to figure out orientations
		// TODO once a tile knows of the gridData, remove this reference
		private GridData gridData = null;

		// Container holding the existing grid Tile Visuals
		private Dictionary<TileData, GameObject> gridTileVisuals = new Dictionary<TileData, GameObject>();

		// Container holding existing grid object visuals
		// TODO how to handle more than one object on a grid tile
		private Dictionary<TileData, GameObject> gridTileObjectVisuals = new Dictionary<TileData, GameObject>();

		// temp styling
		Dictionary<int, Color> colorMap = new Dictionary<int, Color>();
		
		#region Interface
		public void SetGrid(GridData gridData)
		{
			UpdateCollections();
			this.gridData = gridData;
			gridData.OnTileChanged += UpdateTileVisual;
			gridData.OnTileObjectChanged += UpdateTileObjectVisual;

			// create all tile visuals for the first time
			foreach (TileData tileData in gridData)
			{
				UpdateTileVisual(tileData);
				UpdateTileObjectVisual(tileData);
			}
		}

		public void UpdateTileVisual(TileData tileData)
		{
			// get current visual
			GameObject currentVisual;
			gridTileVisuals.TryGetValue(tileData, out currentVisual);

			if (currentVisual != null)
			{
				// release visual
				ReleaseVisual(currentVisual);
			}

			// get new visual
			GameObject newVisual = null;

			if (tileCollectionsMap == null || tileCollectionsMap.Keys.Count <= 0)
			{
				Log.Warning("TileFactory::GetTileVisual - tile collections map is not yet initialized or null");
				newVisual = null;
			}
			
			// catch special type cases here
			switch (tileData.Type)
			{
				case DungeonTileType.None:
					// No need for a tile visual -> silent fail
					newVisual = null;
					break;
				default:
				case DungeonTileType.Wall:
				case DungeonTileType.Flat:
					newVisual = SpawnTile(tileData, GetRegularTileVisual);
					break;
			}

			if(gridTileVisuals.ContainsKey(tileData))
			{
				if(newVisual == null)
				{
					gridTileVisuals.Remove(tileData);
				}
				else
				{
					gridTileVisuals[tileData] = newVisual;
				}
			}
			else
			{
				gridTileVisuals.Add(tileData, newVisual);
			}
        }
		
		public void UpdateTileObjectVisual(TileData tileData)
		{
			TileObjectData objectData = tileData.ObjectData;

			GameObject currentVisual;
			gridTileObjectVisuals.TryGetValue(tileData, out currentVisual);

			if (objectData == null)
			{
				// clear the current visual if it exists ...
				if (currentVisual != null)
				{
					ReleaseVisual(currentVisual);
					gridTileObjectVisuals.Remove(tileData);
                }

				// ... and then we are done here
				return;
			}

			// hardcoded implementation for now

			GameObject newVisual = null;

			if(objectData.ObjectCode == "door")
			{
				newVisual = SpawnObject(tileData, GetDoorTileVisual);
			}

			// ATM only one object per tile
			if (gridTileObjectVisuals.ContainsKey(tileData))
			{
				gridTileObjectVisuals[tileData] = newVisual;
			}
			else
			{
				gridTileObjectVisuals.Add(tileData, newVisual);
			}
		}

		#endregion
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
				Log.Warning("TileFactory::SpawnTile - resourceGetter did not return a valid prefab");
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

		private GameObject SpawnObject(TileData tileData, System.Func<TileData, GameObject> resourceGetter)
		{
			// get the prefab from the provided function 
			if (resourceGetter == null)
			{
				Log.Error("TileFactory::SpawnObject - resourceGetter is null");
			}
			GameObject tilePrefab = resourceGetter(tileData);

			if (tilePrefab == null)
			{
				Log.Warning("TileFactory::SpawnObject - resourceGetter did not return a valid prefab");
				return null;
			}
			else
			{
				// Instantiate the Tile and set to correct position
				// TODO implement pooling here somewhere
				GameObject tileVisual = Instantiate<GameObject>(tilePrefab);
				tileVisual.name = tileData.ObjectData.ToString();
				tileVisual.transform.position = new Vector3(tileData.Column, 0f, tileData.Row);
				tileVisual.transform.SetParent(transform, false);
								
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
				orientation = orientation.AddDirection(GridDirection.North);
			if (neighbour_South != null && neighbour_South.Type == tileType)
				orientation = orientation.AddDirection(GridDirection.South);
			if (neighbour_East != null && neighbour_East.Type == tileType)
				orientation = orientation.AddDirection(GridDirection.East);
			if (neighbour_West != null && neighbour_West.Type == tileType)
				orientation = orientation.AddDirection(GridDirection.West);

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

			if (neighbour_North != null && (neighbour_North.Type == DungeonTileType.Wall)
				&& neighbour_South != null && (neighbour_South.Type == DungeonTileType.Wall))
				return Door_EW;

			// East - West are walls/doors -> passage NS
			TileData neighbour_East = gridData.GetTile(tileData.Column + 1, tileData.Row);
			TileData neighbour_West = gridData.GetTile(tileData.Column - 1, tileData.Row);

			if (neighbour_East != null && (neighbour_East.Type == DungeonTileType.Wall)
				&& neighbour_West != null && (neighbour_West.Type == DungeonTileType.Wall))
				return Door_NS;

			// return this door until the algorithm doesn't derp out on doors anymore
			return Door_NS;
		}

		public void ReleaseVisual(GameObject visual)
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
			switch (orientation)
			{
				default:
				case GridDirection.None:
					visual = collection.Tile_X;
					if(visual == null)
					{
						Debug.LogWarning("TileFactory::TileForOrientation - The default tile (Tile_X) is not set for the collection " + collection.CollectionName);
					}
					break;
				case GridDirection.North:
					visual = collection.Tile_N;
					break;
				case GridDirection.NorthEast:
					visual = collection.Tile_NE;
					break;
				case GridDirection.East:
					visual = collection.Tile_E;
					break;
				case GridDirection.SouthEast:
					visual = collection.Tile_ES;
					break;
				case GridDirection.South:
					visual = collection.Tile_S;
					break;
				case GridDirection.SouthWest:
					visual = collection.Tile_SW;
					break;
				case GridDirection.West:
					visual = collection.Tile_W;
					break;
				case GridDirection.NorthWest:
					visual = collection.Tile_NW;
					break;
				case GridDirection.NNeEEsSwW:
					visual = collection.Tile_NNeESeSwW;
					break;
				case GridDirection.NESW:
					visual = collection.Tile_NESW;
					break;
				case GridDirection.NS:
					visual = collection.Tile_NS;
					break;
				case GridDirection.EW:
					visual = collection.Tile_EW;
					break;
				case GridDirection.SW:
					visual = collection.Tile_SW;
					break;
				case GridDirection.NW:
					visual = collection.Tile_NW;
					break;
				case GridDirection.NE:
					visual = collection.Tile_NE;
					break;
				case GridDirection.ES:
					visual = collection.Tile_ES;
					break;
				case GridDirection.ESW:
					visual = collection.Tile_ESW;
					break;
				case GridDirection.NSW:
					visual = collection.Tile_NSW;
					break;
				case GridDirection.NEW:
					visual = collection.Tile_NEW;
					break;
				case GridDirection.NES:
					visual = collection.Tile_NES;
					break;
			}

			if (visual == null)
			{
				// temp fix: use the singel tile when tile is not found
				Log.Write("TileFactory::TileForOrientation - no tile found for orientation " + orientation);
				visual = collection.Tile_X;
			}

			return visual;
		}
	}
}
