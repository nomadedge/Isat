using System;
using System.Collections.Generic;

namespace Isat.Lab1.Services
{
    public static class FMeasureService
    {
        public static double CalculateFMeasure(List<List<int>> confusionMatrix)
        {
            var classesCount = confusionMatrix.Count;

            var Ts = new List<int>(classesCount);
            var Cs = new List<int>(classesCount);
            var Ps = new List<int>(classesCount);

            var all = 0;
            for (int i = 0; i < classesCount; i++)
            {
                var Ci = 0;
                var Pi = 0;
                for (int j = 0; j < classesCount; j++)
                {
                    all += confusionMatrix[i][j];
                    Ci += confusionMatrix[i][j];
                    Pi += confusionMatrix[j][i];
                }
                Ts.Add(confusionMatrix[i][i]);
                Cs.Add(Ci);
                Ps.Add(Pi);
            }

            var precisionW = 0d;
            var recallW = 0d;
            var microF = 0d;

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

            var fMeasure = (macroF + microF) / 2;
            return fMeasure;
        }

        private static double Precision(int T, int P)
        {
            return Convert.ToDouble(T) / Convert.ToDouble(P);
        }

        private static double Recall(int T, int C)
        {
            return Convert.ToDouble(T) / Convert.ToDouble(C);
        }

        private static double FMeasure(int T, int C, int P)
        {
            if (T == 0)
            {
                return 0;
            }
            var precision = Precision(T, P);
            var recall = Recall(T, C);
            return 2 * precision * recall / (precision + recall);
        }

        static double PrecisionW(int T, int C, int P)
        {
            return Convert.ToDouble(T) * Convert.ToDouble(C) / Convert.ToDouble(P);
        }
    }
}
