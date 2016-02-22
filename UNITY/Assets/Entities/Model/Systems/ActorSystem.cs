using Entities.Model.Components;
using GridCode;
using PathFinding;
using System;
using System.Collections.Generic;

namespace Entities.Model.Systems
{
	public class ActorSystem
	{
		public Action OnResume;

		private readonly GridEntities entities;
		private readonly GridData grid;
		Random rand = new Random();

		public ActorSystem(GridEntities entities, GridData grid)
		{
			this.entities = entities;
			this.grid = grid;
		}

		/// <summary>
		/// Determines what to do with the given actor
		/// </summary>
		/// <param name="actor">the Actor to process</param>
		/// <returns>Should processing continue for this frame</returns>
		public bool HandleActor(Actor actor)
		{
			currentEntity = entities[actor.entityID];

			if (actor.InstantActor)
			{
				// do stuff here and continue

				// temp assume random for each instant actor
				Position pos = currentEntity.GetComponent<Position>();
				if(pos != null)
				{
					// let's not care about bumping into walls shall we
					GridDirection dir = GridDirectionHelper.GetRandomDirection(rand);

					// TODO this movement code should not be used on position directly
					// -> use a movement component in the future
					if (pos.IsValidTile(grid[pos.Pos + dir].Type))
					{
						pos.Pos += dir;
					}
					else
					{
						pos.Orientation = dir;
					}
				}

				currentEntity = null;
				return true;
			}
			else
			{

				// Temp implementation
				if(playerPath == null || playerPath.Count <= 0)
				{
					// start listening to input for a path
					InputController.Instance.OnTileClicked += TileTapped;
				}
				else
				{
					// just continue along path

					Position pos = currentEntity.GetComponent<Position>();
					pos.Pos = playerPath.Dequeue().Position;

					return true;
				}


				return false;
			}
		}

		private Entity currentEntity = null;

		// temp
		private Queue<TileData> playerPath;

		private void TileTapped(TileData tile)
		{
			InputController.Instance.OnTileClicked -= TileTapped;

			// TODO use movement component here as well
			Position pos = currentEntity.GetComponent<Position>();

			if (pos.IsValidTile(tile.Type))
			{

				playerPath = new Queue<TileData>(PathFinder<TileData>.FindPath(grid[pos.Pos], tile));

				// remove start pos from path
				playerPath.Dequeue();

				if (playerPath.Count > 0)
					pos.Pos = playerPath.Dequeue().Position;
			}

			FinishTurn();
		}


		private void FinishTurn()
		{
			currentEntity = null;

			if (OnResume != null)
			{
				OnResume();
			}
			else
			{
				Log.Warning("ActorSystem::TileTapped - There is no resume callback available, this will probably break the gameloop");
			}
		}
	}
}
