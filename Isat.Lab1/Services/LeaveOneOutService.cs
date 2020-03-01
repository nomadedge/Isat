using Isat.Lab1.Enums;
using Isat.Lab1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Isat.Lab1.Services
{
    public static class LeaveOneOutService
    {
        public static double CalculateFMeasureNaive(Parameters parameters)
        {
            //Fill confusion matrix with zero values
            var classesCount = parameters.Entities[0].Classes.Count;
            var confusionMatrix = new List<List<int>>(classesCount);
            for (int i = 0; i < classesCount; i++)
            {
                var confusionMatrixRow = new List<int>(classesCount);
                for (int j = 0; j < classesCount; j++)
                {
                    confusionMatrixRow.Add(0);
                }
                confusionMatrix.Add(confusionMatrixRow);
            }

            //Iterate over leave-one-out
            for (int i = 0; i < parameters.Entities.Count; i++)
            {
                var queryEntityClassNumber = 0d;
                switch (parameters.WindowType)
                {
                    case WindowType.Fixed:
                        for (int j = 0; j < parameters.Entities.Count - 1; j++)
                        {
                            if (parameters.DistancesForEachElement[i][j].Value >= parameters.WindowWidth)
                            {
                                parameters.NeighborsCount = j;
                                break;
                            }
                            if (j == parameters.Entities.Count - 2)
                            {
                                parameters.NeighborsCount = j + 1;
                                break;
                            }
                        }
                        break;
                    case WindowType.Variable:
                        parameters.WindowWidth = parameters.DistancesForEachElement[i][parameters.NeighborsCount].Value;
                        break;
                    default:
                        break;
                }

                if (parameters.WindowWidth == 0)
                {
                    queryEntityClassNumber = GetAverageNaive(parameters, i);
                }
                else
                {
                    var numerator = 0d;
                    var denominator = 0d;
                    for (int j = 0; j < parameters.Entities.Count; j++)
                    {
                        var kernel = CalculateKernel(
                            parameters.DistancesForEachElement[i][j].Value / parameters.WindowWidth, parameters.KernelFunctionType);
                        var classNumber = parameters.Entities[parameters.DistancesForEachElement[i][j].EntityIndex].ClassNumber;
                        numerator += classNumber * kernel;
                        denominator += kernel;
                    }

                    if (denominator == 0)
                    {
                        queryEntityClassNumber = GetAverageNaive(parameters, i);
                    }
                    else
                    {
                        queryEntityClassNumber = numerator / denominator;
                    }
                }

                //Add prediction to confusion matrix
                confusionMatrix[parameters.Entities[i].ClassNumber][Convert.ToInt32(Math.Round(queryEntityClassNumber))] += 1;
            }

            var fMeasure = FMeasureService.CalculateFMeasure(confusionMatrix);

            return fMeasure;
        }

        public static double CalculateFMeasureOneHot(Parameters parameters)
        {
            //Fill confusion matrix with zero values
            var classesCount = parameters.Entities[0].Classes.Count;
            var confusionMatrix = new List<List<int>>(classesCount);
            for (int i = 0; i < classesCount; i++)
            {
                var confusionMatrixRow = new List<int>(classesCount);
                for (int j = 0; j < classesCount; j++)
                {
                    confusionMatrixRow.Add(0);
                }
                confusionMatrix.Add(confusionMatrixRow);
            }

            //Iterate over leave-one-out
            for (int i = 0; i < parameters.Entities.Count; i++)
            {
                var queryEntityClasses = new List<double>();
                switch (parameters.WindowType)
                {
                    case WindowType.Fixed:
                        for (int j = 0; j < parameters.Entities.Count - 1; j++)
                        {
                            if (parameters.DistancesForEachElement[i][j].Value >= parameters.WindowWidth)
                            {
                                parameters.NeighborsCount = j;
                                break;
                            }
                            if (j == parameters.Entities.Count - 2)
                            {
                                parameters.NeighborsCount = j + 1;
                                break;
                            }
                        }
                        break;
                    case WindowType.Variable:
                        parameters.WindowWidth = parameters.DistancesForEachElement[i][parameters.NeighborsCount].Value;
                        break;
                    default:
                        break;
                }

                if (parameters.WindowWidth == 0)
                {
                    for (int k = 0; k < classesCount; k++)
                    {
                        queryEntityClasses.Add(GetAverageOneHot(parameters, i, k));
                    }
                }
                else
                {
                    var numerator = 0d;
                    var denominator = 0d;
                    for (int j = 0; j < classesCount; j++)
                    {
                        for (int k = 0; k < parameters.Entities.Count; k++)
                        {
                            var kernel = CalculateKernel(
                            parameters.DistancesForEachElement[i][k].Value / parameters.WindowWidth, parameters.KernelFunctionType);
                            var classColumn = parameters.Entities[parameters.DistancesForEachElement[i][k].EntityIndex].Classes[j];
                            numerator += classColumn * kernel;
                            denominator += kernel;
                        }
                        if (denominator == 0)
                        {
                            queryEntityClasses.Add(GetAverageOneHot(parameters, i, j));
                        }
                        else
                        {
                            queryEntityClasses.Add(numerator / denominator);
                        }
                    }
                }

                //Add prediction to confusion matrix. If two columns have similar values choose first occurence
                var predictedClass = queryEntityClasses.IndexOf(queryEntityClasses.Max());
                confusionMatrix[parameters.Entities[i].ClassNumber][predictedClass] += 1;
            }

            var fMeasure = FMeasureService.CalculateFMeasure(confusionMatrix);

            return fMeasure;
        }

        public static List<FMeasureFromWidthOrCount> FindFMeasureFromWindowWidth(
            Parameters parameters,
            double minWindowWidth,
            double maxWindowWidth,
            double step,
            ReductionType reductionType)
        {
            var dependency = new List<FMeasureFromWidthOrCount>();
            var currentWindowWidth = minWindowWidth;
            while (currentWindowWidth < maxWindowWidth)
            {
                parameters.WindowWidth = currentWindowWidth;
                switch (reductionType)
                {
                    case ReductionType.Naive:
                        dependency.Add(new FMeasureFromWidthOrCount
                        {
                            Value = CalculateFMeasureNaive(parameters),
                            WindowWidth = currentWindowWidth
                        });
                        break;
                    case ReductionType.OneHot:
                        dependency.Add(new FMeasureFromWidthOrCount
                        {
                            Value = CalculateFMeasureOneHot(parameters),
                            WindowWidth = currentWindowWidth
                        });
                        break;
                    default:
                        break;
                }
                currentWindowWidth += step;
            }
            return dependency;
        }

        public static List<FMeasureFromWidthOrCount> FindFMeasureFromNeighborsCount(
            Parameters parameters,
            int minNeiborsCount,
            int maxNeighborsCount,
            int step,
            ReductionType reductionType)
        {
            var dependency = new List<FMeasureFromWidthOrCount>();
            var currentNeighborsCount = minNeiborsCount;
            while (currentNeighborsCount < maxNeighborsCount)
            {
                parameters.NeighborsCount = currentNeighborsCount;
                switch (reductionType)
                {
                    case ReductionType.Naive:
                        dependency.Add(new FMeasureFromWidthOrCount
                        {
                            Value = CalculateFMeasureNaive(parameters),
                            NeighborsCount = currentNeighborsCount
                        });
                        break;
                    case ReductionType.OneHot:
                        dependency.Add(new FMeasureFromWidthOrCount
                        {
                            Value = CalculateFMeasureOneHot(parameters),
                            NeighborsCount = currentNeighborsCount
                        });
                        break;
                    default:
                        break;
                }
                currentNeighborsCount += step;
            }
            return dependency;
        }

        private static double CalculateKernel(double value, KernelFunctionType kernelFunctionType)
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

        private static double GetAverageNaive(Parameters parameters, int queryEntityIndex)
        {
            var similarEntities = parameters.Entities.Where(e =>
                Enumerable.SequenceEqual(e.NormalizedParameters, parameters.Entities[queryEntityIndex].NormalizedParameters)).ToList();
            if (similarEntities.Any())
            {
                return similarEntities.Average(e => e.ClassNumber);
            }
            else
            {
                return parameters.Entities.Average(e => e.ClassNumber);
            }
        }

        private static double GetAverageOneHot(Parameters parameters, int queryEntityIndex, int classIndex)
        {
            var similarEntities = parameters.Entities.Where(e =>
                Enumerable.SequenceEqual(e.NormalizedParameters, parameters.Entities[queryEntityIndex].NormalizedParameters)).ToList();
            if (similarEntities.Any())
            {
                return similarEntities.Average(e => e.Classes[classIndex]);
            }
            else
            {
                return parameters.Entities.Average(e => e.Classes[classIndex]);
            }
        }
    }
}
