using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    internal class SelectionSort : ISortingMethod
    {
        public string Name => "Selection sort";
        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if(cts.IsCancellationRequested)
                {
                    return;
                }
                int minIndex = i;
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[j].CompareTo(list[minIndex]) < 0)
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    (list[minIndex], list[i]) = (list[i], list[minIndex]);
                }
            }
        }
    }
}
