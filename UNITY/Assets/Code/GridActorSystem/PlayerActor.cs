using System;
using UnityEngine;

namespace GridActorSystem
{
	class PlayerActor : MonoBehaviour, IGridActor
	{
		public void ProcessTurn()
		{
			throw new NotImplementedException();
		}

		public bool RequiresInput()
		{
			throw new NotImplementedException();
		}

		void IGridActor.StartTurn(Action onEndTurn)
		{
			throw new NotImplementedException();
		}
	}
}
