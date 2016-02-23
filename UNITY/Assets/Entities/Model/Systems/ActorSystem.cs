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
		private Random rand = new Random();

		/// <summary>
		/// saved reference to the current Entity
		/// this reference should only be valid 'till the end of the turn
		/// </summary>
		private Entity currentEntity = null;

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
			Position pos = currentEntity.GetComponent<Position>();
			Mover mover = currentEntity.GetComponent<Mover>();

			if (pos == null || mover == null)
			{
				// nothing to do here ATM (until other stuff comes around, like turrets)
				return true;
			}


			// if a path exists -> carry on no matter what type of actor
			// TODO check for interruptions to movement here
			if (mover.Path != null && mover.Path.Count > 0)
			{
				// just continue along path					
				pos.Pos = mover.Path.Dequeue().Position;
				return true;
			}

			if (actor.InstantActor)
			{
				// do stuff here and continue for instant processing

				// create a path to a random position

				TileData tile = grid.GetRandomTile(GridTileType.Flat);
				
				SetPath(mover, grid[pos.Pos], tile);

				if (mover.Path.Count > 0)
					pos.Pos = mover.Path.Dequeue().Position;


				currentEntity = null;
				return true;
			}
			else
			{
				// start listening to input for a path
				InputController.Instance.OnTileClicked += TileTapped;
				return false;
			}
		}

		private void TileTapped(TileData tile)
		{

			// TODO use movement component here as well
			Position pos = currentEntity.GetComponent<Position>();
			Mover mover = currentEntity.GetComponent<Mover>();

			if (pos.IsValidTile(tile.Type))
			{
				SetPath(mover, grid[pos.Pos], tile);

				if (mover.Path.Count > 0)
					pos.Pos = mover.Path.Dequeue().Position;
				
				// TODO subscribe on some sort of cancel click?

				// only unsubscribe when everything has been dealt with
				InputController.Instance.OnTileClicked -= TileTapped;

				// hand control back over to the game
				FinishActorHandling();
			}

		}
		
		private void FinishActorHandling()
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


		private void SetPath(Mover mover, TileData currentPos, TileData target)
		{
			mover.Path = new Queue<TileData>(PathFinder<TileData>.FindPath(currentPos, target));
			// remove start pos from path
			mover.Path.Dequeue();
		}
	}
}
