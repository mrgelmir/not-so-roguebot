using System;
using System.Collections;
using System.Collections.Generic;

// For reference: http://www.policyalmanac.org/games/aStarTutorial.htm 

public class PathFinder
{
	public static void GetPath(GridTile from, GridTile to, Action<IEnumerable<GridTile>> OnPath)
	{
		UnityEngine.Debug.Log("requesting path from " + from + " to " + to);
		OnPath(FindPath(from, to));
	}


	private static IEnumerable<GridTile> FindPath(GridTile from, GridTile to)
	{
		// create pathfinder and let it work its magic
		PathFinder p = new PathFinder(from, to);

		// return values

		List<GridTile> l = new List<GridTile>();

		foreach (IPathFindeable pf in p.Path)
		{
			l.Add(pf as GridTile);
		}

		return l;
	}

	Node currentNode = null;
	IPathFindeable target = null;
	List<Node> OpenList = new List<Node>();
	List<Node> ClosedList = new List<Node>(); // TODO calculate a feasible amount to start with?

	private PathFinder(IPathFindeable from, IPathFindeable to)
	{
		target = to;
		OpenList.Add(new Node(from));

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

		if(OpenList.Count == 0)
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
			Node newNode = new Node(pathFindeable, currentNode, 10, target);
			if (pathFindeable.Walkeable && !OpenList.Contains(newNode) && !ClosedList.Contains(newNode))
			{
				// TODO calculate movement cost
				OpenList.Add(newNode);
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

		public Node(IPathFindeable pathFindeable, Node parent, int MovementCost, IPathFindeable target)
		{
			Parent = parent;
			PathFindeable = pathFindeable;
			G = MovementCost;
			H = PathFindeable.HeuristicDistance(target);
		}

		public override bool Equals(object other)
		{
			Node otherNode = other as Node;
			return otherNode == null ? false : this.Equals(otherNode);
		}

		public override int GetHashCode()
		{
			return PathFindeable.GetHashCode();
		}

		public bool Equals(Node other)
		{
			return this.PathFindeable.Equals(other.PathFindeable);
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
	
	int HeuristicDistance(IPathFindeable other); // Manhattan method?
	//int MovementCost(IPathFindeable other); // check if can reach first
	IEnumerable<IPathFindeable> Neighbours { get; }
	bool Walkeable { get; }
	
}
