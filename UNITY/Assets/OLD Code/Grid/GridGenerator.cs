using System;
using DungeonGeneration;
using UnityEngine;

namespace Grid
{
	class GridGenerator : MonoBehaviour
	{
		public GridTypes gridType = GridTypes.BASIC;

		[SerializeField]
		private bool autoGenerate = true;

		[SerializeField]
		private GridController gridController;

		[SerializeField]
		private DungeonGenerationInfo info;
		
		protected void Start()
		{
			if(autoGenerate)
			{
				GenerateGrid();
			}
		}

		[ContextMenu("Generate Grid")]
		public void GenerateGrid()
		{
			gridController.ClearGrid();

			IDungeonGenerator generator = GetGenerator();
			generator.Setup(info);
			DungeonGenerationData data = generator.GenerateDungeon();

			gridController.DrawGrid(data);
		}

		private IDungeonGenerator GetGenerator()
		{
			switch(gridType)
			{
				default:
				case GridTypes.NONE:
				case GridTypes.BASIC:
					return new BasicGenerator();
				case GridTypes.WALK:
					return new DungeonWalkGenerator();
			}
		}
	}

	public enum GridTypes
	{
		NONE,
		BASIC,
		WALK,
		// TODO add
	}
}
