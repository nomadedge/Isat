namespace Isat.Lab1.Models
{
    public class FMeasureFromEnums
    {
        public Parameters Parameters { get; set; }
        public double Value { get; set; }

        public FMeasureFromEnums(Parameters parameters, double value)
        {
            Parameters = parameters;
            Value = value;
        }
    }
}
