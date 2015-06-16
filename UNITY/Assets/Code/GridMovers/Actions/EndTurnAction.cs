using UnityEngine;
using System.Collections;

public class EndTurnAction : BaseGameAction 
{
	public override int ActionCost 
	{
		// Return all remaining action points, thus ending the turn
		get { return actor.ActionPoints; }
	} 

	public override void ExecuteAction (System.Action onActionComplete, System.Action onActionCancelled)
	{
		base.ExecuteAction (onActionComplete, onActionCancelled);
		// there is a joke about auto-complete in here somewhere...
		ActionComplete();
	}
}
