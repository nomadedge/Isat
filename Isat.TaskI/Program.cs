using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskI
{
    class Entity
    {
        public double X1 { get; set; }
        public double X2 { get; set; }

        public Entity(int x1, int x2)
        {
            X1 = x1;
            X2 = x2;
        }
    }

    class Solution
    {
        public int EntitiesCount { get; set; }
        public List<Entity> Entities { get; set; }
        public double Pirson { get; set; }

        public Solution(int entitiesCount)
        {
            EntitiesCount = entitiesCount;
            Entities = new List<Entity>(entitiesCount);

            ReadEntities();
        }

        private void ReadEntities()
        {
            for (int i = 0; i < EntitiesCount; i++)
            {
                var values = Console.ReadLine().Split(' ');
                Entities.Add(new Entity(Convert.ToInt32(values[0]), Convert.ToInt32(values[1])));
            }
        }

        public void Solve()
        {
            var x1Mean = Entities.Average(e => e.X1);
            var x2Mean = Entities.Average(e => e.X2);

            var nominator = 0d;
            var x1Dispersion = 0d;
            var x2Dispersion = 0d;

            foreach (var entity in Entities)
            {
                var x1Difference = entity.X1 - x1Mean;
                var x2Difference = entity.X2 - x2Mean;

                nominator += x1Difference * x2Difference;
                x1Dispersion += Math.Pow(x1Difference, 2);
                x2Dispersion += Math.Pow(x2Difference, 2);
            }

            var denominator = Math.Sqrt(x1Dispersion * x2Dispersion);

            Pirson = denominator == 0 ? 0 : nominator / denominator;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var entitiesCount = Convert.ToInt32(Console.ReadLine());
            var solution = new Solution(entitiesCount);

            solution.Solve();

            Console.WriteLine(solution.Pirson);
        }
    }
}
