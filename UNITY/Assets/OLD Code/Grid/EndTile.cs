using UnityEngine;
using System.Collections;
using GridActorSystem;

public class EndTile : GridTileView
{
	public override void OnEnterTile(GridEntity actor)
	{
		base.OnEnterTile(actor);
		
		// TODO: fix or replace with better solution, like a victory trigger

		//PlayerMover player = actor as PlayerMover;
		//if(player != null)
		//{
		//	//victory: later on send a message to the GameManager
		//	//Application.LoadLevel(0);

		//	GameManager mgr = FindObjectOfType<GameManager>();

		//	mgr.FinishGame();
		//}
	}
	
}
