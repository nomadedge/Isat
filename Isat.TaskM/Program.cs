using System;
using System.Collections.Generic;
using System.Text;

namespace Isat.TaskM
{
    class Solution
    {
        public int KX1 { get; set; }
        public int KX2 { get; set; }
        public int EntitiesCount { get; set; }
        public Dictionary<int, int> X1Counts { get; set; }
        public Dictionary<int, int> X2Counts { get; set; }
        public List<Dictionary<int, int>> PairCounts { get; set; }
        public double Hi2 { get; set; }

        public Solution(int kX1, int kX2, int entitiesCount)
        {
            KX1 = kX1;
            KX2 = kX2;
            EntitiesCount = entitiesCount;

            X1Counts = new Dictionary<int, int>(KX1);
            X2Counts = new Dictionary<int, int>(KX2);
            PairCounts = new List<Dictionary<int, int>>(KX1);
            for (int i = 0; i < KX1; i++)
            {
                PairCounts.Add(new Dictionary<int, int>());
            }

            ReadEntities();
        }

        private void ReadEntities()
        {
            for (int i = 0; i < EntitiesCount; i++)
            {
                var values = Console.ReadLine().Split(' ');
                var x1 = Convert.ToInt32(values[0]) - 1;
                var x2 = Convert.ToInt32(values[1]) - 1;
                
                if (X1Counts.ContainsKey(x1))
                {
                    X1Counts[x1] += 1;
                }
                else
                {
                    X1Counts.Add(x1, 1);
                }

                if (X2Counts.ContainsKey(x2))
                {
                    X2Counts[x2] += 1;
                }
                else
                {
                    X2Counts.Add(x2, 1);
                }

                if (PairCounts[x1].ContainsKey(x2))
                {
                    PairCounts[x1][x2] += 1;
                }
                else
                {
                    PairCounts[x1].Add(x2, 1);
                }
            }
        }

        public void Solve()
        {
            double hi2 = EntitiesCount;

            for (int i = 0; i < KX1; i++)
            {
                foreach (var pairCount in PairCounts[i])
                {
                    var value = X1Counts[i] * X2Counts[pairCount.Key] / (double)EntitiesCount;
                    hi2 += Math.Pow(pairCount.Value - value, 2) / value - value;
                }
            }

            Hi2 = hi2;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var values = Console.ReadLine().Split(' ');
            var kX1 = Convert.ToInt32(values[0]);
            var kX2 = Convert.ToInt32(values[1]);
            var entitiesCount = Convert.ToInt32(Console.ReadLine());

            var solution = new Solution(kX1, kX2, entitiesCount);

            solution.Solve();

            Console.WriteLine(solution.Hi2);
        }
    }
}
