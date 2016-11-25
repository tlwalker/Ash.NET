using System;
using System.Collections.Generic;
using NUnit.Framework;
using Net.RichardLord.Ash.Core;

namespace Net.RichardLord.AshTests.Core
{
    [TestFixture]
	public class EntityTests
	{
        private Entity _entity;

        [SetUp]
		public void CreateEntity()
		{
			_entity = new Entity();
		}

		[TearDown]
		public void ClearEntity()
		{
			_entity = null;
		}

		[Test]
		public void AddReturnsReferenceToEntity()
		{
			var component = new MockComponent();
			var e = _entity.Add(component);
		    Assert.That(_entity, Is.SameAs(e));
		}

        [Test]
        public void CanStoreAndRetrieveComponent()
        {
            var component = new MockComponent();
            _entity.Add(component);
            Assert.That(_entity.Get(typeof(MockComponent)), Is.SameAs(component));
        }

        [Test]
        public void CanStoreAndRetrieveMultipleComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent2();
            _entity.Add(component2);
            Assert.That(_entity.Get(typeof(MockComponent)), Is.SameAs(component1));
            Assert.That(_entity.Get(typeof(MockComponent2)), Is.SameAs(component2));
        }

        [Test]
        public void CanReplaceComponent()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent();
            _entity.Add(component2);
            Assert.That(component2, Is.EqualTo(_entity.Get(typeof(MockComponent))));
        }

        [Test]
        public void CanStoreBaseAndExtendedComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponentExtended();
            _entity.Add(component2);
            Assert.That(component1,Is.EqualTo(_entity.Get(typeof(MockComponent))));
            Assert.That(component2, Is.EqualTo(_entity.Get(typeof(MockComponentExtended))));
        }

        [Test]
        public void CanStoreExtendedComponentAsBaseType()
        {
            var component = new MockComponentExtended();
            _entity.Add(component, typeof(MockComponent));
            Assert.That(component, Is.EqualTo(_entity.Get(typeof(MockComponent))));
        }

        [Test]
        public void GetReturnNullIfNoComponent()
        {
            Assert.That(_entity.Get(typeof(MockComponent)), Is.Null);
        }

        [Test]
        public void WillRetrieveAllComponents()
        {
            var component1 = new MockComponent();
            _entity.Add(component1);
            var component2 = new MockComponent2();
            _entity.Add(component2);
            var all = _entity.GetAll();
            Assert.That(all, Is.EquivalentTo(new List<object> { component1, component2 }));
        }

        [Test]
        public void HasComponentIsFalseIfComponentTypeNotPresent()
        {
            _entity.Add(new MockComponent2());
            Assert.That(_entity.Has(typeof(MockComponent)), Is.False);
        }

        [Test]
        public void HasComponentIsTrueIfComponentTypeIsPresent()
        {
            _entity.Add(new MockComponent());
            Assert.That(_entity.Has(typeof(MockComponent)), Is.True);
        }

        [Test]
        public void CanRemoveComponent()
        {
            var component = new MockComponent();
            _entity.Add(component);
            _entity.Remove(typeof(MockComponent));
            Assert.That(_entity.Has(typeof(MockComponent)), Is.False);
        }

        [Test]
        public void StoringComponentTriggersAddedSignal()
        {
            var eventFired = false;
            var component = new MockComponent();
            _entity.ComponentAdded += (entity, componentType) => eventFired = true;
            _entity.Add(component);
            Assert.That(eventFired, Is.True);
        }

        [Test]
        public void RemovingComponentTriggersRemovedSignal()
        {
            var eventFired = false;
            var component = new MockComponent();
            _entity.Add(component);
            _entity.ComponentRemoved += (entity, componentType) => eventFired = true;
            _entity.Remove(typeof(MockComponent));
            Assert.That(eventFired, Is.True);
        }

        [Test]
        public void ComponentAddedSignalContainsCorrectType()
        {
            Type type = null;
            var component = new MockComponent();
            _entity.ComponentAdded += (entity, componentType) => type = componentType;
            _entity.Add(component);
            Assert.That(type, Is.TypeOf<MockComponent>());
        }

        [Test]
        public void ComponentAddedSignalEnablesAccessToComponentValue()
        {
            var value = 0;
            var component = new MockComponent { Value = 10 };
            _entity.ComponentAdded += 
                (entity, componentType) => value = ((MockComponent)entity.Get(componentType)).Value;
            _entity.Add(component);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void ComponentRemovedSignalContainsCorrectType()
        {
            Type type = null;
            var component = new MockComponent();
            _entity.ComponentRemoved += (entity, componentType) => type = componentType;
            _entity.Add(component);
            _entity.Remove(typeof(MockComponent));
            Assert.That(type, Is.TypeOf<MockComponent>());
        }

        [Test]
        public void ComponentRemovedSignalContainsEntityWithRemovedComponent()
        {
            var component = new MockComponent { Value = 10 };
            _entity.ComponentRemoved +=
                (entity, componentType) => component = (MockComponent)entity.Get(componentType);
            _entity.Add(component);
            _entity.Remove(typeof (MockComponent));
            Assert.That(component, Is.Null);
        }

        [Test]
        public void CloneIsNewReference()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.That(clone, Is.Not.SameAs(_entity));
        }

        [Test]
        public void CloneHasChildComponent()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.That(clone.Has(typeof(MockComponent)), Is.True);
        }

        [Test]
        public void CloneChildComponentIsNewReference()
        {
            _entity.Add(new MockComponent());
            var clone = _entity.Clone();
            Assert.That(clone.Get(typeof(MockComponent)), Is.Not.SameAs(_entity.Get(typeof(MockComponent))));
        }

        [Test]
        public void CloneChildComponentHasSameProperties()
        {
            var component = new MockComponent {Value = 5};
            _entity.Add(component);
            var clone = _entity.Clone();
            Assert.That(clone.Get<MockComponent>(typeof(MockComponent)).Value, Is.EqualTo(5));
        }
	}

    class MockComponent
    {
        public int Value { get; set; }
    }

    class MockComponent2 {}

    class MockComponentExtended : MockComponent {}
}

