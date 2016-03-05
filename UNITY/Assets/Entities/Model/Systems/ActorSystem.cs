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
			// check if the actor has enough energy to process this turn
			if (++actor.CurrentEnergy >= actor.NeededEnergy)
			{
				actor.CurrentEnergy -= actor.NeededEnergy;
			}
			else
			{
				// not enough energy -> abort
				return true;
			}


			currentEntity = actor.Entity;
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
				// just continue along path if not blocked
				if (MoveToTile(mover, mover.Path.Dequeue()))
				{
					return true;
				}
			}

			if (actor.InstantActor)
			{
				// do stuff here and continue for instant processing

				// create a path to a random position

				TileData tile = grid.GetRandomTile(GridTileType.Flat);

				GetPath(mover, grid[pos.Pos], tile);

				MoveToTile(mover, mover.Path.Dequeue());

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



			// 1. check if we can move to tile

			if (mover.MoveBehaviour.CanEnterTile(tile))
			{
				GetPath(mover, grid[pos.Pos], tile);

				if (mover.Path == null || mover.Path.Count <= 0)
					return;

				MoveToTile(mover, mover.Path.Dequeue());

				// TODO: subscribe on some sort of cancel click?

				// only unsubscribe when everything has been dealt with
				InputController.Instance.OnTileClicked -= TileTapped;

				// hand control back over to the game
				FinishActorHandling();
				return;
			}

			// 2. check if we can interact with tile
			if (tile.Position.Neighbours(pos.Pos))
			{
				Interactable interactable = null;

				foreach (Entity entity in tile.LinkedEntities)
				{
					interactable = entity.GetComponent<Interactable>();

					// if we do not face the interactable, do so
					GridDirection dir = GridDirectionUtil.DirectionBetween(pos.Pos, tile.Position);
					pos.Orientation = dir;

					if (interactable != null)
					{
						Log.Write("actor can interact with tile");
						break;
					}
				}

				if (interactable != null)
				{
					// do interaction
					if (interactable.OnInteract != null)
					{
						interactable.OnInteract(currentEntity);
					}
				}
			}



			// 3. check if ther eis an actor on the tile that we can attack

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
				Log.Warning("ActorSystem::FinishActorHandling - There is no resume callback available, this will probably break the gameloop");
			}
		}

		private void GetPath(Mover mover, TileData from, TileData to)
		{
			try
			{
				// dynamic grid thing
				mover.Path = new Queue<TileData>(PathFinder<TileData>.FindPath(
					from, to, mover.MoveBehaviour.CanPathThroughTile));

				if (mover.Path != null && mover.Path.Count > 0)
				{
					// remove start pos from path
					mover.Path.Dequeue();
				}
			}
			catch (ArgumentNullException e)
			{
				// well, the path will be null 
			}

			//// graph based version
			//Path_AStar path = new Path_AStar(grid, currentPos, target);

			//if (path.Done)
			//{
			//	mover.Path = path.Path;
			//}

		}

		private bool MoveToTile(Mover mover, TileData tile)
		{
			if (mover.MoveBehaviour.CanEnterTile(tile))
			{
				mover.Pos.Pos = tile.Position;
				return true;
			}

			// invalidate path
			mover.InvalidatePath();

			return false;
		}

		private bool IsValidTile(Mover mover, TileData tile)
		{
			// check if tile is valid
			return mover.MoveBehaviour.CanEnterTile(tile);
		}


	}
}
