#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.Save;

namespace UnityGameSystems.Tests
{
    public sealed class SaveProfileTests
    {
        [Test]
        public void SetUpdatesExistingValueWithoutDuplicates()
        {
            var profile = new SaveProfile();
            profile.Set("coins", "10");
            profile.Set("coins", "25");

            Assert.AreEqual(1, profile.values.Count);
            Assert.IsTrue(profile.TryGet("coins", out var value));
            Assert.AreEqual("25", value);
        }

        [Test]
        public void MissingKeyReturnsFalse()
        {
            var profile = new SaveProfile();
            Assert.IsFalse(profile.TryGet("missing", out _));
        }
    }
}
#endif
