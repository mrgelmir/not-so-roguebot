using Entities.Model.Components;
using System.Collections.Generic;
using System;

namespace Entities.Model
{
	public class Entity
	{

		// TODO see how to handle deletion (do not alter IEnumerations while iterating)

		private readonly int id;
		private List<Component> components = new List<Component>();

		public int ID
		{
			get { return id; }
		}

		public int ComponentCount
		{
			get { return components.Count; }
		}

		public Entity(int id)
		{
			this.id = id;
		}

		public bool AddComponent<T>(T component) where T : Component
		{
			if (HasComponent<T>())
			{
				Log.Warning("Entity::AddComponent - attempt to add duplicate component " + component.GetType().ToString());
				return false;
			}
			else
			{
				// Add component to list and mark with personal ID
				components.Add(component);
				component.Entity = this;
				return true;
			}
		}

		public bool RemoveComponent<T>()
		{
			bool removed = false;
			for (int i = 0; i < components.Count;)
			{
				if(components[i] is T)
				{
					components.RemoveAt(i);
					removed = true;
					continue;
				}

				++i;
			}

			return removed;
		}

		/// <summary>
		/// Gets the component of the specified type
		/// </summary>
		/// <typeparam name="T">The desired Component-derived class</typeparam>
		/// <returns>The Component if found, null if not found</returns>
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

		public override int GetHashCode()
		{
			return ID;
		}

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
