using System;
using System.Collections.Generic;

namespace Isat.TaskA
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize everything from input
            var string1 = Console.ReadLine();
            var string2 = Console.ReadLine();

            var numbers1 = string1.Split(' ');
            var objectsCount = int.Parse(numbers1[0]);
            var classesCount = int.Parse(numbers1[1]);
            var partsCount = int.Parse(numbers1[2]);

            var numbers2 = string2.Split(' ');
            var Cs = new List<int>();
            foreach (var number in numbers2)
            {
                Cs.Add(int.Parse(number));
            }

            //Sort objects by class
            var objectsByClass = new List<List<int>>();
            for (int i = 0; i < classesCount; i++)
            {
                var iClass = new List<int>();
                for (int j = 0; j < objectsCount; j++)
                {
                    if (Cs[j] == i + 1)
                    {
                        iClass.Add(j);
                    }
                }
                objectsByClass.Add(iClass);
            }

            //Create two-dimensions list for objects sorted by part
            var objectsByPart = new List<List<int>>();
            for (int i = 0; i < partsCount; i++)
            {
                objectsByPart.Add(new List<int>());
            }

            //Sort objects by part
            var lesserParts = new List<int>();
            foreach (var iClass in objectsByClass)
            {
                if (lesserParts.Count < iClass.Count)
                {
                    for (int i = 0; i < lesserParts.Count; i++)
                    {
                        objectsByPart[lesserParts[i]].Add(iClass[i]);
                    }
                    int j = -1;
                    for (int i = lesserParts.Count; i < iClass.Count; i++)
                    {
                        j = (j == partsCount - 1) ? 0 : j + 1;
                        objectsByPart[j].Add(iClass[i]);
                    }

                    lesserParts.Clear();
                    for (int i = j + 1; i < partsCount; i++)
                    {
                        lesserParts.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < iClass.Count; i++)
                    {
                        objectsByPart[lesserParts[i]].Add(iClass[i]);
                    }
                    lesserParts.RemoveRange(0, iClass.Count);
                }
            }

            //Print parts
            foreach (var part in objectsByPart)
            {
                Console.Write(part.Count);
                foreach (var number in part)
                {
                    Console.Write($" {number + 1}");
                }
                Console.WriteLine();
            }

            return;
        }
    }
}
