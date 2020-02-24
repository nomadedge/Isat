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
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 0.5;
                    }
                    break;
                case KernelFunctionType.Triangular:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 1 - Math.Abs(value);
                    }
                    break;
                case KernelFunctionType.Epanechnikov:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 0.75 * (1 - Math.Pow(value, 2));
                    }
                    break;
                case KernelFunctionType.Quartic:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 15 / 16 * Math.Pow(1 - Math.Pow(value, 2), 2);
                    }
                    break;
                case KernelFunctionType.Triweight:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 35 / 32 * Math.Pow(1 - Math.Pow(value, 2), 3);
                    }
                    break;
                case KernelFunctionType.Tricube:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = 70 / 81 * Math.Pow(1 - Math.Pow(Math.Abs(value), 3), 3);
                    }
                    break;
                case KernelFunctionType.Gaussian:
                    kernel = Math.Pow(Math.E, -Math.Pow(value, 2) / 2) / Math.Sqrt(2 * Math.PI);
                    break;
                case KernelFunctionType.Cosine:
                    if (Math.Abs(value) <= 1)
                    {
                        kernel = Math.PI * Math.Cos(Math.PI * value / 2) / 4;
                    }
                    break;
                case KernelFunctionType.Logistic:
                    kernel = 1 / Math.Pow(Math.E, value) + 2 + Math.Pow(Math.E, -value);
                    break;
                case KernelFunctionType.Sigmoid:
                    kernel = 2 / Math.PI / Math.Pow(Math.E, value) + Math.Pow(Math.E, -value);
                    break;
                default:
                    break;
            }
            return kernel;
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
                        if (Distances[i].Value > WindowWidth)
                        {
                            NeighborsCount = i;
                            break;
                        }
                    }
                    break;
                case WindowType.Variable:
                    WindowWidth = Distances[NeighborsCount - 1].Value;
                    break;
                default:
                    break;
            }

            var numerator = 0d;
            var denominator = 0d;
            for (int i = 0; i < NeighborsCount; i++)
            {

            }
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
            }

            var queryEntityAttributesStrings = Console.ReadLine().Split(' ');
            for (int i = 0; i < solution.AttributesCount; i++)
            {
                solution.QueryEntity.Attributes.Add(int.Parse(queryEntityAttributesStrings[i]));
            }

            solution.DistanceFunctionType = Enum.Parse<DistanceFunctionType>(Console.ReadLine(), true);
            solution.KernelFunctionType = Enum.Parse<KernelFunctionType>(Console.ReadLine(), true);
            solution.WindowType = Enum.Parse<WindowType>(Console.ReadLine(), true);
            switch (solution.WindowType)
            {
                case WindowType.Fixed:
                    solution.WindowWidth = int.Parse(Console.ReadLine());
                    break;
                case WindowType.Variable:
                    solution.NeighborsCount = int.Parse(Console.ReadLine());
                    break;
                default:
                    break;
            }



            //to do: finish solution
        }
    }
}
