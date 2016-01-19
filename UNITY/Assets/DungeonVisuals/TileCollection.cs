using UnityEngine;

namespace DungeonVisuals
{
	class TileCollection : ScriptableObject
	{
		public GameObject TopLeft;
		public GameObject Top;
		public GameObject TopRight;
		public GameObject Right;
		public GameObject BottomRight;
		public GameObject Bottom;
		public GameObject BottomLeft;
		public GameObject Left;

		public GameObject Center;

		public GameObject DoorTop;
		public GameObject DoorRight;
		public GameObject DoorBottom;
		public GameObject DoorLeft;

		// TODO think this over ( one collection per type? )
		public GameObject Wall;
	}
}
