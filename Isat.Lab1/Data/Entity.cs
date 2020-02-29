using System.Collections.Generic;

namespace Isat.Lab1.Data
{
    public class Entity
    {
        public List<decimal> Parameters { get; set; }
        public string ClassName { get; set; }
        public int ClassNumber { get; set; }
        public List<int> Classes { get; set; }


        public Entity(List<decimal> parameters, string className)
        {
            Parameters = parameters;
            ClassName = className;
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
            return result;
        }
    }
}
