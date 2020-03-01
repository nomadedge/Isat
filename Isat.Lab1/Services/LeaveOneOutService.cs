using Isat.Lab1.Enums;
using Isat.Lab1.Models;
using System;
using System.Collections.Generic;

namespace Isat.Lab1.Services
{
    public static class LeaveOneOutService
    {
        public static double CalculateFMeasureNaive(Parameters parameters)
        {

        }

        public static double CalculateFMeasureOneHot(Parameters parameters)
        {

        }

        public static List<FMeasureFromWidthOrCount> FindFMeasureFromWindowWidth(
            Parameters parameters,
            double minWindowWidth,
            double maxWindowWidth,
            double step,
            ReductionType reductionType)
        {

        }

        public static List<FMeasureFromWidthOrCount> FindFMeasureFromNeighborsCount(
            Parameters parameters,
            int minNeiborsCount,
            int maxNeighborsCount,
            int step,
            ReductionType reductionType)
        {

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
    }
}
