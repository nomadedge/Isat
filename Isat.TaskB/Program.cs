using System;
using System.Collections.Generic;

namespace Isat.TaskB
{
    class Program
    {
        static decimal Precision(int T, int P)
        {
            return Convert.ToDecimal(T) / Convert.ToDecimal(P);
        }

        static decimal Recall(int T, int C)
        {
            return Convert.ToDecimal(T) / Convert.ToDecimal(C);
        }

        static decimal FMeasure(int T, int C, int P)
        {
            if (T == 0)
            {
                return 0;
            }
            var precision = Precision(T, P);
            var recall = Recall(T, C);
            return 2 * precision * recall / (precision + recall);
        }

        static decimal PrecisionW(int T, int C, int P)
        {
            return Convert.ToDecimal(T) * Convert.ToDecimal(C) / Convert.ToDecimal(P);
        }

        static void Main(string[] args)
        {
            //Initialize everything from input and calculate all
            var classesCount = int.Parse(Console.ReadLine());

            var confusionMatrix = new List<List<int>>(classesCount);
            int all = 0;

            for (int i = 0; i < classesCount; i++)
            {
                var rawStrings = Console.ReadLine().Split(' ');
                var rawInts = new List<int>(classesCount);
                for (int j = 0; j < classesCount; j++)
                {
                    var number = int.Parse(rawStrings[j]);
                    rawInts.Add(number);
                    all += number;
                }
                confusionMatrix.Add(rawInts);
            }

            //Calculate T = TruePositive; Class = TruePositive + FalseNegative; Positive = TruePositive + FalsePositive
            var Ts = new List<int>(classesCount);
            var Cs = new List<int>(classesCount);
            var Ps = new List<int>(classesCount);

            for (int i = 0; i < classesCount; i++)
            {
                var Ci = 0;
                var Pi = 0;
                for (int j = 0; j < classesCount; j++)
                {
                    Ci += confusionMatrix[i][j];
                    Pi += confusionMatrix[j][i];
                }
                Ts.Add(confusionMatrix[i][i]);
                Cs.Add(Ci);
                Ps.Add(Pi);
            }

            //Calculate micro and macro F-measure
            var precisionW = 0m;
            var recallW = 0m;
            var microF = 0m;

            for (int i = 0; i < classesCount; i++)
            {
                if (Cs[i] != 0 && Ps[i] != 0)
                {
                    precisionW += PrecisionW(Ts[i], Cs[i], Ps[i]);
                    recallW += Ts[i];
                    microF += Cs[i] * FMeasure(Ts[i], Cs[i], Ps[i]);
                }
            }
            precisionW /= all;
            recallW /= all;
            microF /= all;

            var macroF = 2 * precisionW * recallW / (precisionW + recallW);

            Console.WriteLine(macroF);
            Console.WriteLine(microF);
        }
    }
}
