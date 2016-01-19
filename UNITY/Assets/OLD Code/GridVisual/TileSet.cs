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
	private GridTileView topLeft;

	[SerializeField]
	private GridTileView topRight;

	[SerializeField]
	private GridTileView bottomRight;

	[SerializeField]
	private GridTileView bottomLeft;


	[SerializeField]
	private GridTileView topLeftInner;
	[SerializeField]
	private GridTileView topRightInner;
	[SerializeField]
	private GridTileView bottomRightInner;
	[SerializeField]
	private GridTileView bottomLeftInner;

	// Straights
	[SerializeField]
	private GridTileView top;

	[SerializeField]
	private GridTileView bottom;

	[SerializeField]
	private GridTileView left;

	[SerializeField]
	private GridTileView right;


	// Flat
	[SerializeField]
	private GridTileView center;

	// Getters

	public GridTileView TopLeft
	{
		get { return topLeft; }
	}
	public GridTileView TopRight
	{
		get { return topRight; }
	}
	public GridTileView BottomRight
	{
		get { return bottomRight; }
	}
	public GridTileView BottomLeft
	{
		get { return bottomLeft; }
	}
	public GridTileView Top
	{
		get { return top; }
	}
	public GridTileView Bottom
	{
		get { return bottom; }
	}
	public GridTileView Left
	{
		get { return left; }
	}
	public GridTileView Right
	{
		get { return right; }
	}
	public GridTileView Center
	{
		get { return center; }
	}

}
