using System;

namespace DungeonGeneration
{
	public class TileObjectData
	{
		#region Data
		private TileData tile;
		private readonly string objectType;

		public bool Walkable
		{
			get;
			protected set;
		}

		public bool Tileable
		{
			get;
			protected set;
		}

		public string ObjectCode
		{
			get { return objectType; }
		}

		#endregion

		public TileObjectData(string objectType)
		{
			this.objectType = objectType;
		}

		public override string ToString()
		{
			return "Tile object of type " + objectType;
		}
	}
}
