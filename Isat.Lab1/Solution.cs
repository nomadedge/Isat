using Isat.Lab1.Enums;
using Isat.Lab1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Isat.Lab1
{
    public class Solution
    {
        public List<Entity> Entities { get; set; }
        public List<string> ClassNames { get; set; }
        public DistanceFunctionType BestDistanceFunctionType { get; set; }
        public KernelFunctionType BestKernelFunctionType { get; set; }
        public WindowType BestWindowType { get; set; }
        public double BestWindowWidth { get; set; }
        public int BestNeighborsCount { get; set; }
        public int ParametersCount => Entities[0].Parameters.Count;

        public Solution(string fileName)
        {
            Entities = new List<Entity>();
            ClassNames = new List<string>();

            ReadFromCsv(fileName);

            PerformOneHotReduction();
            NormalizeParameters();
        }

        private void ReadFromCsv(string fileName)
        {
            string data = File.ReadAllText(fileName);
            var dataRows = data.Split("\n");

            for (int i = 1; i < dataRows.Length; i++)
            {
                var parameters = new List<double>();
                var dataRowStrings = dataRows[i].Split(',');
                for (int j = 1; j < dataRowStrings.Length - 1; j++)
                {
                    parameters.Add(double.Parse(dataRowStrings[j]));
                }
                var className = dataRowStrings.Last();
                Entities.Add(new Entity(parameters, className));
                var classNumber = ClassNames.IndexOf(className);
                if (classNumber == -1)
                {
                    ClassNames.Add(className);
                    Entities.Last().ClassNumber = ClassNames.Count - 1;
                }
                else
                {
                    Entities.Last().ClassNumber = classNumber;
                }
            }
        }

        private void PerformOneHotReduction()
        {
            foreach (var entity in Entities)
            {
                entity.Classes = new List<int>(new int[ClassNames.Count]);
                entity.Classes[entity.ClassNumber] = 1;
            }
        }

        private void NormalizeParameters()
        {
            for (int i = 0; i < ParametersCount; i++)
            {
                var average = 0d;
                var rmse = 0d;
                for (int j = 0; j < Entities.Count; j++)
                {
                    average += Entities[j].Parameters[i];
                }
                average /= Entities.Count;
                for (int j = 0; j < Entities.Count; j++)
                {
                    rmse += Math.Pow(Entities[j].Parameters[i] - average, 2);
                }
                rmse = Math.Sqrt(rmse / Entities.Count);
                for (int j = 0; j < Entities.Count; j++)
                {
                    Entities[j].NormalizedParameters.Add((Entities[j].Parameters[i] - average) / rmse);
                }
            }
        }

        private double CalculateDistance(Entity entity1, Entity entity2, DistanceFunctionType distanceFunctionType)
        {
            var distance = 0d;
            switch (distanceFunctionType)
            {
                case DistanceFunctionType.Manhattan:
                    for (int i = 0; i < ParametersCount; i++)
                    {
                        distance += Math.Abs(entity1.Parameters[i] - entity2.Parameters[i]);
                    }
                    break;
                case DistanceFunctionType.Euclidean:
                    for (int i = 0; i < ParametersCount; i++)
                    {
                        distance += Math.Pow(entity1.Parameters[i] - entity2.Parameters[i], 2);
                    }
                    distance = Math.Sqrt(distance);
                    break;
                case DistanceFunctionType.Chebyshev:
                    for (int i = 0; i < ParametersCount; i++)
                    {
                        var iDistance = Math.Abs(entity1.Parameters[i] - entity2.Parameters[i]);
                        distance = iDistance > distance ? iDistance : distance;
                    }
                    break;
                default:
                    break;
            }
            return distance;
        }

        private double CalculateKernel(double value, KernelFunctionType kernelFunctionType)
        {
            var kernel = 0d;
            switch (kernelFunctionType)
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

        public override string ToString()
        {
            var result = "";
            foreach (var entity in Entities)
            {
                result += $"{entity}\n";
            }
            return result;
        }
    }
}
