using System;

namespace Entities.Model.Components
{
	public abstract class Component
	{
		// this might get dirty when set from ouside an Entity
		/// <summary>
		/// ONLY set from within an entity
		/// </summary>
		public Entity Entity;
	}


}
