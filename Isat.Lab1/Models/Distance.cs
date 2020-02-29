namespace Isat.Lab1.Models
{
    public class Distance
    {
        public double Value { get; set; }
        public int EntityIndex { get; set; }

        public Distance(double value, int entityIndex)
        {
            Value = value;
            EntityIndex = entityIndex;
        }
    }
}
