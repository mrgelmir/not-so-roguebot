using GridCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridCode.Visuals
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

		GridCameraBounds currentCamBounds = GridCameraBounds.Empty;
		
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

			// TODO push all functions below into the GridCameraBounds struct

			// get new values
			GridCameraBounds newBounds = new GridCameraBounds()
			{
				Column = Mathf.RoundToInt(transform.position.x),
				Row = Mathf.RoundToInt(transform.position.z),
				CameraSize = Mathf.RoundToInt(boundsCamera.orthographicSize),
			};

			if (newBounds == currentCamBounds)
			{
				// nothing has changed -> save processing
				return;
			}

			// get other data
			newBounds.ColumnBuffer = Mathf.RoundToInt(newBounds.CameraSize * boundsCamera.aspect) + CameraBuffer;
			newBounds.RowBuffer = newBounds.CameraSize + CameraBuffer;


			// TODO make this only iterate between the new and current visible tiles
			for (int col = 0; col < gridData.Columns; col++)
			{
				for (int row = 0; row < gridData.Rows; row++)
				{
					bool wasInBounds = (col >= currentCamBounds.Column - currentCamBounds.ColumnBuffer && col <= currentCamBounds.Column + currentCamBounds.ColumnBuffer
						&& row >= currentCamBounds.Row - currentCamBounds.RowBuffer && row <= currentCamBounds.Row + currentCamBounds.RowBuffer);
					bool isInBounds = (col >= newBounds.Column - newBounds.ColumnBuffer && col <= newBounds.Column + newBounds.ColumnBuffer
						&& row >= newBounds.Row - newBounds.RowBuffer && row <= newBounds.Row + newBounds.RowBuffer);

					if (isInBounds && !wasInBounds)
					{
						ShowTile(gridData[col, row]);
					}
					else if (!isInBounds && wasInBounds)
					{
						HideTile(gridData[col, row]);
					}
				}
			}

			// update the current bounds data
			currentCamBounds = newBounds;
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

	[Serializable]
	struct GridCameraBounds
	{
		public int Column;
		public int Row;
		public int CameraSize;
		public int ColumnBuffer;
		public int RowBuffer;

		public static GridCameraBounds Empty
		{
			get
			{
				return new GridCameraBounds()
				{
					Column = 0,
					Row = 0,
					CameraSize = 0,
					ColumnBuffer = 0,
					RowBuffer = 0,
				};

			}
		}

		public override int GetHashCode()
		{
			return Column ^ Row ^ CameraSize;
		}

		public override bool Equals(object obj)
		{
			if (obj is GridCameraBounds)
			{
				return Equals((GridCameraBounds)obj);
			}
			else
			{
				return false;
			}
		}

		public bool Equals(GridCameraBounds other)
		{
			return this == other;
		}

		public static bool operator ==(GridCameraBounds b1, GridCameraBounds b2)
		{
			return !(b1 != b2);
		}

		public static bool operator !=(GridCameraBounds b1, GridCameraBounds b2)
		{
			return (b1.Column != b2.Column || b1.Row != b2.Row || b1.CameraSize != b2.CameraSize);
		}
	}
}
