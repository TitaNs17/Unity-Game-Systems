#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityEngine;
using UnityGameSystems.Abilities;

namespace UnityGameSystems.Tests
{
    public sealed class CooldownTrackerTests
    {
        [Test]
        public void NewAbilityIsReady()
        {
            var tracker = new CooldownTracker();
            Assert.IsTrue(tracker.IsReady("dash"));
            Assert.AreEqual(0f, tracker.Remaining("dash"));
        }

        [Test]
        public void ResetClearsCooldowns()
        {
            var tracker = new CooldownTracker();
            tracker.Reset();
            Assert.IsTrue(tracker.IsReady("dash"));
        }
    }
}
#endif
