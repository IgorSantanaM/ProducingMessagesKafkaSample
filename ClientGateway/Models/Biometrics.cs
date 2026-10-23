namespace ClientGateway.Models
{
    public class Biometrics
    {
        public Guid DeviceId { get; set; }
        public List<HeartRate>? HeartRate { get; set; } = new();
        public List<StepCount>? StepCount { get; set; } = new();
        public int? MaxHeartRate { get; set; }

        public Biometrics(Guid deviceId, List<HeartRate>? heartRate = null, List<StepCount>? stepCount = null, int? maxHeartRate = null)
        { 
            DeviceId = deviceId;
            HeartRate = heartRate; 
            StepCount = stepCount; 
            MaxHeartRate = maxHeartRate;
        }
    }
}
