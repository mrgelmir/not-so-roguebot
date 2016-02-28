using Microsoft.VisualStudio.TestTools.UnitTesting;
using GridCode;

namespace GridCode.Tests
{
	[TestClass()]
	public class GridPositionExtensionsTests
	{
		[TestMethod()]
		public void MoveByTest_Cardinal()
		{

			GridPosition pos = new GridPosition(5, 5);

			Assert.IsTrue(pos.MoveBy(GridDirection.None, 10) == pos);
			Assert.IsTrue(pos.MoveBy(GridDirection.North, 0) == pos);
			Assert.IsTrue(pos.MoveBy(GridDirection.North, 2) == new GridPosition(5, 7));
			Assert.IsTrue(pos.MoveBy(GridDirection.East, 3) == new GridPosition(8, 5));
			Assert.IsTrue(pos.MoveBy(GridDirection.South, 1) == new GridPosition(5, 4));
			Assert.IsTrue(pos.MoveBy(GridDirection.West, 4) == new GridPosition(1, 5));


		}

		[TestMethod()]
		public void MoveByTest_Diagonal()
		{
			GridPosition pos = new GridPosition(5, 5);

			Assert.IsTrue(pos.MoveBy(GridDirection.NorthEast, 2) == new GridPosition(7, 7));
		}
	}
}