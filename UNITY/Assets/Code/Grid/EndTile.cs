using UnityEngine;
using System.Collections;

public class EndTile : GridTile
{
	public override void OnEnterTile(GridItem actor)
	{
		base.OnEnterTile(actor);

		PlayerMover player = actor as PlayerMover;
		if(player != null)
		{
			//victory: later on send a message to the GameManager
			//Application.LoadLevel(0);

			BaseGameManager mgr = FindObjectOfType<BaseGameManager>();

			mgr.FinishGame();
		}
	}
	
}
