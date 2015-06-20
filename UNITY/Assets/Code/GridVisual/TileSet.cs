using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class TileSet : ScriptableObject
{
	[SerializeField]
	private TileType type;

	public TileType Type
	{
		get { return type; }
	}

	// Actual serialized data objects

	// Outer Corners
	[SerializeField]
	private GridTile topLeft;

	[SerializeField]
	private GridTile topRight;

	[SerializeField]
	private GridTile bottomRight;

	[SerializeField]
	private GridTile bottomLeft;


	[SerializeField]
	private GridTile topLeftInner;
	[SerializeField]
	private GridTile topRightInner;
	[SerializeField]
	private GridTile bottomRightInner;
	[SerializeField]
	private GridTile bottomLeftInner;

	// Straights
	[SerializeField]
	private GridTile top;

	[SerializeField]
	private GridTile bottom;

	[SerializeField]
	private GridTile left;

	[SerializeField]
	private GridTile right;


	// Flat
	[SerializeField]
	private GridTile center;

	// Getters

	public GridTile TopLeft
	{
		get { return topLeft; }
	}
	public GridTile TopRight
	{
		get { return topRight; }
	}
	public GridTile BottomRight
	{
		get { return bottomRight; }
	}
	public GridTile BottomLeft
	{
		get { return bottomLeft; }
	}
	public GridTile Top
	{
		get { return top; }
	}
	public GridTile Bottom
	{
		get { return bottom; }
	}
	public GridTile Left
	{
		get { return left; }
	}
	public GridTile Right
	{
		get { return right; }
	}
	public GridTile Center
	{
		get { return center; }
	}

}
