using GridCode.Entities.Model.Components;
using System.Collections.Generic;

namespace GridCode.Entities.Model
{
	public class Entity
	{
		private readonly int id;
		private List<Component> components = new List<Component>();

		public int ID
		{
			get { return id; }
		}

		public bool AddComponent<T>(T component) where T : Component
		{
			if (HasComponent<T>())
			{
				return false;
			}
			else
			{
				// Add component to list and mark with personal ID
				components.Add(component);
				component.ID = ID;
				return true;
			}
		}

		public T GetComponent<T>() where T : Component
		{
			foreach (Component component in components)
			{
				if (component is T)
				{
					return component as T;
				}
			}
			return null;
		}

		public bool HasComponent<T>() where T : Component
		{
			foreach (Component component in components)
			{
				if (component is T)
				{
					return true;
				}
			}
			return false;
		}

		#region Comparison

		public override bool Equals(object obj)
		{
			Entity other = obj as Entity;
			return other == null ? false : Equals(other);
		}

		public bool Equals(Entity other)
		{
			return ID == other.id;
		}

		#endregion
	}
}
