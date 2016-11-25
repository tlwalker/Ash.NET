using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using NUnit.Framework;
using Net.RichardLord.Ash.Core;

namespace Net.RichardLord.AshTests.Core
{
    [TestFixture]
	public class ComponentMatchingFamilyTests
	{
		private IGame _game;
		private IFamily _family;
		
		[SetUp]
		public void CreateFamily()    
		{
			_game = new Game<ComponentMatchingFamily>();
		    _family = new ComponentMatchingFamily();
            _family.Setup(_game, typeof(MockNode));
		}

    	[TearDown]
		public void ClearFamily()    
		{
			_family = null;
			_game = null;
		}
		
		[Test]
		public void TestNodeListIsInitiallyEmpty()    
		{
			var nodes = _family.NodeList;
			Assert.That(nodes.Head, Is.Null);
		}
		
		[Test]
		public void TestMatchingEntityIsAddedWhenAccessNodeListFirst()    
		{
			var nodes = _family.NodeList;
			var entity = new Entity();
			entity.Add(new Point());
			_family.NewEntity(entity);
			Assert.That(nodes.Head.Entity, Is.SameAs(entity));
		}

        [Test]
        public void TestMatchingEntityIsAddedWhenAccessNodeListSecond()
        {
            var entity = new Entity();
            entity.Add(new Point());
            _family.NewEntity(entity);
            var nodes = _family.NodeList;
            Assert.That(nodes.Head.Entity, Is.SameAs(entity));
        }
		
        [Test]
        public void TestNodeContainsEntityProperties()    
        {
            var entity = new Entity();
            var point = new Point(1,2);
            entity.Add(point);
            _family.NewEntity(entity);
            var nodes = _family.NodeList;
            Assert.That(((MockNode)nodes.Head).Point, Is.EqualTo(point));
        }

        [Test]
        public void TestMatchingEntityIsAddedWhenComponentAdded()
        {
            var nodes = _family.NodeList;
            var entity = new Entity();
            entity.Add(new Point());
            _family.ComponentAddedToEntity(entity, typeof(Point));
            Assert.AreSame(entity, nodes.Head.Entity);
            Assert.That(nodes.Head.Entity, Is.SameAs(entity));
        }

        [Test]
        public void TestNonMatchingEntityIsNotAdded()
        {
            var entity = new Entity();
            _family.NewEntity(entity);
            var nodes = _family.NodeList;
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void TestNonMatchingEntityIsNotAddedWhenComponentAdded()
        {
            var entity = new Entity();
            entity.Add(new Matrix());
            _family.ComponentAddedToEntity(entity, typeof(Matrix));
            var nodes = _family.NodeList;
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void TestEntityIsRemovedWhenAccessNodeListFirst()
        {
            var entity = new Entity();
            entity.Add(new Point());
            _family.NewEntity(entity);
            var nodes = _family.NodeList;
            _family.RemoveEntity(entity);
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void TestEntityIsRemovedWhenAccessNodeListSecond()
        {
            var entity = new Entity();
            entity.Add(new Point());
            _family.NewEntity(entity);
            _family.RemoveEntity(entity);
            var nodes = _family.NodeList;
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void TestEntityIsRemovedWhenComponentRemoved()
        {
            var entity = new Entity();
            entity.Add(new Point());
            _family.NewEntity(entity);
            entity.Remove(typeof(Point));
            _family.ComponentRemovedFromEntity(entity, typeof(Point));
            var nodes = _family.NodeList;
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void NodeListContainsOnlyMatchingEntities()
        {
            var entities = new List<Entity>();
            for (var i = 0; i < 5; ++i)
            {
                var entity = new Entity();
                entity.Add(new Point());
                entities.Add(entity);
                _family.NewEntity(entity);
                _family.NewEntity(new Entity());
            }
			
            var nodes = _family.NodeList;
            var results = new List<bool>();
            for(var node = nodes.Head; node != null; node = node.Next)
            {
                results.Add(entities.Contains(node.Entity));
            }

            Assert.That(results, Is.All.True);
        }

        [Test]
        public void NodeListContainsAllMatchingEntities()
        {
            var entities = new List<Entity>();
            for(var i = 0; i < 5; ++i)
            {
                var entity = new Entity();
                entity.Add(new Point());
                entities.Add(entity);
                _family.NewEntity(entity);
                _family.NewEntity(new Entity());
            }

            var nodes = _family.NodeList;
            for (var node = nodes.Head; node != null; node = node.Next)
            {
                entities.RemoveAt(entities.IndexOf(node.Entity));
            }
            Assert.That(entities, Is.Empty);
        }

        [Test]
        public void CleanUpEmptiesNodeList()
        {
            var entity = new Entity();
            entity.Add(new Point());
            _family.NewEntity(entity);
            var nodes = _family.NodeList;
            _family.CleanUp();
            Assert.That(nodes.Head, Is.Null);
        }

        [Test]
        public void CleanUpSetsNextNodeToNull()
        {
            var entities = new List<Entity>();
            for(var i = 0; i < 5; ++i)
            {
                var entity = new Entity();
                entity.Add(new Point());
                entities.Add(entity);
                _family.NewEntity(entity);
            }
			
            var nodes = _family.NodeList;
            var node = nodes.Head.Next;
            _family.CleanUp();
            Assert.That(node.Next, Is.Null);
        }
    
        class MockNode : Node
        {
            public Point Point { get; set; }
        }
    }
}

