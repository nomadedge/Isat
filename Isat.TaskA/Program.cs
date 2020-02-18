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
            var objects = new List<int>(objectsCount);
            foreach (var number in numbers2)
            {
                objects.Add(int.Parse(number));
            }

            //Sort objects by class
            var objectsByClass = new List<List<int>>(classesCount);
            for (int i = 0; i < classesCount; i++)
            {
                objectsByClass.Add(new List<int>());
            }
            for (int i = 0; i < objectsCount; i++)
            {
                objectsByClass[objects[i] - 1].Add(i);
            }

            //Create 2D list for objects sorted by part
            var initialCapacity = objectsCount / partsCount + 1;
            var objectsByPart = new List<List<int>>(partsCount);
            for (int i = 0; i < partsCount; i++)
            {
                objectsByPart.Add(new List<int>(initialCapacity));
            }

            //Sort objects by part
            int j = -1;
            foreach (var iClass in objectsByClass)
            {
                for (int i = 0; i < iClass.Count; i++)
                {
                    j = (j == partsCount - 1) ? 0 : j + 1;
                    objectsByPart[j].Add(iClass[i]);
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
