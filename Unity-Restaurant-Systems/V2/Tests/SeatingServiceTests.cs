#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityGameSystems.Restaurant.Seating;

namespace UnityGameSystems.Restaurant.Tests
{
    public sealed class SeatingServiceTests
    {
        [Test]
        public void FindsSmallestTableThatFitsParty()
        {
            var service = new SeatingService();
            var tableForTwo = new RestaurantTable(2);
            var tableForFour = new RestaurantTable(4);
            var tableForSix = new RestaurantTable(6);

            service.Register(tableForSix);
            service.Register(tableForFour);
            service.Register(tableForTwo);

            var result = service.FindBestTable(3);

            Assert.AreSame(tableForFour, result);
        }

        [Test]
        public void ReservedTableIsSkippedForWalkInParty()
        {
            var service = new SeatingService();
            var reserved = new RestaurantTable(4);
            var open = new RestaurantTable(4);
            reserved.TryReserve("booking-1");

            service.Register(reserved);
            service.Register(open);

            Assert.AreSame(open, service.FindBestTable(2));
        }
    }
}
#endif
