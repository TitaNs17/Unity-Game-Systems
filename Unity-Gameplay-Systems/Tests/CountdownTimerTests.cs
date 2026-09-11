#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.GameFlow;

namespace UnityGameSystems.Tests
{
    public sealed class CountdownTimerTests
    {
        [Test]
        public void TimerFinishesAfterDuration()
        {
            var timer = new CountdownTimer(2f);
            var finishedCount = 0;
            timer.Finished += () => finishedCount++;

            timer.Start();
            timer.Tick(1f);
            timer.Tick(1f);

            Assert.IsTrue(timer.IsFinished);
            Assert.AreEqual(0f, timer.Remaining);
            Assert.AreEqual(1, finishedCount);
        }

        [Test]
        public void StopPreventsFurtherProgress()
        {
            var timer = new CountdownTimer(5f);
            timer.Start();
            timer.Tick(1f);
            timer.Stop();
            timer.Tick(3f);

            Assert.AreEqual(4f, timer.Remaining);
            Assert.IsFalse(timer.IsRunning);
        }
    }
}
#endif
