using Isat.Lab1.Enums;
using Isat.Lab1.Models;
using Isat.Lab1.Services;
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
        public int ParametersCount => Entities[0].Parameters.Count;
        public List<FMeasureFromEnums> NaiveFMeasures { get; set; }
        public List<FMeasureFromEnums> OneHotFMeasures { get; set; }
        public FMeasureFromEnums NaiveBestFMeasure { get; set; }
        public FMeasureFromEnums OneHotBestFMeasure { get; set; }
        public List<FMeasureFromWidthOrCount> NaiveFMeasureDependency { get; set; }
        public List<FMeasureFromWidthOrCount> OneHotFMeasureDependency { get; set; }
        public List<List<List<Distance>>> DistancesForEachType { get; set; }

        public Solution(string fileName)
        {
            Entities = new List<Entity>();
            ClassNames = new List<string>();
            NaiveFMeasures = new List<FMeasureFromEnums>();
            OneHotFMeasures = new List<FMeasureFromEnums>();
            DistancesForEachType = new List<List<List<Distance>>>();

            ReadFromCsv(fileName);

            PerformOneHotReduction();
            NormalizeParameters();
            CalculateDistances();
            SortDistances();
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

        private void CalculateDistances()
        {
            foreach (DistanceFunctionType distanceFunctionType in Enum.GetValues(typeof(DistanceFunctionType)))
            {
                var distancesForEachElement = new List<List<Distance>>();
                for (int i = 0; i < Entities.Count; i++)
                {
                    var distancesForElement = new List<Distance>();
                    for (int j = 0; j < Entities.Count; j++)
                    {
                        if (j < i)
                        {
                            distancesForElement.Add(new Distance(distancesForEachElement[j][i].Value, j));
                        }
                        //else if (j == i)
                        //{
                        //    distancesRow.Add(new Distance(0, i, j));
                        //}
                        else
                        {
                            distancesForElement.Add(new Distance(CalculateDistance(Entities[i], Entities[j], distanceFunctionType), j));
                        }
                    }
                    distancesForEachElement.Add(distancesForElement);
                }
                DistancesForEachType.Add(distancesForEachElement);
            }
        }

        private void SortDistances()
        {
            for (int i = 0; i < DistancesForEachType.Count; i++)
            {
                for (int j = 0; j < DistancesForEachType[i].Count; j++)
                {
                    DistancesForEachType[i][j] = DistancesForEachType[i][j].OrderBy(d => d.Value).ToList();
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
                        distance += Math.Abs(entity1.NormalizedParameters[i] - entity2.NormalizedParameters[i]);
                    }
                    break;
                case DistanceFunctionType.Euclidean:
                    for (int i = 0; i < ParametersCount; i++)
                    {
                        distance += Math.Pow(entity1.NormalizedParameters[i] - entity2.NormalizedParameters[i], 2);
                    }
                    distance = Math.Sqrt(distance);
                    break;
                case DistanceFunctionType.Chebyshev:
                    for (int i = 0; i < ParametersCount; i++)
                    {
                        var iDistance = Math.Abs(entity1.NormalizedParameters[i] - entity2.NormalizedParameters[i]);
                        distance = iDistance > distance ? iDistance : distance;
                    }
                    break;
                default:
                    break;
            }
            return distance;
        }

        private void IterateThroughTypes(double windowWidth, int neighborsCount)
        {
            foreach (WindowType windowType in Enum.GetValues(typeof(WindowType)))
            {
                foreach (DistanceFunctionType distanceFunctionType in Enum.GetValues(typeof(DistanceFunctionType)))
                {
                    foreach (KernelFunctionType kernelFunctionType in Enum.GetValues(typeof(KernelFunctionType)))
                    {
                        var parameters = new Parameters(
                            Entities,
                            DistancesForEachType[Convert.ToInt32(distanceFunctionType)],
                            distanceFunctionType, windowType, kernelFunctionType, windowWidth, neighborsCount);

                        var naiveFMeasure = LeaveOneOutService.CalculateFMeasureNaive(parameters);
                        NaiveFMeasures.Add(new FMeasureFromEnums(parameters, naiveFMeasure));

                        var oneHotFMeasure = LeaveOneOutService.CalculateFMeasureOneHot(parameters);
                        OneHotFMeasures.Add(new FMeasureFromEnums(parameters, oneHotFMeasure));
                    }
                }
            }
        }

        private void FindDependencies()
        {
            switch (NaiveBestFMeasure.Parameters.WindowType)
            {
                case WindowType.Fixed:
                    NaiveFMeasureDependency = LeaveOneOutService.FindFMeasureFromWindowWidth(
                        NaiveBestFMeasure.Parameters,
                        0,
                        40,
                        0.5,
                        ReductionType.Naive);
                    break;
                case WindowType.Variable:
                    NaiveFMeasureDependency = LeaveOneOutService.FindFMeasureFromNeighborsCount(
                        NaiveBestFMeasure.Parameters,
                        1,
                        Entities.Count,
                        1,
                        ReductionType.Naive);
                    break;
                default:
                    break;
            }

            switch (OneHotBestFMeasure.Parameters.WindowType)
            {
                case WindowType.Fixed:
                    OneHotFMeasureDependency = LeaveOneOutService.FindFMeasureFromWindowWidth(
                        NaiveBestFMeasure.Parameters,
                        0,
                        40,
                        0.5,
                        ReductionType.OneHot);
                    break;
                case WindowType.Variable:
                    OneHotFMeasureDependency = LeaveOneOutService.FindFMeasureFromNeighborsCount(
                        NaiveBestFMeasure.Parameters,
                        1,
                        Entities.Count,
                        1,
                        ReductionType.OneHot);
                    break;
                default:
                    break;
            }
        }

        public void Solve(
            double windowWidth,
            int neighborsCount)
        {
            IterateThroughTypes(windowWidth, neighborsCount);

            NaiveBestFMeasure = NaiveFMeasures.Aggregate((max, next) => max.Value > next.Value ? max : next);
            OneHotBestFMeasure = OneHotFMeasures.Aggregate((max, next) => max.Value > next.Value ? max : next);

            //FindDependencies();

            foreach (var fMeasure in NaiveFMeasures)
            {
                Console.WriteLine($"{fMeasure.Parameters.WindowType}; {fMeasure.Parameters.DistanceFunctionType}; {fMeasure.Parameters.KernelFunctionType}; {fMeasure.Value}");
            }
            Console.WriteLine($">>>>>NAIVE BEST: {NaiveBestFMeasure.Parameters.WindowType}; {NaiveBestFMeasure.Parameters.DistanceFunctionType}; {NaiveBestFMeasure.Parameters.KernelFunctionType}; {NaiveBestFMeasure.Value}");
            foreach (var fMeasure in OneHotFMeasures)
            {
                Console.WriteLine($"{fMeasure.Parameters.WindowType}; {fMeasure.Parameters.DistanceFunctionType}; {fMeasure.Parameters.KernelFunctionType}; {fMeasure.Value}");
            }
            Console.WriteLine($">>>>>ONEHOT BEST: {OneHotBestFMeasure.Parameters.WindowType}; {OneHotBestFMeasure.Parameters.DistanceFunctionType}; {OneHotBestFMeasure.Parameters.KernelFunctionType}; {OneHotBestFMeasure.Value}");
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
