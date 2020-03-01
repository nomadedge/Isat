using Isat.Lab1.Enums;
using System.Collections.Generic;

namespace Isat.Lab1.Models
{
    public class Parameters
    {
        public List<List<Distance>> DistancesForEachElement { get; set; }
        public WindowType WindowType { get; set; }
        public KernelFunctionType KernelFunctionType { get; set; }
        public double WindowWidth { get; set; }
        public int NeighborsCount { get; set; }

        public Parameters(
            List<List<Distance>> distancesForEachElement,
            WindowType windowType,
            KernelFunctionType kernelFunctionType,
            double windowWidth,
            int neighborsCount)
        {
            DistancesForEachElement = distancesForEachElement;
            WindowType = windowType;
            KernelFunctionType = kernelFunctionType;
            WindowWidth = windowWidth;
            NeighborsCount = neighborsCount;
        }

        public Parameters() { }
    }
}
