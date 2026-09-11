#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.Progression;

namespace UnityGameSystems.Tests
{
    public sealed class ExperienceProgressionTests
    {
        [Test]
        public void ExperienceCarriesAcrossMultipleLevels()
        {
            var progression = new ExperienceProgression(level => 100);

            progression.AddExperience(250);

            Assert.AreEqual(3, progression.Level);
            Assert.AreEqual(50, progression.CurrentXp);
            Assert.AreEqual(250, progression.TotalXp);
        }

        [Test]
        public void NonPositiveExperienceIsIgnored()
        {
            var progression = new ExperienceProgression(level => 100);

            progression.AddExperience(0);
            progression.AddExperience(-20);

            Assert.AreEqual(1, progression.Level);
            Assert.AreEqual(0, progression.CurrentXp);
        }

        [Test]
        public void RestoreStateNormalizesOverflowingExperience()
        {
            var progression = new ExperienceProgression(level => 100);

            progression.SetState(2, 220, 320);

            Assert.AreEqual(4, progression.Level);
            Assert.AreEqual(20, progression.CurrentXp);
        }
    }
}
#endif
