﻿namespace Isat.Lab1
{
    class Program
    {
        const string DatasetFileName = "D:\\Development\\Study\\ISaT\\Isat\\Isat.Lab1\\dataset.csv";

        static void Main(string[] args)
        {
            var solution = new Solution(DatasetFileName);

            solution.Solve(5d, 50);
        }
    }
}
