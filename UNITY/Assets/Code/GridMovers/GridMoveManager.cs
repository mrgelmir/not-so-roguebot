using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMoveManager : MonoBehaviour
{
	[SerializeField] private List<GridActor> gridActors = new List<GridActor>();

	public int CurrentRound = 0;
	public bool BreakOnRoundEnd = false;

	public System.Action OnEndRound;
	
//	protected void Start()
//	{
//		StartGame();
//	}

	private void AddGridActor(GridActor actor)
	{
		if(!gridActors.Contains(actor))
		{
			gridActors.Add(actor);
			actor.OnDestroyed += OnActorDestroyed;
		}
	}

	public void StartGame()
	{
		// get all gridActors, even the missing ones
		foreach (var actor in FindObjectsOfType<GridActor>())
		{
			AddGridActor (actor);
		}

		// sort by initiative
		gridActors.Sort((GridActor x, GridActor y) => x.moverData.Initiative.CompareTo(y.moverData.Initiative));
		foreach (var actor in gridActors)
		{
			actor.Setup();
		}

		CurrentRound = 0;

//		NextTurn();
		StartRound();
	}

	public void EndGame()
	{
		foreach (GridActor actor in gridActors)
		{
			actor.StopTurn();
		}
	}

	private int currentActorIndex = 0;

	private void StartRound()
	{
		++CurrentRound;
		// reset all variables
		currentActorIndex = 0;

		// start turn
		ContinueRound();

	}

	private void ContinueRound()
	{
		// check all actors
		for(; currentActorIndex < gridActors.Count; ++currentActorIndex)
		{
			// if actors can move: start his turn and return 
			if(gridActors[currentActorIndex] != null && (gridActors[currentActorIndex].InitiativeCountDown -= 1) <= 0)
			{
				StartActorTurn(gridActors[currentActorIndex]);
				// we don't hit the end loop stuff, so we have to manually increment the counter here
				++currentActorIndex;
				// when actors is done with his turn, this function gets called again
				return;
			}
		}

		EndRound();
	}

	private void EndRound()
	{
		// check for objects to remove from list 

		gridActors.RemoveAll(delegate(GridActor obj) {
			if(obj == null)
				return true;
			if(obj.Remove)
			{
				Destroy(obj.gameObject);
				return true;
			}
			return false;
		});

		if(OnEndRound != null)
		{
			OnEndRound();
		}

		// start next round
		StartCoroutine(StartRoundRoutine());
	}

	private IEnumerator StartRoundRoutine ()
	{
		yield return null;

		if(BreakOnRoundEnd)
			Debug.Break();

		StartRound();
	}

	private void StartActorTurn(GridActor actor)
	{
		currentActor = actor.name;
		actor.StartTurn(EndActorTurn);
	}

	private void EndActorTurn(GridActor actor)
	{
		// re-add initiative
		actor.InitiativeCountDown += actor.moverData.Initiative;

		// continue current turn
		ContinueRound();
	}

	private void OnActorDestroyed(GridItem item)
	{
        GridActor actor = item as GridActor;
        if(actor != null)
        {
		    actor.Remove = true;
        }
	}

	private int SortByInitiative(GridActor x, GridActor y)
	{
		// Smaller initiative is better
		if(x.InitiativeCountDown < y.InitiativeCountDown)
			return -1;
		if(x.InitiativeCountDown == y.InitiativeCountDown)
		{
			if(x.moverData.Initiative > y.moverData.Initiative)
				return 1;
			else if(x.moverData.Initiative == y.moverData.Initiative)
				return 0;
			else
				return -1;
		}
		return -1;
	}


	private string currentActor = "";
	protected void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10f, 10f, 300f, 100f));

		GUILayout.Label("Current round = " + CurrentRound);
		GUILayout.Label(currentActor);

		GUILayout.EndArea();
	}

}
