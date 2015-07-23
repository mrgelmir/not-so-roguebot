using System;
using System.Collections;
using System.Collections.Generic;

// For reference: http://www.policyalmanac.org/games/aStarTutorial.htm 

public class PathFinder
{
	public static void GetPath(GridTile from, GridTile to, Action<List<GridTile>> OnPath)
	{
		OnPath(FindPath(from, to));
	}

	private static List<GridTile> FindPath(GridTile from, GridTile to)
	{
		Node<GridTile> currentNode = new Node<GridTile>(from);
		List<Node<GridTile>> OpenList = new List<Node<GridTile>>();
		List<Node<GridTile>> ClosedList = new List<Node<GridTile>>(); // TODO calculate a feasible amount to start with

		
		return null;
	}

	private class Node<T> where T : class, IPathFindeable
	{
		public T Parent = null;
		public T PathFindeable;
		public int G;
		public int H;
		public int F { get { return G + F; } }

		public Node(T pathFindeable)
		{
			PathFindeable = pathFindeable;
		}
	}
}

interface IPathFindeable
{
	/*
	int HeuristicDistance(IPathFindeable other); // Manhattan method?
	int MovementCost(IPathFindeable other); // check if can reach first
	IEnumerable<IPathFindeable> GetNeighbours();
	*/
}
