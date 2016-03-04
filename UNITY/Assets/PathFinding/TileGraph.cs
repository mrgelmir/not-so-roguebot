using GridCode;
using System.Collections.Generic;

namespace PathFinding
{

	/// <summary>
	/// Temp graph of the grid
	/// - flat tiles are nodes
	/// - neighbouring tiles are linked by edges
	/// TODO: make generic, or move out of this namespace and into the GridCode one
	/// </summary>
	public class TileGraph
	{
		private Dictionary<TileData, Node<TileData>> nodes;

		public Dictionary<TileData, Node<TileData>> Nodes
		{
			get { return nodes; }
		}

		public TileGraph(GridData grid)
		{
			nodes = new Dictionary<TileData, Node<TileData>>();

			// create node for each tile
			foreach (TileData tile in grid)
			{
				// TODO: take other types of tiles into account later (ie for flying, lava-walking ...)
				if (tile.Type == GridTileType.Flat)
				{
					nodes.Add(tile, new Node<TileData>(tile));
				}
			}


			// loop trough created nodes and create edges
			foreach (Node<TileData> node in nodes.Values)
			{

				// create edges for valid neigbours
				List<Edge<TileData>> neigbourEdges = new List<Edge<TileData>>(8);
				foreach (TileData neighbourTile in grid.GetNeigbours(node.Data, true))
				{
					if (neighbourTile == null)
						continue;

					// TODO: only allow diagonals that do not cross walls
					if (neighbourTile.Type == GridTileType.Flat)
					{
						neigbourEdges.Add(new Edge<TileData>(nodes[neighbourTile]));
					}
				}

				// add edges to node
				node.Edges = neigbourEdges.ToArray();
			}
		}

		// Indexer
		public Node<TileData> this[TileData tile]
		{
			get
			{
				Node<TileData> tileNode;
				if (nodes.TryGetValue(tile, out tileNode))
				{
					return tileNode;
				}
				else
				{
					return null;
				}
			}
		}
	}
}
