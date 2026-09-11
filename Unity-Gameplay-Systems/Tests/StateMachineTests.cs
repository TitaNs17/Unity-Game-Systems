#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.StateMachine;

namespace UnityGameSystems.Tests
{
    public sealed class StateMachineTests
    {
        private sealed class TestState : IState
        {
            public int EnterCount { get; private set; }
            public int TickCount { get; private set; }
            public int ExitCount { get; private set; }

            public void Enter() => EnterCount++;
            public void Tick(float deltaTime) => TickCount++;
            public void Exit() => ExitCount++;
        }

        [Test]
        public void ChangingStateExitsPreviousAndEntersNext()
        {
            var machine = new StateMachine.StateMachine();
            var first = new TestState();
            var second = new TestState();

            machine.SetState(first);
            machine.Tick(0.016f);
            machine.SetState(second);

            Assert.AreEqual(1, first.EnterCount);
            Assert.AreEqual(1, first.TickCount);
            Assert.AreEqual(1, first.ExitCount);
            Assert.AreEqual(1, second.EnterCount);
        }
    }
}
#endif
