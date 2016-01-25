using DungeonGeneration;
using System;
using UnityEngine;

namespace DungeonVisuals
{
	[RequireComponent(typeof(Camera))]
	class TileRenderer : MonoBehaviour
	{
		private GridData gridData;
		private Camera boundsCamera;

		// these are private because people need to get data when they subscribe to these callbacks
		private Action<TileData> OnTileBecameVisible;
		private Action<TileData> OnTileBecameInvisible;

		protected void Start()
		{
			// get reference to grid Data
			gridData = FindObjectOfType<GameManager>().GridData;

			if (gridData == null)
			{
				Log.Warning("TileRenderer::Start - gridData does not exist");
			}

			// TEMP set position to center  of grid
			CenterOnTile(gridData[gridData.Rows, gridData.Columns]);
			UpdateCameraBounds();
		}

		public void SubscribeOnVisualTiles(Action<TileData> visibleCallback, Action<TileData> invisibleCallback)
		{
			OnTileBecameVisible += visibleCallback;
			OnTileBecameInvisible += invisibleCallback;

			//// TODO call the callbacks now for all tiles that are visual right now
			//foreach (TileData tileData in gridData)
			//{
			//	visibleCallback(tileData);
			//}
		}

		public void UnSubscribeFromVisualTiles(Action<TileData> visibleCallback, Action<TileData> invisibleCallback)
		{
			OnTileBecameVisible -= visibleCallback;
			OnTileBecameInvisible -= invisibleCallback;
		}

		private void UpdateCameraBounds()
		{
			// check which tiles are in bounds and which aren't


			// for now add all tiles
			if (OnTileBecameVisible != null)
			{
				foreach (TileData tileData in gridData)
				{
					OnTileBecameVisible(tileData);
				}
			}
		}

		private void CenterOnTile(TileData tileData)
		{
			if (tileData == null)
			{
				Log.Warning("TileRenderer::CenterOnTile - the tile on which to center is null");
				return;
			}
			// maintain current height (Z)
			Vector3 newPos = transform.position;
			newPos.x = tileData.Column;
			newPos.y = tileData.Row;

			transform.position = newPos;
		}
	}
}
