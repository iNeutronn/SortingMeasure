using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace lab6
{
    internal static class MeasureManager
    {
        public static readonly TimeSpan maxDuration = TimeSpan.FromSeconds(30);
        public static List<int> GenerateRandomList(int n)
        {
            List<int> list = new List<int>();
            Random random = new();

            for (int i = 0; i < n; i++)
            {
                list.Add(random.Next());
            }

            return list;
        }
        public static TimeSpan MeasureSortingTimeAsync<T>(List<T> list, ISortingMethod sortingAlgorithm) where T : IComparable<T>
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var stopwatch = new Stopwatch();

            var sortingTask = Task.Run(() =>
            {
                bool ISorted<T>(IList<T> list) where T : IComparable<T>
                {
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        if (list[i].CompareTo(list[i + 1]) > 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                stopwatch.Start();
                sortingAlgorithm.Sort<T>(list, cancellationTokenSource);
                stopwatch.Stop();
                if (!ISorted(list))
                {
                    //throw new Exception("List is not sorted");
                }
            }, cancellationTokenSource.Token);

            

            if (Task.WaitAny(new[] { sortingTask }, maxDuration) != -1)
            {
                // Сортування завершилося вчасно
                cancellationTokenSource.Cancel();
                return stopwatch.Elapsed;
            }
            else
            {
                
                cancellationTokenSource.Cancel();
                // Сортування триває довше, ніж максимально допустимо
                throw new TimeoutException();
            }
        }
    }
    
}
