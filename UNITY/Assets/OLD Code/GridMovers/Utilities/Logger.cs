using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GridActor))]
public class Logger : MonoBehaviour 
{
	public GridActor Actor;
	public TextMesh Text;

	public LogType logType = LogType.Hitpoints;

	protected void Start()
	{
		if(Actor == null)
		{
			Actor = GetComponent<GridActor>();
			if(Actor == null)
			{
				Debug.Log("No Mover found: removing", gameObject);
				Destroy(this);
			}
		}
	}

	protected void Update()
	{
		switch (logType) {
		default:
		case LogType.Hitpoints :
			Text.text = Actor.HitPoints + "/" + Actor.moverData.Hitpoints;
			break;
		case LogType.Initiative :
			Text.text = Actor.InitiativeCountDown.ToString();
			break;
		}
	}

	public enum LogType
	{
		Initiative,
		Hitpoints,
	}
}
