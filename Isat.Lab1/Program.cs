using System.Collections.Generic;

namespace Isat.Lab1
{
    class Program
    {
        const string DatasetFileName = "D:\\Development\\Study\\ISaT\\Isat\\Isat.Lab1\\dataset.csv";

        static void Main(string[] args)
        {
            var solution = new Solution(DatasetFileName);

            //distances for Manhattan, Euclidean, Chebyshev
            var distances = new List<double> { 30d, 4d, 3d };

            solution.Solve(distances, 10);
        }
    }
}
