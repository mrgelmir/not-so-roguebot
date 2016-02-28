using Entities.Model.Components;
using UnityEngine;

namespace Entities.Visual
{
	public class EntityVisualFactory : MonoBehaviour
	{

		// TEMP visual references - replace by a more generic system or a container
		[SerializeField]
		private GameObject playerVisual;
		[SerializeField]
		private GameObject enemyVisual;
		[SerializeField]
		private GameObject doorVisual;
		[SerializeField]
		private GameObject triggerVisual;
		[SerializeField]
		private GameObject defaultVisual;

		/// <summary>
		/// Request an entity visual here
		/// </summary>
		/// <param name="visual">the entity's visual component</param>
		/// <returns>An instance of the given entity's visual</returns>
		public GameObject GetVisual(EntityVisual visual)
		{
			// TODO: fix all of this :-)
			GameObject visualPrefab;
			switch (visual.visualReference)
			{
				default:
					visualPrefab = defaultVisual;
					break;
				case "enemyVisual":
					visualPrefab = enemyVisual;
					break;
				case "playerVisual":
					visualPrefab = playerVisual;
					break;
				case "door":
					visualPrefab = doorVisual;
					break;
				case "triggerVisual":
					visualPrefab = triggerVisual;
					break;
			}

			return Instantiate(visualPrefab);
		}
	}
}
