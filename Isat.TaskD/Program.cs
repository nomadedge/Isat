using System;
using System.Collections.Generic;

namespace Isat.TaskD
{
    class Entity
    {
        public List<double> Features { get; set; }
        public List<double> NormalizedFeatures { get; set; }
        public int TargetValue { get; set; }

        public Entity(int featuresCount)
        {
            Features = new List<double>(featuresCount);
            NormalizedFeatures = new List<double>(featuresCount);
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

            Normalize();
        }

        public void Solve(int epochsCount, double learningRate)
        {
            //Coefficients.AddRange(new double[FeaturesCount + 1]);
            var random = new Random();
            for (int i = 0; i < FeaturesCount + 1; i++)
            {
                Coefficients.Add(random.NextDouble());
            }

            for (int epochNumber = 1; epochNumber < epochsCount + 1; epochNumber++)
            {
                var sumError = 0d;
                foreach (var entity in Entities)
                {
                    var predictedValue = Predict(entity.NormalizedFeatures);
                    var error = predictedValue - entity.TargetValue;
                    sumError += Math.Pow(error, 2);
                    for (int i = 0; i < FeaturesCount; i++)
                    {
                        Coefficients[i] -= learningRate * error * entity.NormalizedFeatures[i];
                    }
                    Coefficients[FeaturesCount] -= learningRate * error;
                }

                Console.WriteLine($"Epoch: {epochNumber}\tError = {sumError}");
                foreach (var coefficient in Coefficients)
                {
                    Console.Write($"{coefficient} ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public double Predict(List<double> features)
        {
            if (features.Count != FeaturesCount)
            {
                return 0;
            }

            var predictedValue = 0d;
            for (int i = 0; i < FeaturesCount; i++)
            {
                predictedValue += features[i] * Coefficients[i];
            }
            predictedValue += Coefficients[FeaturesCount];

            return predictedValue;
        }

        private void Normalize()
        {
            for (int i = 0; i < FeaturesCount; i++)
            {
                var average = 0d;
                var rmse = 0d;
                for (int j = 0; j < EntitiesCount; j++)
                {
                    average += Entities[j].Features[i];
                }
                average /= EntitiesCount;
                for (int j = 0; j < EntitiesCount; j++)
                {
                    rmse += Math.Pow(Entities[j].Features[i] - average, 2);
                }
                rmse = Math.Sqrt(rmse / Entities.Count);
                for (int j = 0; j < Entities.Count; j++)
                {
                    Entities[j].NormalizedFeatures.Add((Entities[j].Features[i] - average) / rmse);
                }
            }
        }

        public void Print()
        {
            foreach (var coefficient in Coefficients)
            {
                Console.WriteLine(coefficient);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var countStrings = Console.ReadLine().Split(' ');
            var entitiesCount = Convert.ToInt32(countStrings[0]);
            var featuresCount = Convert.ToInt32(countStrings[1]);
            var entityStrings = new List<string>();
            for (int i = 0; i < entitiesCount; i++)
            {
                entityStrings.Add(Console.ReadLine());
            }

            var solution = new Solution(entitiesCount, featuresCount, entityStrings);

            solution.Solve(1000, 0.01);

            solution.Print();
        }
    }
}
