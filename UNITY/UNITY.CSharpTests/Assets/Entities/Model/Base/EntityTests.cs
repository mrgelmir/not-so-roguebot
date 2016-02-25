using Microsoft.VisualStudio.TestTools.UnitTesting;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridCode;

namespace Entities.Model.Tests
{
	[TestClass()]
	public class EntityTests
	{

		[TestMethod()]
		public void GetComponentTest()
		{
			Entity testEntity = GetTestEntity();
					

			Assert.IsTrue(testEntity.GetComponent<DefaultComponentTest1>() != null);
		}

		[TestMethod()]
		public void AddComponentTest()
		{
			Entity testEntity = GetTestEntity();

			int originalComponentCount = testEntity.ComponentCount;
			
			if (testEntity.HasComponent<AddedComponentTest1>())
			{
				Assert.Fail("test entity should NOT contain Component AddedComponentTest1 by default");
			}
			else
			{
				bool hasAddedEntity = testEntity.AddComponent(new AddedComponentTest1());

				Assert.IsTrue(hasAddedEntity, 
					"AddComponent should return true when adding a component it does not contain");
				Assert.IsTrue(originalComponentCount + 1 == testEntity.ComponentCount,
					"component count should increment on addition");
			}
		}

		[TestMethod()]
		public void AddComponentTest_DuplicateAddition()
		{
			Entity testEntity = GetTestEntity();

			// Duplicate component addition
			int originalComponentCount = testEntity.ComponentCount;
			if (testEntity.HasComponent<DefaultComponentTest1>())
			{
				bool hasAddedEntity = testEntity.AddComponent(new DefaultComponentTest1());

				Assert.IsFalse(hasAddedEntity, "duplicate additions should not happen");
				Assert.IsTrue(originalComponentCount == testEntity.ComponentCount,
					"component count should not change with duplicate addition");
			}
			else
			{
				Assert.Fail("test entity should contain Component DefaultComponentTest1 by default");
			}

		}

		[TestMethod()]
		public void RemoveComponentTest()
		{
			Entity testEntity = GetTestEntity();			

			// Testing single removal
			int componentCount = testEntity.ComponentCount;
			
			if (testEntity.RemoveComponent<DefaultComponentTest1>())
			{
				//Assert.IsTrue(testEntity.ComponentCount + 1 == componentCount, 
				//	"entity's component count has nod decreased by one");
				Assert.IsFalse(testEntity.HasComponent<DefaultComponentTest1>(), 
					"remove succeded, but entity still has component");
			}
			else
			{
				Assert.Fail("the component was not removed correctly");
			}

		}

		[TestMethod()]
		public void EqualsTest()
		{
			Entity testEntity = new Entity(0);
			Entity otherEntity = testEntity;

			// TODO: change some stuff later

			Assert.AreEqual(testEntity, otherEntity);
		}

		private int entityIndex = 0;
		private Entity GetTestEntity()
		{
			Entity testEntity = new Entity(++entityIndex);

			testEntity.AddComponent<DefaultComponentTest1>(new DefaultComponentTest1());
			testEntity.AddComponent<DefaultComponentTest2>(new DefaultComponentTest2());

			Assert.IsTrue(testEntity.HasComponent<DefaultComponentTest1>(), "default component should have DefaultComponentTest1");
			Assert.IsTrue(testEntity.HasComponent<DefaultComponentTest2>(), "default component should have DefaultComponentTest2");

			return testEntity;
		}


		// test components
		private class DefaultComponentTest1 : Components.Component
		{

		}

		private class DefaultComponentTest2 : Components.Component
		{

		}

		private class AddedComponentTest1 : Components.Component
		{

		}

		private class AddedComponentTest2 : Components.Component
		{

		}
	}
}