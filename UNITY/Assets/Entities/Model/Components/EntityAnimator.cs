using System;

namespace Entities.Model.Components
{
	public class EntityAnimator : Component
	{
		public Action<string, float> OnUpdateFloat;
		public Action<string> OnSetTrigger;
	}

	public static class EntityAnimatorFunctionality
	{
		public static void UpdateFloat(this EntityAnimator animator, string key, float value)
		{
			if(animator.OnUpdateFloat != null)
			{
				animator.OnUpdateFloat(key, value);
			}
		}
	}
}
