using System;

namespace Entities.Model.Components
{
	public class Interactable : Component
	{
		// has some sort of interaction logic
		// this can be a button, a door to be opened, a switch ...
		// an actor should be next to the interactable to use it (possibly facing as well)

		public bool SingleAction;
		public Action<Entity> OnInteract;
		
	}

	public static class InteractableFunctionality
	{
		public static void Interact(this Interactable interactable, Entity actor)
		{
			if(interactable.OnInteract != null)
			{
				interactable.OnInteract(actor);
			}

			if(interactable.SingleAction)
			{
				interactable.Entity.RemoveComponent<Interactable>();
			}
		}
	}
}
