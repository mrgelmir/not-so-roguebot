//using Priority_Queue;
using System;
using System.Collections.Generic;

// For reference: http://www.policyalmanac.org/games/aStarTutorial.htm 

namespace PathFinding
{
	public class PathFinder<T> where T : class, IPathFindable<T>
	{

		public static IEnumerable<T> FindPath(T from, T to, Func<T,bool> validateTileFunction)
		{
			
			// create pathfinder and let it work its magic
			PathFinder<T> p = new PathFinder<T>(from, to, validateTileFunction);

			// return values

			List<T> l = new List<T>();

			foreach (T pf in p.Path)
			{
				l.Add(pf as T);
			}
			
			return l;
		}

		private Func<T, bool> validateTile;

		private Node currentNode = null;
		private T target = null;
		//private SimplePriorityQueue<Node> OpenList = new SimplePriorityQueue<Node>();
		private List<Node> OpenList = new List<Node>();
		private HashSet<Node> ClosedList = new HashSet<Node>(); // TODO calculate a feasible amount to start with?
		private bool finished = true;

		protected List<T> Nodes;
		protected List<int> HeuristicValues;
		protected List<int> MoveValues;

		private PathFinder(T from, T to, Func<T, bool> validateTileFunction)
		{
			validateTile = validateTileFunction;

			target = to;
			//OpenList.Enqueue(new Node(from, to, null), 0);
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
			OpenList.Sort(); // temp sort here (maybe us more performant container like priorityqueue)

			if (OpenList.Count == 0)
			{
				Log.Write("No more open list nodes");
				finished = false;
                return;
			}
			currentNode = OpenList[0];
			//currentNode = OpenList.Dequeue();

			//move node from open list and to closed list
			OpenList.Remove(currentNode);
			ClosedList.Add(currentNode);

			// add its neigbours to open list
			foreach (T pathFindeable in currentNode.PathFindeable.Neighbours)
			{
				Node newNode = new Node(pathFindeable, target, currentNode);
				if (validateTile(pathFindeable) && !OpenList.Contains(newNode) && !ClosedList.Contains(newNode))
				//if (pathFindeable.Walkeable && !OpenList.Contains(newNode) && !ClosedList.Contains(newNode))
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

		private IEnumerable<T> Path
		{
			get
			{
				if (!finished)
					return null;

				List<T> itinerary = new List<T>();
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
			public T PathFindeable;
			public int G; // total move cost to here
			public int H; // heuristic move distance to target
			public int F { get { return G + H; } }

			public Node(T pathFindeable)
			{
				PathFindeable = pathFindeable;
			}

			public Node(T pathFindeable, T target, Node parent)
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

	public interface IPathFindable<T> where T : class
	{
		int HeuristicDistance(T other);
		int MovementCostFrom(T other);
		IEnumerable<T> Neighbours { get; }

	}
	

}