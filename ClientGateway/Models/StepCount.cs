namespace ClientGateway.Models
{
    public class StepCount
    {
        public DateTime DateTime { get; set; }
        public int Value { get; set; }
        public StepCount(int value, DateTime dateTime)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}
