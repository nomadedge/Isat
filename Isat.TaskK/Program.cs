using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskK
{
    class Solution
    {
        public int YCount { get; set; }
        public int EntitiesCount { get; set; }
        public Dictionary<long, List<long>> Entities { get; set; }
        public List<Tuple<long, long>> EntitiesOriginal { get; set; }
        public long AllDistance { get; set; }
        public long InnerDistance { get; set; }
        public long OuterDistance { get; set; }

        public Solution(int yCount, int entitiesCount)
        {
            YCount = yCount;
            EntitiesCount = entitiesCount;
            Entities = new Dictionary<long, List<long>>(yCount);
            EntitiesOriginal = new List<Tuple<long, long>>();

            ReadEntities();
        }

        private void ReadEntities()
        {
            long allDistance = 0;

            for (int i = 0; i < EntitiesCount; i++)
            {
                var values = Console.ReadLine().Split(' ');
                var x = Convert.ToInt64(values[0]);
                var y = Convert.ToInt64(values[1]);
                EntitiesOriginal.Add(new Tuple<long, long>(x, y));
            }

            EntitiesOriginal = EntitiesOriginal.OrderBy(eo => eo.Item1).ToList();

            for (int i = 0; i < EntitiesCount; i++)
            {
                allDistance += EntitiesOriginal[i].Item1 * (2 * i - EntitiesCount + 1);
                if (!Entities.ContainsKey(EntitiesOriginal[i].Item2))
                {
                    Entities.Add(EntitiesOriginal[i].Item2, new List<long>());
                }
                Entities[EntitiesOriginal[i].Item2].Add(EntitiesOriginal[i].Item1);
            }

            AllDistance = Math.Abs(allDistance * 2);
        }

        public void Solve()
        {
            long innerDistance = 0;

            for (int i = 1; i < YCount + 1; i++)
            {
                if (Entities.ContainsKey(i))
                {
                    for (int j = 0; j < Entities[i].Count; j++)
                    {
                        innerDistance += Entities[i][j] * (2 * j - Entities[i].Count + 1);
                    }
                }
            }

            InnerDistance = innerDistance * 2;
            OuterDistance = AllDistance - InnerDistance;
        }

        public void Print()
        {
            Console.WriteLine(InnerDistance);
            Console.WriteLine(OuterDistance);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var yCount = Convert.ToInt32(Console.ReadLine());
            var entitiesCount = Convert.ToInt32(Console.ReadLine());

            var solution = new Solution(yCount, entitiesCount);

            solution.Solve();

            solution.Print();
        }
    }
}
