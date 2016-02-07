using System;

namespace GridCode
{
	public class TileData
	{

		#region Data
		private readonly GridData grid;
		private readonly GridPosition pos;

		// determines the functionality of the tile
		private DungeonTileType type = DungeonTileType.None;

		// to group tiles together
		private int roomIndex = -1;

		// the object living in this tile
		private TileObjectData tileObject = null;

		// TODO determine the style of the tile?
		#endregion

		#region Accessors
		public DungeonTileType Type
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

		public TileObjectData ObjectData
		{
			get { return tileObject; }
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
		public TileData(GridData grid, int column, int row) : this(grid, column, row, DungeonTileType.None)
		{

		}

		/// <summary>
		/// Create an empty grid tile of the specified Type at the specified Column and Row.
		/// </summary>
		/// <param name="column">Column</param>
		/// <param name="row">Row</param>
		/// <param name="type"></param>
		public TileData(GridData grid, int column, int row, DungeonTileType type)
		{
			this.grid = grid;
			pos.Column = column;
			pos.Row = row;
			this.type = type;
		}

		public bool AddObject(TileObjectData tileOjbectData)
		{
			// TODO
			tileObject = tileOjbectData;

			// if the object has changed -> do the callback for visuals etc
			if (OnObjectChanged != null)
				OnObjectChanged(this);

			return true;
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
		#endregion
	}


}
