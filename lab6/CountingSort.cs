using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    internal class CountingSort : ISortingMethod
    {
        public string Name => "Counting sort";

        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            if (list.Count == 0)
                return;

            T min = list[0];
            T max = list[0];

            foreach (T item in list)
            {
                if (item.CompareTo(min) < 0)
                    min = item;
                if (item.CompareTo(max) > 0)
                    max = item;
            }

            Dictionary<T, int> countDictionary = new();

            foreach (var item in list)
            {
                countDictionary.TryGetValue(item, out var count);
                countDictionary[item] = count + 1;
            }

            int j = 0;
            for (T key = min; key.CompareTo(max) <= 0; key = Increment(key))
            {
                if (cts.IsCancellationRequested)
                {
                    return;
                }
                if (countDictionary.TryGetValue(key, out int count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        list[j] = key;
                        j++;
                    }
                }
            }
        }

        private static T Increment<T>(T value)
        {
            dynamic incremented = value;
            incremented++;
            return incremented;
        }
    }

}
