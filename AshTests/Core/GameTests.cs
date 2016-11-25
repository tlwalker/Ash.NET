using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Net.RichardLord.Ash.Core;
using NUnit.Framework;
namespace Net.RichardLord.AshTests.Core
{
    [TestFixture]
    class GameTests
    {
	    private IGame _game;

        [SetUp]
        public void CreateGame()
        {
            _game = new Game<MockFamily>();
            MockFamily.Reset();
        }

        [TearDown]
        public void ClearGame()
        {
            _game = null;
        }

        [Test]
        public void TestAddEntityChecksWithAllFamilies()
        {
            _game.GetNodeList<MockNode>();
            _game.GetNodeList<MockNode2>();
            var entity = new Entity();
            _game.AddEntity(entity);

            var results = new List<int>
            {
                MockFamily.Instances[0].NewEntityCalls,
                MockFamily.Instances[1].NewEntityCalls
            };
            Assert.That(results, Is.All.EqualTo(1));
        }

        [Test]
        public void TestRemoveEntityChecksWithAllFamilies()
        {
            _game.GetNodeList<MockNode>();
            _game.GetNodeList<MockNode2>();
            var entity = new Entity();
            _game.AddEntity(entity);
            _game.RemoveEntity(entity);
            var results = new List<int>
            {
                MockFamily.Instances[0].RemoveEntityCalls,
                MockFamily.Instances[1].RemoveEntityCalls
            };
            Assert.That(results, Is.All.EqualTo(1));
        }

        [Test]
        public void TestRemoveAllEntitiesChecksWithAllFamilies()
        {
            _game.GetNodeList<MockNode>();
            _game.GetNodeList<MockNode2>();
            var entity = new Entity();
            var entity2 = new Entity();
            _game.AddEntity(entity);
            _game.AddEntity(entity2);
            _game.RemoveAllEntities();
            var results = new List<int>
            {
                MockFamily.Instances[0].RemoveEntityCalls,
                MockFamily.Instances[1].RemoveEntityCalls
            };
            Assert.That(results, Is.All.EqualTo(2));
        }

        [Test]
        public void TestComponentAddedChecksWithAllFamilies()
        {
            _game.GetNodeList<MockNode>();
            _game.GetNodeList<MockNode2>();
            var entity = new Entity();
            _game.AddEntity(entity);
            entity.Add(new Point());
            var results = new List<int>
            {
                MockFamily.Instances[0].ComponentAddedCalls,
                MockFamily.Instances[1].ComponentAddedCalls
            };
            Assert.That(results, Is.All.EqualTo(1));
        }

        [Test]
        public void TestComponentRemovedChecksWithAllFamilies()
        {
            _game.GetNodeList<MockNode>();
            _game.GetNodeList<MockNode2>();
            var entity = new Entity();
            _game.AddEntity(entity);
            entity.Add(new Point());
            entity.Remove<Point>();
            var results = new List<int>
            {
                MockFamily.Instances[0].ComponentRemovedCalls,
                MockFamily.Instances[1].ComponentRemovedCalls
            };
            Assert.That(results, Is.All.EqualTo(1));
        }

        [Test]
        public void TestGetNodeListCreatesFamily()
        {
            _game.GetNodeList<MockNode>();
            Assert.That(MockFamily.Instances, Is.Not.Empty);
        }

        [Test]
        public void TestGetNodeListChecksAllEntities()
        {
            _game.AddEntity(new Entity());
            _game.AddEntity(new Entity());
            _game.GetNodeList<MockNode>();
            Assert.That(MockFamily.Instances[0].NewEntityCalls, Is.EqualTo(2));
        }

        [Test]
        public void testReleaseNodeListCallsCleanUp()
        {
            _game.GetNodeList<MockNode>();
            _game.ReleaseNodeList<MockNode>();
            Assert.That(MockFamily.Instances[0].CleanUpCalls, Is.EqualTo(1));
        }

        class MockNode : Node
        {
            public Point Point { get; set; }
        }

        class MockNode2 : Node
        {
            public Matrix Matrix { get; set; }
        }
    }
}
