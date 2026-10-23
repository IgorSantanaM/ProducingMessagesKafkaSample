namespace HeartRateZoneService.Domain
{
    public record HeartRateZoneReached(Guid DeviceId, HeartRateZone Zone, DateTime DateTime, int Heartrate, int? MaxHeartRate);
}
