using UnityEngine;
using System.Collections;

public class MeleeAction : BaseGameAction
{
    private GridTile targetTile = null;

    public override void Init(GridActor attachedActor)
    {
        base.Init(attachedActor);
        visualisation.gameObject.SetActive(false);
    }

    // Optional method to set the target (for auto attacking etc)
    public virtual void SetTarget(GridTile target)
    {
        this.targetTile = target;
    }

    public override void ExecuteAction(System.Action onActionComplete, System.Action onActionCancelled)
    {
        base.ExecuteAction(onActionComplete, onActionCancelled);

        if (targetTile == null || targetTile.Actor == null)
        {
            GetTarget();
        }
        else
        {
            Attack();
        }
    }

    private void GetTarget()
    {
        ITargeter targeter = actor as ITargeter;
        if(targeter == null)
        {
            ActionCancelled();
        }
        else
        {
            targeter.RequestTargetNeigbourTile(AttackTargetTile);
        }
    }

    private void AttackTargetTile(GridTile tile)
    {
        if (tile != null)
        {
            targetTile = tile;
            Attack();
        }
        else
        {
            CancelAction();
        }
    }

    private void Attack()
    {
        if (targetTile.Actor as GridItem != null)
        {
            // attack here using actor's strength and stuff
            StartCoroutine(AttackRoutine());
        }
        else
        {
            CancelAction();
        }
    }

    [SerializeField] private LineRenderer visualisation;

    private IEnumerator AttackRoutine()
    {
        float attackDuration = .2f;

        // attack animation
        //iTween.PunchScale(gameObject, Vector3.one * .1f, attackDuration);
        visualisation.gameObject.SetActive(true);
        visualisation.SetPosition(0, transform.position + Vector3.up);
        visualisation.SetPosition(1, targetTile.transform.position + Vector3.up);

        yield return new WaitForSeconds(attackDuration);

        // calculate damage (temp)
        int damage = actor.moverData.Strength;

        // Deal damage (we assume that the actor is already nullchecked here)
        actor.Damage(targetTile.Actor as GridItem, damage);

        visualisation.gameObject.SetActive(false);
        yield return null;

        ActionComplete();
    }
}
