using System;
using System.Collections.Generic;
using NUnit.Framework;
using Net.RichardLord.Ash.Core;

namespace Net.RichardLord.AshTests.Core
{
    [TestFixture]
    public class SystemTests
    {
        private IGame _game;

        private SystemBase _system1;
        private SystemBase _system2;

        [SetUp]
        public void CreateEntity()
        {
            _game = new Game<ComponentMatchingFamily>();
        }

        [TearDown]
        public void ClearEntity()
        {
            _game = null;
        }

        [Test]
        public void AddSystemCallsAddToGame()
        {
            var result = new Tuple<string, IGame>(null, null);
            var system = new MockSystem((sys, action, game) => result = new Tuple<string, IGame>(action, (IGame) game));
            _game.AddSystem(system, 0);
            Assert.That(result, Is.EqualTo(Tuple.Create("added", _game)));
        }

        [Test]
        public void RemoveSystemCallsRemovedFromGame()
        {
            var result = new Tuple<string, IGame>(null, null);
            var system = new MockSystem((sys, action, game) => result = new Tuple<string, IGame>(action, (IGame) game));
            _game.AddSystem(system, 0);
            _game.RemoveSystem(system);
            Assert.That(result, Is.EqualTo(Tuple.Create("removed", _game)));
        }

        [Test]
        public void GameCallsUpdateOnSystems()
        {
            var result = new Tuple<string, object>(null, 0);
            var system = new MockSystem((sys, action, time) => result = new Tuple<string, object>(action, time));
            _game.AddSystem(system, 0);
            _game.Update(0.1);
            Assert.That(result, Is.EqualTo(new Tuple<string, object>("update", 0.1)));
        }

        [Test]
        public void DefaultPriorityIsZero()
        {
            var system = new MockSystem((sys, action, game) => { });
            Assert.That(system.Priority, Is.EqualTo(0));
        }

        [Test]
        public void CanSetPriorityWhenAddingSystem()
        {
            var system = new MockSystem((sys, action, game) => { });
            _game.AddSystem(system, 10);
            Assert.That(system.Priority, Is.EqualTo(10));
        }

        [Test]
        public void SystemsUpdatedInPriorityOrderIfSameAsAddOrder()
        {
            var result = new List<SystemBase>();
            _system1 = new MockSystem((sys, action, time) => result.Add(sys));
            _system2 = new MockSystem((sys, action, time) => result.Add(sys));
            _game.AddSystem(_system1, 10);
            _game.AddSystem(_system2, 20);
            result = new List<SystemBase>();
            _game.Update(0.1);
            var expected = new List<SystemBase> {_system1, _system2};
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SystemsUpdatedInPriorityOrderIfReverseOfAddOrder()
        {
            var result = new List<SystemBase>();
            _system1 = new MockSystem((sys, action, time) => result.Add(sys));
            _system2 = new MockSystem((sys, action, time) => result.Add(sys));
            _game.AddSystem(_system2, 20);
            _game.AddSystem(_system1, 10);
            result = new List<SystemBase>();
            _game.Update(0.1);
            var expected = new List<SystemBase> {_system1, _system2};
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void SystemsUpdatedInPriorityOrderIfPrioritiesAreNegative()
        {
            var result = new List<SystemBase>();
            _system1 = new MockSystem((sys, action, time) => result.Add(sys));
            _system2 = new MockSystem((sys, action, time) => result.Add(sys));
            _game.AddSystem(_system1, 10);
            _game.AddSystem(_system2, -20);
            result = new List<SystemBase>();
            _game.Update(0.1);
            var expected = new List<SystemBase> {_system2, _system1};
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void UpdatingIsFalseBeforeUpdate()
        {
            Assert.That(_game.Updating, Is.False);
        }

        [Test]
        public void UpdatingIsTrueDuringUpdate()
        {
            var updating = false;
            var system = new MockSystem((sys, action, time) => updating = _game.Updating);
            _game.AddSystem(system, 0);
            _game.Update(0.1);
            Assert.That(updating, Is.True);
        }

        [Test]
        public void UpdatingIsFalseAfterUpdate()
        {
            var system = new MockSystem((sys, action, time) => { });
            _game.AddSystem(system, 0);
            _game.Update(0.1);
            Assert.That(_game.Updating, Is.False);
        }

        [Test]
        public void CompleteEventIsDispatchedAfterUpdate()
        {
            var eventDispatched = false;
            _game.UpdateComplete += () => eventDispatched = true;
            var system = new MockSystem((sys, action, time) => { });
            _game.AddSystem(system, 0);
            _game.Update(0.1);
            Assert.That(eventDispatched, Is.True);            
        }

        [Test]
        public void GetSystemReturnsTheSystem()
        {
            var system = new MockSystem((sys, action, time) => { });
            _game.AddSystem(system, 0);
            _game.AddSystem(new DummySystem(), 0);
            Assert.AreSame(system, _game.GetSystem(typeof(MockSystem)));
        }

        [Test]
        public void GetSystemReturnsNullIfNoSuchSystem()
        {
            _game.AddSystem(new DummySystem(), 0);
            Assert.That(_game.GetSystem(typeof(MockSystem)), Is.Null);
        }

        [Test]
        public void RemoveAllSystemsDoesWhatItSays()
        {
            _game.AddSystem(new MockSystem((sys, action, time) => { }), 0);
            _game.AddSystem(new DummySystem(), 0);
            _game.RemoveAllSystems();
            var expected = new Tuple<SystemBase, SystemBase>(null, null);
            var results = new Tuple<SystemBase, SystemBase>(_game.GetSystem(typeof (MockSystem)),
                                                            _game.GetSystem(typeof (DummySystem)));
            Assert.That(results, Is.EqualTo(expected));
        }

        [Test]
        public void RemoveSystemAndAddItAgainDontCauseInvalidLinkedList()
        {
            _system1 = new DummySystem();
            _system2 = new DummySystem();
            _game.AddSystem(_system1, 0);
            _game.AddSystem(_system2, 0);
            _game.RemoveSystem(_system1);
            _game.AddSystem(_system1, 0);
            _game.Update(0.1);
            var expected = new Tuple<SystemBase, SystemBase>(null, null);
            var results = new Tuple<SystemBase, SystemBase>(_system2.Previous, _system1.Next);
            Assert.That(results, Is.EqualTo(expected));
        }
    }
}
