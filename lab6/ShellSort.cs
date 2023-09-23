using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    internal class ShellSort : ISortingMethod
    {
        public string Name => "Shell sort";
        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            int n = list.Count;
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                if (cts.IsCancellationRequested)
                {
                    return;
                }
                for (int i = gap; i < n; i++)
                {
                    T temp = list[i];
                    int j;
                    for (j = i; j >= gap && list[j - gap].CompareTo(temp) > 0; j -= gap)
                    {
                        list[j] = list[j - gap];
                    }
                    list[j] = temp;
                }
            }
        }
    }
}
