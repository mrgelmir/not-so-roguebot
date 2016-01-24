using System;

namespace DungeonGeneration
{
	public class TileData
	{

		#region Data
		private readonly GridData grid;
		private readonly int column;
		private readonly int row;

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

		public int Column
		{
			get { return column; }
		}

		public int Row
		{
			get { return row; }
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
			this.column = column;
			this.row = row;
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

		#endregion
	}


}
