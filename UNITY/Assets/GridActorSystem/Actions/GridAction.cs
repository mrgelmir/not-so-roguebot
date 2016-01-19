using UnityEngine;

namespace GridActorSystem
{
	public abstract class GridAction : MonoBehaviour
	{
		public GridEntity Entity { get; private set; }

		protected virtual void OnEnable()
		{
			Entity = GetComponent<GridEntity>();
			if(Entity == null)
			{
				Log.Warning("This Action is not attached to any Entity!", this);
			}
		}
	}
}
