using System;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class GridData : IEnumerable<TileData>
	{
		// Contains all gridTiles
		private TileData[,] tiles;

		// TODO make a data structure to hold style info?

		public int Columns
		{ get { return tiles.GetLength(0); } }

		public int Rows
		{ get { return tiles.GetLength(1); } }


		public GridData(int columns, int rows)
		{
			tiles = new TileData[columns, rows];

			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					tiles[column, row] = new TileData(column, row);
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

		public bool ContainsPosition(int column, int row)
		{
			return column >= 0 && column < Columns && row >= 0 && row < Rows;
		}

		public IEnumerator<TileData> GetEnumerator()
		{
			return new GridEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator() as IEnumerator;
		}
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
