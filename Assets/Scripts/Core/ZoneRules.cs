using WheelDemo.Data;

namespace WheelDemo.Core
{
    // Pure rules, no Unity references, so it unit-tests cleanly.
    public static class ZoneRules
    {
        public const int SafeZoneInterval = 5;
        public const int SuperZoneInterval = 30;

        public static WheelTier GetTierForZone(int zone)
        {
            if (zone % SuperZoneInterval == 0) return WheelTier.Golden;
            if (zone % SafeZoneInterval == 0) return WheelTier.Silver;
            return WheelTier.Bronze;
        }

        // Player can only walk away on a risk-free zone.
        public static bool CanLeaveAtZone(int zone)
        {
            var tier = GetTierForZone(zone);
            return tier == WheelTier.Silver || tier == WheelTier.Golden;
        }
    }
}
