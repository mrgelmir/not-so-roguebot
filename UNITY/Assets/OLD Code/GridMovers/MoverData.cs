using UnityEngine;
using System.Collections;

// Data container for actors
[System.Serializable]
public class MoverData
{
	[SerializeField] protected int initiative;
	[SerializeField] protected int hitpoints;
	[SerializeField] protected int strength;
	[SerializeField] protected int actionPoints;

	public int Initiative { get { return initiative; } }
	public int Hitpoints { get { return hitpoints; } }
	public int Strength { get { return strength; } }
	public int ActionPoints { get { return actionPoints; } }
}
