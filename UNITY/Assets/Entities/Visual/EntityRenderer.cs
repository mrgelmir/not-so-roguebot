using Entities.Model;
using Entities.Model.Components;
using GridCode;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Visual
{
	/// <summary>
	/// Makes sure all needed entities are visually represented on the grid
	/// </summary>
	public class EntityRenderer : MonoBehaviour
	{
		// Scene references
		[SerializeField]
		private EntityVisualFactory entityFactory;

		// private members
		private GridEntities entities;
		private Dictionary<int, GameObject> entityVisuals = new Dictionary<int, GameObject>();

		protected void Start()
		{
			// get gridEntities reference form gameManager
			entities = FindObjectOfType<GameManager>().Entities;
			entities.SubscribeOnEntities(EntityAdded, EntityRemoved, true);
		}

		public void UpdateEntityVisual(Entity entity)
		{
			//Log.Write("Entity with id " + entityID + " is updating its visual");

			// get id from entitymanager OR pass entity here
			EntityVisual v = entity.GetComponent<EntityVisual>();
			EntityName n = entity.GetComponent<EntityName>();
			Position p = entity.GetComponent<Position>();

			if (v != null)
			{
				// check if a visual already exists
				GameObject visual;
				if (!entityVisuals.TryGetValue(entity.ID, out visual))
				{
					visual = entityFactory.GetVisual(v);
					visual.transform.SetParent(transform);

					visual.name = n != null ? n.NameString : p.Pos.ToString();

					// add visual to collection
					entityVisuals.Add(entity.ID, visual);
				}
				

				// TEMP visual only gets created, not updated

			}
			else
			{
				// update the unexisting visual of an entity ... ?

				// if a visaul exists for this entity -> remove
				RemoveVisual(entity);
			}
		}

		public GameObject GetEntityVisual(int entityID)
		{
			GameObject visual;
			if (!entityVisuals.TryGetValue(entityID, out visual))
			{
				return null;
			}
			else
			{
				return visual;
			}
		}

		public void UpdateEntityPosition(Entity entity)
		{
			Position position = entity.GetComponent<Position>();

			if(position!= null)
			{
				// Check if a visual already exists (visual can be an empty GameObject)
				// TODO: should we be subscribed to a non-visual entity? 
				GameObject visual = GetOrCreateVisual(entity);

				// update the position
				visual.transform.position = position.Pos.ToWorldPos();
				visual.transform.eulerAngles = position.Orientation.ToEulerAngles();
			}
			else
			{
				Log.Error("EntityFactory::UpdateEntityPosition - Updating an entity without a position is not possible");
			}
		}

		private void EntityAdded(Entity entity)
		{
			// update visual
			UpdateEntityVisual(entity);
			UpdateEntityPosition(entity);

			// subscribe to all hooks etc here

			Position p = entity.GetComponent<Position>();
			if(p != null)
			{
				p.OnPositionChanged += UpdateEntityPosition;
				p.OnOrientationChanged += UpdateEntityPosition;
			}
		}

		private void EntityRemoved(Entity entity)
		{
			// TODO

			RemoveVisual(entity);
		}

		private GameObject GetOrCreateVisual(Entity entity)
		{
			GameObject visual;
			if (!entityVisuals.TryGetValue(entity.ID, out visual))
			{
				// entity not found -> create new one

				// get needed data
                EntityName name = entity.GetComponent<EntityName>();
                Position position = entity.GetComponent<Position>();

				// a position is quite essential for visual entities
				if(position == null)
				{
					Log.Error("EntityFactory::GetOrCreateVisual - Creating a visual without a position is a no-go");
					return null;
				}

				// create actual gameObject
				visual = new GameObject(name == null ? position.Pos.ToString() : name.NameString);
				visual.transform.position = position.Pos.ToWorldPos();
				visual.transform.eulerAngles = position.Orientation.ToEulerAngles();

				// add visual reference
				entityVisuals.Add(entity.ID, visual);
            }

			return visual;
		}

		private void RemoveVisual(Entity entity)
		{
			GameObject visual;
			if (entityVisuals.TryGetValue(entity.ID, out visual))
			{
				entityVisuals.Remove(entity.ID);

				// maybe do pooling here

				Destroy(visual);
			}
		}
	}
}
