using GridCode;

namespace Entities.Model.Components
{
	class Trigger : Component
	{
		/// <summary>
		/// The position to check
		/// this can be an entity's own position or a totally different one
		/// </summary>
		public GridPosition position;

		// TODO make this more data/callback oriented

		public void OnTriggerEnter(Entity entity)
		{
			//// temp
			HitPoints hp = entity.GetComponent<HitPoints>();
			if (hp != null)
			{
				hp.CurrentHitpoints -= 1;
			}

		}

		public void OnTriggerExit(Entity entity)
		{

		}

		public void OnTriggerStay(Entity entity)
		{
			// not yet implemented
		}
	}
}