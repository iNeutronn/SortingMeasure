using lab6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace lab6
{
    public class CountingSort : ISortingMethod
    {
        public string Name => "Counting Sort";

        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count <= 1)
                return;

            if (list.Any(item => item == null))
                throw new ArgumentException("Counting Sort does not support null elements.");

            if (typeof(T) != typeof(int))
                throw new ArgumentException("Counting Sort only supports sorting integers.");

            // Приведемо елементи до int для сортування
            var intList = list.Cast<int>().ToList();

            int maxValue = intList.Max();
            int minValue = intList.Min();
            int range = maxValue - minValue + 1;

            int[] count = new int[range];
            List<int> sortedList = new List<int>(intList.Count);

            foreach (int num in intList)
            {
                count[num - minValue]++;
            }

            for (int i = 0; i < range; i++)
            {
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                for (int j = 0; j < count[i]; j++)
                {
                    sortedList.Add(i + minValue);
                }
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                if (cts.IsCancellationRequested)
                {
                    throw new OperationCanceledException("Sorting operation was canceled.");
                }

                intList[i] = sortedList[i];
            }

            // Копіюємо відсортований список назад в початковий список
            for (int i = 0; i < intList.Count; i++)
            {
                list[i] = (T)(object)intList[i];
            }
        }
    }
}