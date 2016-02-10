﻿using Entities.Model;
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
		// TEMP references
		public GameObject MechReference;


		private GridEntities entities;
		private Dictionary<int, GameObject> entityVisuals = new Dictionary<int, GameObject>();

		protected void Start()
		{
			// get gridEntities reference form gameManager
			entities = FindObjectOfType<GameManager>().Entities;
			entities.SubscribeOnEntities(EntityAdded, EntityRemoved, true);
		}

		public void UpdateEntityVisual(int entityID)
		{
			Log.Write("Entity with id " + entityID + " is updating its visual");

			// get id from entitymanager OR pass entity here
			EntityVisual v = entities[entityID].GetComponent<EntityVisual>();
			if (v != null)
			{
				// check if a visual already exists
				GameObject visual;
				if (!entityVisuals.TryGetValue(entityID, out visual))
				{
					visual = Instantiate(MechReference);

					// create visual
					entityVisuals.Add(entityID, visual);
				}
				

				// TEMP visual only gets created, not updated

			}
		}

		public void UpdateEntityPosition(int entityID)
		{
			Position position = entities[entityID].GetComponent<Position>();

			if(position!= null)
			{
				// Check if a visual already exists (visual can be an empty GameObject)
				// TODO: should we be subscribed to a non-visual entity? 
				GameObject visual = GetOrCreateVisual(entityID);

				// update the position
				visual.transform.position = position.Pos.ToWorldPos();
				visual.transform.eulerAngles = position.Orientation.ToEulerAngles();
			}
			else
			{
				Log.Error("EntityFactory::UpdateEntityPosition - Updating an entity without a position is not possible");
			}
		}

		private void EntityAdded(int entityID)
		{
			// update visual
			UpdateEntityVisual(entityID);
			UpdateEntityPosition(entityID);

			// subscribe to all hooks etc here

			Position p = entities[entityID].GetComponent<Position>();
			if(p != null)
			{
				p.OnPositionChanged += UpdateEntityPosition;
			}
		}

		private void EntityRemoved(int entityID)
		{
			// TODO
		}

		private GameObject GetOrCreateVisual(int entityID)
		{
			GameObject visual;
			if (!entityVisuals.TryGetValue(entityID, out visual))
			{
				// entity not found -> create new one

				// get needed data
				Entity entity = entities[entityID];
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
				entityVisuals.Add(entityID, visual);
            }

			return visual;
		}
	}
}
