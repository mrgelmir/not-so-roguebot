namespace PathFinding
{
	public class Edge<T>
	{
		/// <summary>
		/// Cost of traversing this edge
		/// TODO: make this adabtable depending on type
		/// (walking over lava is a no-go unless you take no damage from it or a live depends on it)
		/// (flying is only possible for certain characters)
		/// -> an edge will probably need a type later
		/// </summary>
		public int Cost;

		/// <summary>
		/// The Node this edge leads to
		/// </summary>
		public readonly Node<T> Node;

		public Edge(Node<T> targetNode)
		{
			Node = targetNode;
		}
	}
}
