namespace ClientGateway.Models
{
    public class Biometrics
    {
        public Guid DeviceId { get; set; }
        public List<HeartRate> HeartRate { get; set; } = new();
        public List<StepCount> StepCount { get; set; } = new();
        public int MaxHeartRate { get; set; }

        public Biometrics(Guid deviceId, List<HeartRate> heartRate, List<StepCount> stepCount, int maxHeartRate)
        {
            DeviceId = deviceId;
            HeartRate = heartRate; 
            StepCount = stepCount; 
            MaxHeartRate = maxHeartRate;
        }
    }
}
