using UnityEngine;

namespace GridCode
{
	/// <summary>
	/// Contains extensions to data classes which should not be contained in the Model (ie. Unity references)
	/// </summary>
	public static class GridExtensions
	{
		public static Vector3 ToWorldPos(this GridPosition pos)
		{
			return new Vector3(pos.Column, 0f, pos.Row);
		}

		public static Vector3 ToEulerAngles(this GridDirection dir)
		{
			return Vector3.up * dir.Rotation();
		}

		public static Vector3 ToDirection(this GridDirection dir)
		{
			return new Vector3(dir.GetHorizontalDirection(), 0f, dir.GetVerticalDirection());
		}


	}
}
