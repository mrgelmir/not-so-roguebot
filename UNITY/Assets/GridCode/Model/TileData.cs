using Entities.Model;
using PathFinding;
using System;
using System.Collections.Generic;

namespace GridCode
{
	public class TileData : IPathFindable<TileData>
	{

		#region Data
		private readonly GridData grid;
		private readonly GridPosition pos;

		// determines the functionality of the tile
		private GridTileType type = GridTileType.None;

		// to group tiles together
		private int roomIndex = -1;

		// TODO add linked entities here
		public List<Entity> LinkedEntities = new List<Entity>();

		// TODO determine the style of the tile?
		#endregion

		#region Accessors
		public GridTileType Type
		{
			get { return type; }
			set
			{
				if (type != value)
				{
					type = value;
					if (OnTileChanged != null)
					{
						OnTileChanged(this);

						// Update Neighbours too
						// TODO add diagonals later

						TileData neighbour_North = grid.GetTile(Column, Row + 1);
						TileData neighbour_South = grid.GetTile(Column, Row - 1);
						TileData neighbour_East = grid.GetTile(Column + 1, Row);
						TileData neighbour_West = grid.GetTile(Column - 1, Row);


						if (neighbour_North != null) OnTileChanged(neighbour_North);
						if (neighbour_South != null) OnTileChanged(neighbour_South);
						if (neighbour_East != null) OnTileChanged(neighbour_East);
						if (neighbour_West != null) OnTileChanged(neighbour_West);
					}
				}
			}
		}

		public int RoomIndex
		{
			get { return roomIndex; }
			set { roomIndex = value; }
		}

		public GridPosition Position
		{
			get { return pos; }
		}

		public int Column
		{
			get { return pos.Column; }
		}

		public int Row
		{
			get { return pos.Row; }
		}

		IEnumerable<TileData> IPathFindable<TileData>.Neighbours
		{
			get
			{
				return grid.GetNeigbours(this, true);
			}
		}
		#endregion

		#region Callbacks
		/// <summary>
		/// Gets called everytime this tile's visual changes
		/// </summary>
		public Action<TileData> OnTileChanged;
		public Action<TileData> OnObjectChanged;
		#endregion

		#region Functions
		/// <summary>
		/// Create an empty grid tile at the specified Column and Row.
		/// </summary>
		/// <param name="column">Column</param>
		/// <param name="row">Row</param>
		public TileData(GridData grid, int column, int row) : this(grid, column, row, GridTileType.None)
		{

		}

		/// <summary>
		/// Create an empty grid tile of the specified Type at the specified Column and Row.
		/// </summary>
		/// <param name="column">Column</param>
		/// <param name="row">Row</param>
		/// <param name="type"></param>
		public TileData(GridData grid, int column, int row, GridTileType type)
		{
			this.grid = grid;
			pos.Column = column;
			pos.Row = row;
			this.type = type;
		}

		public override string ToString()
		{
			return Type.ToString() + " Tile - " + Column + ":" + Row;
		}

		public override bool Equals(object other)
		{
			TileData otherTile = other as TileData;
			return otherTile == null ? false : Equals(otherTile);
		}

		public bool Equals(TileData other)
		{
			// simple for now, only works in a single flattened grid
			return (Column == other.Column && Row == other.Row);
		}

		public override int GetHashCode()
		{
			return Column.GetHashCode() | Row.GetHashCode();
		}

		int IPathFindable<TileData>.HeuristicDistance(TileData other)
		{
			if (other == null)
			{
				// this would mean a non-tiledata IPathFindable would be referenced from a Tiledata
				throw new NotImplementedException("comparing TileData with non-TileData IPathFindable");
			}
			else
			{
				return Position.DistanceTo(other.pos);
			}
		}

		int IPathFindable<TileData>.MovementCostFrom(TileData other)
		{
			// TODO: higher cost for diagonal
			return (other.Column == Column || other.Row == Row) ? 4 : 6;
		}

		bool IPathFindable<TileData>.ValidateMove(TileData to, Func<TileData, bool> validateTile)
		{
			if (!validateTile(to))
				return false;
			
			GridDirection movementDir = GridDirectionUtil.DirectionBetween(Position, to.Position);

			switch (movementDir)
			{
				case GridDirection.None:
				case GridDirection.North:
				case GridDirection.East:
				case GridDirection.South:
				case GridDirection.West:
					// axial movement is always valid
					return true;
				case GridDirection.NorthEast:
				case GridDirection.SouthEast:
				case GridDirection.SouthWest:
				case GridDirection.NorthWest:
					// check neigbouring tiles when moving diagonally
					return validateTile(grid[Position + movementDir.RotateBy(-45)]) &&
						validateTile(grid[Position + movementDir.RotateBy(45)]);
				default:
					// we don't deal with composite directions
					return false;
			}
		}
		#endregion
	}


}
