using System;
using UnityEngine;

namespace GridActorSystem
{
	class PlayerActor : MonoBehaviour, IGridActor
	{
		private Action onEndTurn;

		public void ProcessTurn()
		{
			// this is not implemented :O
			throw new NotImplementedException();
		}

		public bool RequiresInput()
		{
			// change when a series of tasks is set
			return true;
		}

		void IGridActor.StartTurn(Action onEndTurn)
		{
			this.onEndTurn = onEndTurn;

			// Wait for input here
			InputController.Instance.OnTileClicked += TileClicked;
		}

		private void TileClicked(GridTileView tile)
		{
			if(onEndTurn != null)
			{
				onEndTurn();
				onEndTurn = null;
			}
		}
	}
}
