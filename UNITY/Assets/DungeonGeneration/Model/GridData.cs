using System;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGeneration
{
	public class GridData : IEnumerable<GridTile>
	{
		// Contains all gridTiles
		private GridTile[,] tiles;

		// TODO make a data structure to hold style info?

		public int Columns
		{ get { return tiles.GetLength(0); } }

		public int Rows
		{ get { return tiles.GetLength(1); } }


		public GridData(int columns, int rows)
		{
			tiles = new GridTile[columns, rows];

			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					tiles[column, row] = new GridTile(column, row);
				}
			}
		}

		public GridTile GetTile(int column, int row)
		{
			if (ContainsPosition(column, row))
				return tiles[column, row];
			else
				return null;
		}

		public bool ContainsPosition(int column, int row)
		{
			return column > 0 || column < Columns || row > 0 || row < Rows;
		}

		public IEnumerator<GridTile> GetEnumerator()
		{
			return new GridEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	public class GridEnumerator : IEnumerator<GridTile>
	{
		private GridData grid;
		private int currentColumn = 0;
		private int currentRow = 0;

		public GridEnumerator(GridData grid)
		{
			this.grid = grid;
		}

		public GridTile Current
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
				throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			//throw new NotImplementedException();

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
