using ClientGateway.Models;
using HeartRateZoneService.Domain;

namespace HeartRateZoneService
{
    public static class HeartRateExtensions
    {
        public static HeartRateZone GetHeartRateZone(this HeartRate heartRate, int maximumHeartRate)
        {
            switch (maximumHeartRate)
            {
                case < 50:
                    return HeartRateZone.None;
                case < 59:
                    return HeartRateZone.Zone1;
                case < 69:
                    return HeartRateZone.Zone2;
                case < 79:
                    return HeartRateZone.Zone3;
                case < 89:
                    return HeartRateZone.Zone4;
                case >= 90:
                    return HeartRateZone.Zone5;
                default:
                    return HeartRateZone.None;
            }
        }
    }
}
