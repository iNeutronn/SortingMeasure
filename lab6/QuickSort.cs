using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    internal class QuickSort : ISortingMethod
    {
        public string Name => "Quick sort";
        CancellationTokenSource cancellationToken;
        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            cancellationToken = cts;
            QuickSortRecursive(list, 0, list.Count - 1);
        }

        private void QuickSortRecursive<T>(IList<T> list, int left, int right) where T : IComparable<T>
        {
            if (left < right)
            {
                int pivotIndex = Partition(list, left, right);
                QuickSortRecursive(list, left, pivotIndex - 1);
                QuickSortRecursive(list, pivotIndex + 1, right);
            }
        }

        private int Partition<T>(IList<T> list, int left, int right) where T : IComparable<T>
        {
            T pivot = list[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                if (list[j].CompareTo(pivot) <= 0)
                {
                    i++;
                    T temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
            T temp2 = list[i + 1];
            list[i + 1] = list[right];
            list[right] = temp2;
            return i + 1;
        }
    }

}
