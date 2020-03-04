using System;
using System.Collections.Generic;

namespace Isat.TaskD
{
    class Entity
    {
        public List<int> Features { get; set; }
        public int TargetValue { get; set; }

        public Entity(int featuresCount)
        {
            Features = new List<int>(featuresCount);
        }
    }

    class Solution
    {
        public int EntitiesCount { get; set; }
        public int FeaturesCount { get; set; }
        public List<Entity> Entities { get; set; }
        public List<double> Coefficients { get; set; }

        public Solution(
            int entitiesCount,
            int featuresCount,
            List<string> entityStrings)
        {
            EntitiesCount = entitiesCount;
            FeaturesCount = featuresCount;

            Entities = new List<Entity>(EntitiesCount);
            Coefficients = new List<double>(FeaturesCount + 1);

            foreach (var entityString in entityStrings)
            {
                var featureStrings = entityString.Split(' ');
                var entity = new Entity(FeaturesCount);
                for (int i = 0; i < FeaturesCount; i++)
                {
                    entity.Features.Add(Convert.ToInt32(featureStrings[i]));
                }
                entity.TargetValue = Convert.ToInt32(featureStrings[FeaturesCount]);
                Entities.Add(entity);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var countStrings = Console.ReadLine();
            var entitiesCount = Convert.ToInt32(countStrings[0]);
            var featuresCount = Convert.ToInt32(countStrings[1]);
            var entityStrings = new List<string>();
            for (int i = 0; i < entitiesCount; i++)
            {
                entityStrings.Add(Console.ReadLine());
            }

            var solution = new Solution(entitiesCount, featuresCount, entityStrings);


        }
    }
}
