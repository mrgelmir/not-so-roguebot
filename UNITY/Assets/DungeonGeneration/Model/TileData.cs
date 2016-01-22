using System;

namespace DungeonGeneration
{
	public class TileData
	{

		#region Data
		private readonly int column;
		private readonly int row;

		// determines the functionality of the tile
		private DungeonTileType type = DungeonTileType.None;

		// to group tiles together
		private int roomIndex = -1;

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
						OnTileChanged(this);
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
		#endregion

		#region Callbacks
		/// <summary>
		/// Gets called everytime this tile's visual changes
		/// </summary>
		public Action<TileData> OnTileChanged;   
		#endregion

		#region Functions
		/// <summary>
		/// Create an empty grid tile at the specified Column and Row.
		/// </summary>
		/// <param name="column">Column</param>
		/// <param name="row">Row</param>
		public TileData(int column, int row)
		{
			this.column = column;
			this.row = row;
			type = DungeonTileType.None;
		}

		/// <summary>
		/// Create an empty grid tile of the specified Type at the specified Column and Row.
		/// </summary>
		/// <param name="column">Column</param>
		/// <param name="row">Row</param>
		/// <param name="type"></param>
		public TileData(int column, int row, DungeonTileType type)
		{

		}

		public override string ToString()
		{
			return Type.ToString() + " Tile - " + Column + ":" + Row;
		}
		#endregion
	}


}
