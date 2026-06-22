using NUnit.Framework;
using WheelDemo.Core;
using WheelDemo.Data;

namespace WheelDemo.Tests
{
    // Pure-rules tests: no Unity objects required, so these run fast in EditMode.
    public class ZoneRulesTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(29)]
        [TestCase(31)]
        public void GetTierForZone_NormalZone_IsBronze(int zone)
        {
            Assert.AreEqual(WheelTier.Bronze, ZoneRules.GetTierForZone(zone));
        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(25)]
        [TestCase(35)]
        [TestCase(55)]
        public void GetTierForZone_EveryFifthButNotThirtieth_IsSilver(int zone)
        {
            Assert.AreEqual(WheelTier.Silver, ZoneRules.GetTierForZone(zone));
        }

        [TestCase(30)]
        [TestCase(60)]
        [TestCase(90)]
        [TestCase(120)]
        public void GetTierForZone_EveryThirtieth_IsGolden(int zone)
        {
            Assert.AreEqual(WheelTier.Golden, ZoneRules.GetTierForZone(zone));
        }

        [Test]
        public void GoldenTakesPrecedenceOverSilver()
        {
            // 30 is divisible by both 5 and 30; the super-zone rule must win.
            Assert.AreEqual(WheelTier.Golden, ZoneRules.GetTierForZone(30));
        }

        [TestCase(1, false)]
        [TestCase(4, false)]
        [TestCase(6, false)]
        [TestCase(5, true)]
        [TestCase(10, true)]
        [TestCase(30, true)]
        [TestCase(60, true)]
        public void CanLeaveAtZone_OnlyOnSafeOrSuperZones(int zone, bool expected)
        {
            Assert.AreEqual(expected, ZoneRules.CanLeaveAtZone(zone));
        }

        [Test]
        public void Intervals_MatchDocumentedCadence()
        {
            Assert.AreEqual(5, ZoneRules.SafeZoneInterval);
            Assert.AreEqual(30, ZoneRules.SuperZoneInterval);
        }
    }
}
