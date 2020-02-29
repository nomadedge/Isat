using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.TaskC
{
    enum DistanceFunctionType
    {
        Manhattan,
        Euclidean,
        Chebyshev
    }

    enum KernelFunctionType
    {
        Uniform,
        Triangular,
        Epanechnikov,
        Quartic,
        Triweight,
        Tricube,
        Gaussian,
        Cosine,
        Logistic,
        Sigmoid
    }

    enum WindowType
    {
        Fixed,
        Variable
    }

    class Entity
    {
        public List<int> Attributes { get; set; }
        public int TargetValue { get; set; }

        public Entity(int attributesCount)
        {
            Attributes = new List<int>(attributesCount);
        }
    }

    class Distance
    {
        public double Value { get; set; }
        public int EntityIndex { get; set; }

        public Distance(double value, int entityIndex)
        {
            Value = value;
            EntityIndex = entityIndex;
        }
    }

    class Solution
    {
        public int EntitiesCount { get; private set; }
        public int AttributesCount { get; private set; }
        public List<Entity> Entities { get; private set; }
        public Entity QueryEntity { get; set; }
        public double QueryEntityTargetValue { get; set; }
        public DistanceFunctionType DistanceFunctionType { get; set; }
        public KernelFunctionType KernelFunctionType { get; set; }
        public WindowType WindowType { get; set; }
        public double WindowWidth { get; set; }
        public int NeighborsCount { get; set; }
        public List<Distance> Distances { get; set; }

        public Solution(int entitiesCount, int attributesCount)
        {
            EntitiesCount = entitiesCount;
            AttributesCount = attributesCount;
            Entities = new List<Entity>(EntitiesCount);
            QueryEntity = new Entity(AttributesCount);
            Distances = new List<Distance>();
        }

        public double CalculateDistance(Entity entity1, Entity entity2)
        {
            var distance = 0d;
            switch (DistanceFunctionType)
            {
                case DistanceFunctionType.Manhattan:
                    for (int i = 0; i < AttributesCount; i++)
                    {
                        distance += Math.Abs(entity1.Attributes[i] - entity2.Attributes[i]);
                    }
                    break;
                case DistanceFunctionType.Euclidean:
                    for (int i = 0; i < AttributesCount; i++)
                    {
                        distance += Math.Pow(entity1.Attributes[i] - entity2.Attributes[i], 2);
                    }
                    distance = Math.Sqrt(distance);
                    break;
                case DistanceFunctionType.Chebyshev:
                    for (int i = 0; i < AttributesCount; i++)
                    {
                        var iDistance = Math.Abs(entity1.Attributes[i] - entity2.Attributes[i]);
                        distance = iDistance > distance ? iDistance : distance;
                    }
                    break;
                default:
                    break;
            }
            return distance;
        }

        public double CalculateKernel(double value)
        {
            var kernel = 0d;
            switch (KernelFunctionType)
            {
                case KernelFunctionType.Uniform:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 0.5;
                    }
                    break;
                case KernelFunctionType.Triangular:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 1d - Math.Abs(value);
                    }
                    break;
                case KernelFunctionType.Epanechnikov:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 0.75 * (1 - Math.Pow(value, 2));
                    }
                    break;
                case KernelFunctionType.Quartic:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 15d / 16d * Math.Pow(1 - Math.Pow(value, 2), 2);
                    }
                    break;
                case KernelFunctionType.Triweight:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 35d / 32d * Math.Pow(1d - Math.Pow(value, 2d), 3d);
                    }
                    break;
                case KernelFunctionType.Tricube:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = 70d / 81d * Math.Pow(1d - Math.Pow(Math.Abs(value), 3d), 3d);
                    }
                    break;
                case KernelFunctionType.Gaussian:
                    kernel = Math.Exp(-Math.Pow(value, 2d) / 2d) / Math.Sqrt(2d * Math.PI);
                    break;
                case KernelFunctionType.Cosine:
                    if (Math.Abs(value) < 1)
                    {
                        kernel = Math.PI * Math.Cos(Math.PI * value / 2d) / 4d;
                    }
                    break;
                case KernelFunctionType.Logistic:
                    kernel = 1d / (Math.Exp(value) + 2d + Math.Exp(-value));
                    break;
                case KernelFunctionType.Sigmoid:
                    kernel = 2d / (Math.PI * (Math.Exp(value) + Math.Exp(-value)));
                    break;
                default:
                    break;
            }
            return kernel;
        }

        public double GetAverage()
        {
            var similarEntities = Entities.Where(e => Enumerable.SequenceEqual(e.Attributes, QueryEntity.Attributes)).ToList();
            if (similarEntities.Any())
            {
                return similarEntities.Average(e => e.TargetValue);
            }
            else
            {
                return Entities.Average(e => e.TargetValue);
            }
        }

        public void Solve()
        {
            for (int i = 0; i < EntitiesCount; i++)
            {
                Distances.Add(new Distance(CalculateDistance(QueryEntity, Entities[i]), i));
            }
            Distances = Distances.OrderBy(d => d.Value).ToList();

            switch (WindowType)
            {
                case WindowType.Fixed:
                    for (int i = 0; i < EntitiesCount; i++)
                    {
                        if (Distances[i].Value >= WindowWidth)
                        {
                            NeighborsCount = i;
                            break;
                        }
                        if (i == EntitiesCount - 1)
                        {
                            NeighborsCount = EntitiesCount;
                            break;
                        }
                    }
                    break;
                case WindowType.Variable:
                    WindowWidth = Distances[NeighborsCount].Value;
                    break;
                default:
                    break;
            }

            if (WindowWidth == 0)
            {
                QueryEntityTargetValue = GetAverage();
                return;
            }

            var numerator = 0d;
            var denominator = 0d;
            for (int i = 0; i < EntitiesCount; i++)
            {
                var kernel = CalculateKernel(Distances[i].Value / WindowWidth);
                var targetValue = Entities[Distances[i].EntityIndex].TargetValue;
                numerator += targetValue * kernel;
                denominator += kernel;
            }

            if (denominator == 0)
            {
                QueryEntityTargetValue = GetAverage();
                return;
            }

            QueryEntityTargetValue = numerator / denominator;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var countsStrings = Console.ReadLine().Split(' ');
            var solution = new Solution(int.Parse(countsStrings[0]), int.Parse(countsStrings[1]));

            for (int i = 0; i < solution.EntitiesCount; i++)
            {
                var entityStrings = Console.ReadLine().Split(' ');
                var entity = new Entity(solution.AttributesCount);
                for (int j = 0; j < solution.AttributesCount; j++)
                {
                    entity.Attributes.Add(int.Parse(entityStrings[j]));
                }
                entity.TargetValue = int.Parse(entityStrings[solution.AttributesCount]);
                solution.Entities.Add(entity);
            }

            var queryEntityAttributesStrings = Console.ReadLine().Split(' ');
            for (int i = 0; i < solution.AttributesCount; i++)
            {
                solution.QueryEntity.Attributes.Add(int.Parse(queryEntityAttributesStrings[i]));
            }

            solution.DistanceFunctionType = (DistanceFunctionType)Enum.Parse(typeof(DistanceFunctionType), Console.ReadLine(), true);
            solution.KernelFunctionType = (KernelFunctionType)Enum.Parse(typeof(KernelFunctionType), Console.ReadLine(), true);
            solution.WindowType = (WindowType)Enum.Parse(typeof(WindowType), Console.ReadLine(), true);
            switch (solution.WindowType)
            {
                case WindowType.Fixed:
                    solution.WindowWidth = double.Parse(Console.ReadLine());
                    break;
                case WindowType.Variable:
                    solution.NeighborsCount = int.Parse(Console.ReadLine());
                    break;
                default:
                    break;
            }

            solution.Solve();

            Console.WriteLine(solution.QueryEntityTargetValue);
        }
    }
}
