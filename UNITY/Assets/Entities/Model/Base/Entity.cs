using Entities.Model.Components;
using System.Collections.Generic;
using System;

namespace Entities.Model
{
	public class Entity
	{
		#region Members

		private readonly int id;
		private List<Component> components = new List<Component>();

		private Action<Component> onComponentAdded;
		private Action<Component> onComponentRemoved;
		#endregion

		#region Accessors

		public int ID
		{
			get { return id; }
		}

		public IList<Component> Components
		{
			// let's try out the readonly list, if we bump into issues, see what we can do
			get { return components.AsReadOnly(); }
		}

		public int ComponentCount
		{
			get { return components.Count; }
		}

		public event Action<Component> OnComponentAdded
		{
			add { onComponentAdded += value; }
			remove { onComponentAdded -= value; }
		}

		public event Action<Component> OnComponentRemoved
		{
			add { onComponentRemoved += value; }
			remove { onComponentRemoved -= value; }
		}
		#endregion

		#region Public Methods

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

				if(onComponentAdded != null)
				{
					onComponentAdded(component);
				}

				return true;
			}
		}

		public bool RemoveComponent<T>()
		{
			bool removed = false;
			for (int i = 0; i < components.Count;)
			{
				if (components[i] is T)
				{
					components.RemoveAt(i);
					removed = true;

					if(onComponentRemoved != null)
					{
						onComponentRemoved(components[i]);
					}

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
		#endregion

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
