using System;
using System.Collections;
using System.Collections.Generic;

namespace GridActorSystem
{
	class GameMaster
	{
		public List<IGridActor> GridActors;

		public void StartRound()
		{
			var num = ProcessRound();
			Log.Write(num);
		}

		private IEnumerator ProcessRound()
		{			
			foreach (IGridActor actor in GridActors)
			{
				if(!ProcessTurn(actor))
				{
					yield return 0;
				}
			}
		}

		private bool ProcessTurn(IGridActor actor)
		{
			if(actor.RequiresInput())
			{
				actor.StartTurn(StartRound);
				return false;
			}
			else
			{
				actor.ProcessTurn();
				return true;
			}
		}
	}
}
