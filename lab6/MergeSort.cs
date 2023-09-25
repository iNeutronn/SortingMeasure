using System;
using System.Collections.Generic;
using System.Threading;

namespace lab6
{
    class MergeSort : ISortingMethod
    {
        public string Name => "Merge sort";
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        private CancellationTokenSource cancellationToken;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public void Sort<T>(IList<T> list, CancellationTokenSource cts) where T : IComparable<T>
        {
            cancellationToken = cts;
            MergeSortRecursive(list, 0, list.Count - 1);
            
        }

        private void MergeSortRecursive<T>(IList<T> list, int left, int right) where T : IComparable<T>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (left < right)
            {
                int middle = (left + right) / 2;
                MergeSortRecursive(list, left, middle);
                MergeSortRecursive(list, middle + 1, right);
                Merge(list, left, middle, right);
            }
        }

        private void Merge<T>(IList<T> list, int left, int middle, int right) where T : IComparable<T>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            int n1 = middle - left + 1;
            int n2 = right - middle;
            T[] leftArray = new T[n1];
            T[] rightArray = new T[n2];

            for (int i = 0; i < n1; i++)
            {
                leftArray[i] = list[left + i];
            }
            for (int j = 0; j < n2; j++)
            {
                rightArray[j] = list[middle + 1 + j];
            }

            int k = left;
            int iIndex = 0;
            int jIndex = 0;
            while (iIndex < n1 && jIndex < n2)
            {
                if (leftArray[iIndex].CompareTo(rightArray[jIndex]) <= 0)
                {
                    list[k] = leftArray[iIndex];
                    iIndex++;
                }
                else
                {
                    list[k] = rightArray[jIndex];
                    jIndex++;
                }
                k++;
            }

            while (iIndex < n1)
            {
                list[k] = leftArray[iIndex];
                iIndex++;
                k++;
            }

            while (jIndex < n2)
            {
                list[k] = rightArray[jIndex];
                jIndex++;
                k++;
            }
        }
    }
}
