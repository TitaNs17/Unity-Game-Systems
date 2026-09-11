#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.AI;

namespace UnityGameSystems.Tests
{
    public sealed class AIStateMachineTests
    {
        private sealed class TestState : IAIState
        {
            public int EnterCount { get; private set; }
            public int TickCount { get; private set; }
            public int ExitCount { get; private set; }

            public void Enter() => EnterCount++;
            public void Tick(float deltaTime) => TickCount++;
            public void Exit() => ExitCount++;
        }

        [Test]
        public void RegisteredStatesTransitionCleanly()
        {
            var machine = new AIStateMachine();
            var first = new TestState();
            var second = new TestState();

            machine.Register(first);
            machine.Register(second);
            machine.Change<TestState>();

            Assert.AreSame(second, machine.Current);
            Assert.AreEqual(1, second.EnterCount);
        }

        [Test]
        public void StopExitsCurrentState()
        {
            var machine = new AIStateMachine();
            var state = new TestState();
            machine.Register(state);
            machine.Change<TestState>();
            machine.Stop();

            Assert.IsNull(machine.Current);
            Assert.AreEqual(1, state.ExitCount);
        }
    }
}
#endif
