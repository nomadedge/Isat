using System.Collections.Generic;

namespace Isat.Lab1.Models
{
    public class Entity
    {
        public List<double> Parameters { get; set; }
        public string ClassName { get; set; }
        public int ClassNumber { get; set; }
        public List<int> Classes { get; set; }
        public List<double> NormalizedParameters { get; set; }


        public Entity(List<double> parameters, string className)
        {
            Parameters = parameters;
            ClassName = className;
            NormalizedParameters = new List<double>();
        }

        public override string ToString()
        {
            var result = "";
            foreach (var parameter in Parameters)
            {
                result += $"{parameter},";
            }
            result += $"{ClassName},";
            result += $"{ClassNumber},";
            foreach (var item in Classes)
            {
                result += $"{item},";
            }
            result = result.Remove(result.Length - 1);
            result += "\n";
            foreach (var normalizedParameter in NormalizedParameters)
            {
                result += $"{normalizedParameter},";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }
    }
}
