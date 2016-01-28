using DungeonGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonVisuals
{
	[RequireComponent(typeof(Camera))]
	class TileRenderer : MonoBehaviour
	{
		public int CameraBuffer = 2;
		private Camera boundsCamera;

		// these are private because people need to get data when they subscribe to these callbacks
		private Action<TileData> OnTileBecameVisible;
		private Action<TileData> OnTileBecameInvisible;

		private List<TileData> visibleTiles = new List<TileData>();

		private int currentColumn = 0;
		private int currentRow = 0;
		private int currentCameraSize = 0;
		private int currentColumnBuffer = 0;
		private int currentRowBuffer = 0;

		private GridData gridData
		{ get { return GameManager.Instance.GridData; } }

		protected void Start()
		{
			if (gridData == null)
			{
				Log.Warning("TileRenderer::Start - gridData does not exist");
			}

			boundsCamera = GetComponent<Camera>();

			// TEMP set position to center  of grid
			CenterOnTile(gridData[gridData.Rows / 2, gridData.Columns / 2]);
			UpdateCameraBounds();
		}

		protected void Update()
		{
			UpdateCameraBounds();
		}

		public void SubscribeOnVisualTiles(Action<TileData> visibleCallback, Action<TileData> invisibleCallback)
		{
			OnTileBecameVisible += visibleCallback;
			OnTileBecameInvisible += invisibleCallback;

			foreach (TileData tileData in visibleTiles)
			{
				visibleCallback(tileData);
			}
		}

		public void UnSubscribeFromVisualTiles(Action<TileData> visibleCallback, Action<TileData> invisibleCallback)
		{
			OnTileBecameVisible -= visibleCallback;
			OnTileBecameInvisible -= invisibleCallback;
		}

		[ContextMenu("Update camera bounds")]
		private void UpdateCameraBounds()
		{
			// check which tiles are in bounds and which aren't

			// get new values
			int newColumn = Mathf.RoundToInt(transform.position.x);
			int newRow = Mathf.RoundToInt(transform.position.z);
			int newCameraSize = Mathf.RoundToInt(boundsCamera.orthographicSize);

			if(!(newColumn != currentColumn || newRow != currentRow || newCameraSize != currentCameraSize))
			{
				// nothing has changed -> save processing
				return;
			}
			
			int newColumnBuffer = Mathf.RoundToInt(currentCameraSize * boundsCamera.aspect) + CameraBuffer;
			int newRowBuffer = currentCameraSize + CameraBuffer;


			// TODO make this only iterate between the new and current visible tiles
			for (int col = 0; col < gridData.Columns; col++)
			{
				for (int row = 0; row < gridData.Rows; row++)
				{
					bool wasInBounds = (col >= currentColumn - currentColumnBuffer && col <= currentColumn + currentColumnBuffer
						&& row >= currentRow - currentRowBuffer && row <= currentRow + currentRowBuffer);
					bool isInBounds = (col >= newColumn - newColumnBuffer && col <= newColumn + newColumnBuffer
						&& row >= newRow - newRowBuffer && row <= newRow + newRowBuffer);

					if (isInBounds && !wasInBounds)
					{
						ShowTile(gridData[col, row]);
					}
					else if(!isInBounds && wasInBounds)
					{
						HideTile(gridData[col, row]);
					}
				}
			}

			// do the update
			currentColumn = newColumn;
			currentRow = newRow;
			currentCameraSize = newCameraSize;
			currentColumnBuffer = newColumnBuffer;
			currentRowBuffer = newRowBuffer;
		}

		private void ShowTile(TileData tileData)
		{
			if (!visibleTiles.Contains(tileData))
			{
				visibleTiles.Add(tileData);
				// TODO maybe make sure this function never gets called when OnTileBecameVisible == null
				if (OnTileBecameVisible != null)
				{
					OnTileBecameVisible(tileData);
				}
			}
		}

		private void HideTile(TileData tileData)
		{
			if (visibleTiles.Contains(tileData))
			{
				// TODO maybe make sure this function never gets called when OnTileBecameInvisible == null
				if (OnTileBecameInvisible != null)
				{
					OnTileBecameInvisible(tileData);
				}
				visibleTiles.Remove(tileData);
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
