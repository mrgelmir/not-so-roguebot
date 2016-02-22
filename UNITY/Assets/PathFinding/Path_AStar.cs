using GridCode;
using Priority_Queue;
using System;
using System.Collections.Generic;

namespace PathFinding
{
	class Path_AStar
	{
		private Queue<TileData> path;

		public bool Done
		{
			get
			{
				return path.Count <= 0;
			}
		}
		
		public Path_AStar(GridData grid, TileData from, TileData to)
		{
			TileGraph graph = grid.Graph;

			if(graph == null)
			{
				Log.Error("Path_AStar::ctor - grid does not have a tile graph");
			}



			Node<TileData> fromNode = graph[from];
			Node<TileData> toNode = graph[to];

			if (fromNode == null || toNode == null)
			{
				Log.Error("Path_AStar::ctor - graph does not contain either start or end");
			}

			// following code is based on:
			// https://en.wikipedia.org/wiki/A*_search_algorithm
			
			List<Node<TileData>> closedSet = new List<Node<TileData>>();
			SimplePriorityQueue<Node<TileData>> openSet = new SimplePriorityQueue<Node<TileData>>();
			openSet.Enqueue(fromNode, 0.0);

			Dictionary<Node<TileData>, Node<TileData>> cameFrom = new Dictionary<Node<TileData>, Node<TileData>>();

			// every node should have a cost of infinity by default
			Dictionary<Node<TileData>, int> gScore = new Dictionary<Node<TileData>, int>(graph.Nodes.Count);
			foreach (Node<TileData> node in graph.Nodes.Values)
			{
				gScore[node] = int.MaxValue;
			}
			gScore[fromNode] = HeuristicCostEstimate(fromNode, toNode);


			Dictionary<Node<TileData>, int> fScore = new Dictionary<Node<TileData>, int>(graph.Nodes.Count);
			foreach (Node<TileData> node in graph.Nodes.Values)
			{
				fScore[node] = int.MaxValue;
			}
			fScore[fromNode] = HeuristicCostEstimate(fromNode, toNode);

			while(openSet.Count > 0)
			{
				Node<TileData> currentNode = openSet.Dequeue();

				// check if we found the end
				if(currentNode == toNode)
				{
					// TODO construct path before return
					Log.Write("success");
					return;
				}

				closedSet.Add(currentNode);

				foreach (Edge<TileData> neigbour in currentNode.Edges)
				{
					// ignore tile if already in closed list
					if (closedSet.Contains(neigbour.Node))
						continue;

					int tentativeGScore = gScore[currentNode] + DistanceBetweenNeighbours(currentNode, neigbour.Node);

					if (openSet.Contains(neigbour.Node) && tentativeGScore >= gScore[neigbour.Node])
						continue;

					cameFrom[neigbour.Node] = currentNode;
					gScore[neigbour.Node] = tentativeGScore;
					fScore[neigbour.Node] = gScore[neigbour.Node] + HeuristicCostEstimate(neigbour.Node, toNode);

					if(!openSet.Contains(neigbour.Node))
					{
						openSet.Enqueue(neigbour.Node, fScore[neigbour.Node]);
                    }
				}
            }

			// openSet is empty and target not found -> no path
			Log.Write("no success");
		}

		public TileData GetNextTile()
		{
			if(path != null && path.Count > 0)
			{
				return path.Dequeue();
			}
			else
			{
				return null;
			}
		}

		private int HeuristicCostEstimate(Node<TileData> from, Node<TileData> to)
		{
			// use manhattan distance
			return Math.Abs(from.Data.Row - to.Data.Row) + Math.Abs(from.Data.Column - to.Data.Column);
		}

		private int DistanceBetweenNeighbours(Node<TileData> from, Node<TileData> to)
		{
			// WARNING: this assumes both nodes are neigbours
			// if axial neighbours -> cost of 2, if diagonal neigbours -> cost of 3 
			return (from.Data.Row == to.Data.Row || from.Data.Column == to.Data.Column) ? 2 : 3;
		}

		private void ReconstructPath(Dictionary<Node<TileData>, Node<TileData>> cameFrom, Node<TileData> current)
		{
			// current should be the end point here -> construct path from end to start
			

		}
	}
}
