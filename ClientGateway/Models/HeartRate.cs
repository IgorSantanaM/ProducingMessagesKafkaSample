namespace ClientGateway.Models
{
    public class HeartRate
    {
        public DateTime DateTime { get; set; }
        public int Value { get; set; }


        public HeartRate(int value, DateTime dateTime)
        {
            DateTime  = dateTime;
            Value = value;
        }
    }
}
