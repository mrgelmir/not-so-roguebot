using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using GridCode;

public class InputController : MonoBehaviour
{
	// Singleton stuff
	private static InputController instance = null;
	public static InputController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<InputController>();
				if (instance == null)
				{
					GameObject go = new GameObject("InputController");
					instance = go.AddComponent<InputController>();
				}
			}

			return instance;
		}
	}

	// Actual Input stuff

	// temp grid reference
	public GridData Grid;

	//public System.Action<GridActor> OnActorClicked;
	public System.Action<TileData> OnTileClicked;

	[SerializeField]
	private Camera cam;
	public Camera Cam
	{
		get
		{
			// TODO take prefrence to the player camera instead of using the main camera (+ allow swapping cameras?)
			if (cam == null)
			{
				cam = Camera.main;
			}

			return cam;
		}
		set { cam = value; }
	}

	protected void Awake()
	{
		// Singleton stuff
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	protected void Update()
	{
		// if the current event system as taken, we bail out
		// TODO convert this script to a custom Input Module
		if (EventSystem.current.alreadySelecting)
		{
			Log.Write("event system is overriding");
			return;
		}


#if UNITY_EDITOR || UNITY_STANDALONE // mouse controls
		if (Input.GetMouseButtonDown(0))
		{
			TouchPoint(cam.ScreenToViewportPoint(Input.mousePosition));
		}


#else
		//only do one touch atm (heheheh...)
		if(Input.touchCount > 0)
		{
			Touch t = Input.touches[0];

			if(t.phase == TouchPhase.Ended)
			{
				TouchPoint(cam.ScreenToViewportPoint(t.position));
			}
		}
#endif
	}

	public void TouchPoint(Vector3 viewPortPos)
	{

		// TODO check if not used by interface
		if (EventSystem.current.IsPointerOverGameObject()) return;

		// cast a ray
		Ray ray = Cam.ViewportPointToRay(viewPortPos);
		RaycastHit hit;

		// if grid is tapped -> ask 
		if (Physics.Raycast(ray, out hit))
		{
			// only bother to cast when there are subscribers? or check type first, then subscribers?

			if (OnTileClicked != null)
			{
				//GridController controller = hit.collider.GetComponent<GridController>();
				//if (controller != null)
				//{
				//	GridTileView clickedTile = controller.ClosestTileToPoint(hit.point);
				//	if (clickedTile != null)
				//	{
				//		Debug.DrawLine(Vector3.zero, clickedTile.transform.position, Color.cyan, 2f);
				//		OnTileClicked(clickedTile);
				//	}
				//	return;
				//}

				// check where the ray intersects the collision plane
				Vector3 ClickPos = hit.point;

				// try to find the nearest tile (this, as most code, assumes a tile is 1 by 1 unit)
				TileData tile = Grid.GetTile(Mathf.FloorToInt(ClickPos.x + .5f), Mathf.FloorToInt(ClickPos.z + .5f));

				if (tile != null)
				{
					OnTileClicked(tile);
					Debug.DrawRay(ClickPos, Vector3.up, Color.green);
				}
				else
				{
					Debug.DrawRay(ClickPos, Vector3.up, Color.red);
				}
			}

			//if(OnActorClicked != null)
			//{
			//	GridActor actor = hit.collider.GetComponent<GridActor>();
			//	if(actor != null)
			//	{
			//		OnActorClicked(actor);
			//		return;
			//	}
			//}

			//other cases go here (or refactor to fancier solution?)
		}
	}
}
