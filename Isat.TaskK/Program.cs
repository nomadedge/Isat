using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskK
{
    class Solution
    {
        public int YCount { get; set; }
        public int EntitiesCount { get; set; }
        public Dictionary<int, List<int>> Entities { get; set; }
        public List<Tuple<int, int>> EntitiesOriginal { get; set; }
        public long AllDistance { get; set; }
        public long InnerDistance { get; set; }
        public long OuterDistance { get; set; }

        public Solution(int yCount, int entitiesCount)
        {
            YCount = yCount;
            EntitiesCount = entitiesCount;
            Entities = new Dictionary<int, List<int>>(yCount);
            EntitiesOriginal = new List<Tuple<int, int>>();

            for (int i = 1; i < yCount + 1; i++)
            {
                Entities.Add(i, new List<int>());
            }

            ReadEntities();
        }

        private void ReadEntities()
        {
            long allDistance = 0;

            for (int i = 0; i < EntitiesCount; i++)
            {
                var values = Console.ReadLine().Split(' ');
                var x = Convert.ToInt32(values[0]);
                var y = Convert.ToInt32(values[1]);
                EntitiesOriginal.Add(new Tuple<int, int>(x, y));
            }

            EntitiesOriginal = EntitiesOriginal.OrderBy(eo => eo.Item1).ToList();

            for (int i = 0; i < EntitiesCount; i++)
            {
                Entities[EntitiesOriginal[i].Item2].Add(EntitiesOriginal[i].Item1);

                var difference = 2 * EntitiesCount - 2;
                allDistance += EntitiesOriginal[i].Item1 * (4 * i - difference);
            }

            AllDistance = Math.Abs(allDistance);
        }

        public void Solve()
        {
            long innerDistance = 0;

            for (int i = 1; i < YCount + 1; i++)
            {
                for (int j = 0; j < Entities[i].Count; j++)
                {
                    innerDistance += Entities[i][j] * (2 * j - Entities[i].Count + 1);
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
