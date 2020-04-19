using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskN
{
    class Solution
    {
        public int Kx { get; set; }
        public int Ky { get; set; }
        public int EntitiesCount { get; set; }
        public Dictionary<int, Dictionary<int, int>> Entities { get; set; }
        public double Entropy { get; set; }

        public Solution(int kx, int ky, int entitiesCount)
        {
            Kx = kx;
            Ky = ky;
            EntitiesCount = entitiesCount;
            Entities = new Dictionary<int, Dictionary<int, int>>(Kx);

            for (int i = 1; i < kx + 1; i++)
            {
                Entities.Add(i, new Dictionary<int, int>());
            }

            ReadEntities();
        }

        private void ReadEntities()
        {
            for (int i = 0; i < EntitiesCount; i++)
            {
                var values = Console.ReadLine().Split(' ');
                var x = Convert.ToInt32(values[0]);
                var y = Convert.ToInt32(values[1]);
                if (Entities[x].ContainsKey(y))
                {
                    Entities[x][y]++;
                }
                else
                {
                    Entities[x].Add(y, 1);
                }
            }
        }

        public void Solve()
        {
            var entropy = 0d;

            for (int i = 1; i < Kx + 1; i++)
            {
                var conditionalProbability = 0d;
                var xCount = Entities[i].Sum(e => e.Value);

                if (xCount != 0)
                {
                    foreach (var entity in Entities[i])
                    {
                        var value = entity.Value / (double)xCount;
                        conditionalProbability += value * Math.Log(value);
                    }
                    entropy += conditionalProbability * xCount / EntitiesCount;
                }
            }

            Entropy = -entropy;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ks = Console.ReadLine().Split(' ');
            var kx = Convert.ToInt32(ks[0]);
            var ky = Convert.ToInt32(ks[1]);
            var entitiesCount = Convert.ToInt32(Console.ReadLine());

            var solution = new Solution(kx, ky, entitiesCount);

            solution.Solve();

            Console.WriteLine(solution.Entropy);
        }
    }
}
