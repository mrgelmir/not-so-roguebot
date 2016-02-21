using System;
using System.Collections.Generic;

// For reference: http://www.policyalmanac.org/games/aStarTutorial.htm 

namespace PathFinder
{
	public class PathFinder
	{
		//public static void GetPath(GridTileView from, GridTileView to, Action<IEnumerable<GridTileView>> OnPath)
		//{
		//	//UnityEngine.Debug.Log("requesting path from " + from + " to " + to);
		//	OnPath(FindPath(from, to));
		//}


		//private static IEnumerable<GridTileView> FindPath(GridTileView from, GridTileView to)
		//{
		//	// create pathfinder and let it work its magic
		//	PathFinder p = new PathFinder(from, to);

		//	// return values

		//	List<GridTileView> l = new List<GridTileView>();

		//	foreach (IPathFindeable pf in p.Path)
		//	{
		//		l.Add(pf as GridTileView);
		//	}

		//	return l;
		//}

		private Node currentNode = null;
		private IPathFindeable target = null;
		private List<Node> OpenList = new List<Node>();
		private List<Node> ClosedList = new List<Node>(); // TODO calculate a feasible amount to start with?

		protected List<IPathFindeable> Nodes;
		protected List<int> HeuristicValues;
		protected List<int> MoveValues;

		private PathFinder(IPathFindeable from, IPathFindeable to)
		{
			target = to;
			OpenList.Add(new Node(from, to, null));

			do
			{
				Calculate();
			}
			while (currentNode.PathFindeable != target && OpenList.Count > 0);
		}

		private void Calculate() // TODO find better name
		{
			// find lowest heuristic value in open list (lowest F value)
			OpenList.Sort(); // temp sort here (maybe find more performant stuff?)

			if (OpenList.Count == 0)
			{
				UnityEngine.Debug.Log("No more open list nodes");
				return;
			}
			currentNode = OpenList[0];

			//move node from open list and to closed list
			OpenList.Remove(currentNode);
			ClosedList.Add(currentNode);

			// add its neigbours to open list
			foreach (IPathFindeable pathFindeable in currentNode.PathFindeable.Neighbours)
			{
				Node newNode = new Node(pathFindeable, target, currentNode);
				if (pathFindeable.Walkeable && !OpenList.Contains(newNode) && !ClosedList.Contains(newNode))
				{
					// see if this item exists on open list
					int currentIndex = OpenList.IndexOf(newNode);
					if (currentIndex > 0)
					{
						// check if current node would be a better parent 
						if (OpenList[currentIndex].G > newNode.G)
						{
							OpenList[currentIndex].SetParent(currentNode);
						}
					}
					else
					{
						OpenList.Add(newNode);
					}
				}
			}
		}

		private IEnumerable<IPathFindeable> Path
		{
			get
			{
				List<IPathFindeable> itinerary = new List<IPathFindeable>();
				Node n = currentNode;

				do
				{
					itinerary.Add(n.PathFindeable);
					n = n.Parent;
				}
				while (n != null);

				itinerary.Reverse();

				return itinerary;
			}
		}

		private class Node : IComparable
		{
			public Node Parent = null;
			public IPathFindeable PathFindeable;
			public int G; // total move cost to here
			public int H; // heuristic move distance to target
			public int F { get { return G + H; } }

			public Node(IPathFindeable pathFindeable)
			{
				PathFindeable = pathFindeable;
			}

			public Node(IPathFindeable pathFindeable, IPathFindeable target, Node parent)
			{
				PathFindeable = pathFindeable;
				H = PathFindeable.HeuristicDistance(target);
				SetParent(parent);
			}

			public void SetParent(Node parent)
			{
				Parent = parent;

				if (parent == null)
				{
					G = 0;
				}
				else
				{
					G = parent.G + PathFindeable.MovementCostFrom(parent.PathFindeable);
				}
			}

			public override bool Equals(object other)
			{
				if (other == null)
					return false;

				Node otherNode = other as Node;
				return otherNode == null ? false : this.Equals(otherNode);
			}

			public override int GetHashCode()
			{
				return PathFindeable.GetHashCode();
			}

			public bool Equals(Node other)
			{
				return PathFindeable.Equals(other.PathFindeable);
			}

			public int CompareTo(Node other)
			{
				return other == null ? 1 : F.CompareTo(other.F);
			}

			public int CompareTo(object obj)
			{
				Node otherNode = obj as Node;
				return otherNode == null ? 1 : this.CompareTo(otherNode);
			}
		}
	}

	public interface IPathFindeable
	{
		int HeuristicDistance(IPathFindeable other);
		int MovementCostFrom(IPathFindeable other);
		IEnumerable<IPathFindeable> Neighbours { get; }
		bool Walkeable { get; }
		int UniqueIndex { get; }

	}

}