namespace Entities.Model.Components
{
	public class HitPoints : Component
	{
		public int CurrentHitpoints
		{
			get { return currentHitPoints; }
			set { currentHitPoints = value; }
		}
		public int MaxHitPoints
		{
			get { return maxHitPoints; }
			set { maxHitPoints = value; }
		}

		private int currentHitPoints;
		private int maxHitPoints;

		// TODO: an OnDestroy delegate?

	}
}
