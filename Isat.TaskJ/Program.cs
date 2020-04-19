using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskJ
{
    class Entity
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int X1Order { get; set; }
        public int X2Order { get; set; }

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
        public double Spirman { get; set; }

        public Solution(int entitiesCount)
        {
            EntitiesCount = entitiesCount;
            Entities = new List<Entity>();

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
            Entities = Entities.OrderBy(e => e.X1).ToList();
            for (int i = 0; i < EntitiesCount; i++)
            {
                Entities[i].X1Order = i;
            }

            Entities = Entities.OrderBy(e => e.X2).ToList();
            for (int i = 0; i < EntitiesCount; i++)
            {
                Entities[i].X2Order = i;
            }

            var nominator = 0d;
            foreach (var entity in Entities)
            {
                nominator += Math.Pow(entity.X1Order - entity.X2Order, 2);
            }

            Spirman = 1 - (6 * nominator / (Math.Pow(EntitiesCount, 3) - EntitiesCount));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var entitiesCount = Convert.ToInt32(Console.ReadLine());

            var solution = new Solution(entitiesCount);

            solution.Solve();

            Console.WriteLine(solution.Spirman);
        }
    }
}
