using UnityEngine;
using System.Collections;

public class BaseGameAction : MonoBehaviour, IGameAction
{
	[SerializeField] protected int actionCost = 1;
	[SerializeField] protected ActionData data;
	private System.Action onActionComplete;
	private System.Action onActionCancelled;

	protected GridActor actor;
	
	public virtual int ActionCost 
	{
		get { return actionCost; }
	}

	public virtual ActionData Data 
	{
		get { return data; }
	}

	public virtual void Init(GridActor attachedActor)
	{
		this.actor = attachedActor;
	}
    
    public virtual void EndTurn()
    {

    }

	public virtual void ExecuteAction (System.Action onActionComplete, System.Action onActionCancelled)
	{
		this.onActionComplete = onActionComplete;
		this.onActionCancelled = onActionCancelled;
	}

    public virtual void CancelAction()
    {
        ActionCancelled();
    }

	protected virtual void ActionCancelled()
	{
		if(onActionCancelled != null)
			onActionCancelled();
		else
			Debug.LogWarning("BaseGameAction::ActionCancelled - No callback provided, this might screw up the game flow", gameObject);
	}

	protected virtual void ActionComplete()
	{
		if(onActionComplete != null)
			onActionComplete();
		else
			Debug.LogWarning("BaseGameAction::ActionComplete - No callback provided, this will probably screw up the game flow", gameObject);
	}
}
