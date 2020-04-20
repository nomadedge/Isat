using System;
using System.Collections.Generic;

namespace Isat.TaskL
{
    class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Entity(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Helper
    {
        public double Y { get; set; }
        public double YSquare { get; set; }
        public int Count { get; set; }
    }

    class Solution
    {
        public int KX { get; set; }
        public int EntitiesCount { get; set; }
        public List<Entity> Entities { get; set; }
        public List<Helper> Helpers { get; set; }
        public double ConditionalDispersy { get; set; }

        public Solution(int kX, int entitiesCount)
        {
            KX = kX;
            EntitiesCount = entitiesCount;
            Entities = new List<Entity>(EntitiesCount);
            Helpers = new List<Helper>(KX);

            for (int i = 0; i < KX; i++)
            {
                Helpers.Add(new Helper());
            }

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
            foreach (var entity in Entities)
            {
                Helpers[entity.X - 1].Count++;
                Helpers[entity.X - 1].Y += entity.Y;
                Helpers[entity.X - 1].YSquare += Math.Pow(entity.Y, 2);
            }

            var conditionalDispersy = 0d;

            foreach (var helper in Helpers)
            {
                if (helper.Count != 0)
                {
                    conditionalDispersy +=
                        (helper.YSquare / helper.Count - Math.Pow(helper.Y / helper.Count, 2)) * helper.Count / (double)EntitiesCount;
                }
            }

            ConditionalDispersy = conditionalDispersy;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var kX = Convert.ToInt32(Console.ReadLine());
            var entitiesCount = Convert.ToInt32(Console.ReadLine());
            var solution = new Solution(kX, entitiesCount);

            solution.Solve();

            Console.WriteLine(solution.ConditionalDispersy);
        }
    }
}
