using System;
using System.Collections;
using System.Collections.Generic;

namespace GridCode
{
	public class GridData : IEnumerable<TileData>
	{
		// Contains all gridTiles
		private TileData[,] tiles;

		// Contains all tile objects
		private List<TileObjectData> tileObjects;

		private Random rand;

		// TODO make a data structure to hold style info?

		public int Columns
		{ get { return tiles.GetLength(0); } }

		public int Rows
		{ get { return tiles.GetLength(1); } }

		public Action<TileData> OnTileChanged;
		public Action<TileData> OnTileObjectChanged;

		public GridData(int columns, int rows)
		{
			rand = new Random(DateTime.Now.Millisecond);

			tiles = new TileData[columns, rows];

			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					tiles[column, row] = new TileData(this, column, row);
					tiles[column, row].OnTileChanged += TileChanged;
					tiles[column, row].OnObjectChanged += TileObjectChanged;
				}
			}
		}
		public TileData GetTile(int column, int row)
		{
			if (ContainsPosition(column, row))
				return tiles[column, row];
			else
				return null;
		}

		public TileData GetRandomTile(GridTileType tileType = GridTileType.Flat)
		{
			TileData tile = null;
			int maxAttempts = 100;		

			do
			{
				tile = tiles[rand.Next(Columns), rand.Next(Rows)];
			}
			while (tile.Type != tileType && --maxAttempts > 0);

			return tile;
		}

		public bool ContainsPosition(int column, int row)
		{
			return column >= 0 && column < Columns && row >= 0 && row < Rows;
		}

		private void TileChanged(TileData tileData)
		{
			if (OnTileChanged != null)
				OnTileChanged(tileData);
		}

		private void TileObjectChanged(TileData tileData)
		{
			if (OnTileObjectChanged != null)
				OnTileObjectChanged(tileData);
		}


		#region IEnumerable implementation
		public IEnumerator<TileData> GetEnumerator()
		{
			return new GridEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator() as IEnumerator;
		}
		#endregion

		#region Indexers
		public TileData this[int row, int column]
		{
			get
			{
				return GetTile(row, column);
			}
		}

		public TileData this[GridPosition pos]
		{
			get
			{
				return GetTile(pos.Column, pos.Row);
			}
		}
		#endregion
	}

	public class GridEnumerator : IEnumerator<TileData>
	{
		private GridData grid;
		private int currentColumn = 0;
		private int currentRow = 0;

		public GridEnumerator(GridData grid)
		{
			this.grid = grid;
		}

		public TileData Current
		{
			get
			{
				return grid.GetTile(currentColumn, currentRow);
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return Current as object;
			}
		}

		public void Dispose()
		{
			// nothing to dispose here
		}

		public bool MoveNext()
		{
			++currentRow;

			if(currentRow >= grid.Rows)
			{
				currentRow = 0;
				++currentColumn;
				if(currentColumn >= grid.Columns)
				{
					return false;
				}
			}

			return true;
		}

		public void Reset()
		{
			currentColumn = 0;
			currentRow = 0;
		}
	}
}
