using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
	private GridActor actor;
	public Transform HealthBarVisual;

	protected void OnEnable()
	{
		actor = GetComponentInParent<GridActor>();

		if(actor == null)
		{
			Debug.LogWarning("HealthBar - actor not found, selfdestructing now");
			Destroy(gameObject);
		}
	}


	protected void Update()
	{
		// TODO make this subscribbe to an event or smth
		float healthPercentage = (actor.HitPoints * 1f) / actor.moverData.Hitpoints;
		HealthBarVisual.transform.localScale = new Vector3(healthPercentage, 1f, 1f);
	}
}
