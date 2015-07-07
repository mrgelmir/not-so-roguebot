namespace DungeonGeneration
{
	interface IDungeonGenerator
	{
		void Setup(DungeonGenerationInfo info);
		DungeonData GetCurrentGrid();
		bool NextGenerationStep();
	}

	public class DungeonGenerationInfo
	{
		public int Width;
		public int Height;
		public int MaxRooms;

		public int MinRoomWidth;
		public int MinRoomHeight;
		public int MaxRoomWidth;
		public int MaxRoomHeight;
	}
}
