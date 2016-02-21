using System;
using System.Collections.Generic;
using GridCode;

namespace PathFinding
{
	public class Node<T>
	{
		/// <summary> 
		///reference to the object this node represents
		/// </summary>
		public readonly T Data;

		/// <summary>
		/// Nodes leading out from this node
		/// </summary>
		public Edge<T>[] Edges;

		public Node(T data)
		{
			Data = data;
		}
		
	}
}
