using Isat.Lab1.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Isat.Lab1
{
    public class Solution
    {
        public List<Entity> Entities { get; set; }
        public List<string> ClassNames { get; set; }

        public Solution(string fileName)
        {
            Entities = new List<Entity>();
            ClassNames = new List<string>();

            ReadFromCsv(fileName);

        }

        private void ReadFromCsv(string fileName)
        {
            string data = File.ReadAllText(fileName);
            var dataRows = data.Split("\n");

            for (int i = 1; i < dataRows.Length; i++)
            {
                var parameters = new List<decimal>();
                var dataRowStrings = dataRows[i].Split(',');
                for (int j = 1; j < dataRowStrings.Length - 1; j++)
                {
                    parameters.Add(decimal.Parse(dataRowStrings[j]));
                }
                var className = dataRowStrings.Last();
                Entities.Add(new Entity(parameters, className));
                var classNumber = ClassNames.IndexOf(className);
                if (classNumber == -1)
                {
                    ClassNames.Add(className);
                    Entities.Last().ClassNumber = ClassNames.Count - 1;
                }
                else
                {
                    Entities.Last().ClassNumber = classNumber;
                }
            }

            PerformOneHotReduction();
        }

        private void PerformOneHotReduction()
        {
            foreach (var entity in Entities)
            {
                entity.Classes = new List<int>(new int[ClassNames.Count]);
                entity.Classes[entity.ClassNumber] = 1;
            }
        }

        public override string ToString()
        {
            var result = "";
            foreach (var entity in Entities)
            {
                result += $"{entity}\n";
            }
            return result;
        }
    }
}
