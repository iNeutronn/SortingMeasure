using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace lab6
{
    internal class MeasureManager
    {
        public readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);
        public Action? UpdateChart;

        private readonly IEnumerable<int> NumOfEls;
        private readonly Dictionary<ISortingMethod, int> FailedAlgorithms = new Dictionary<ISortingMethod, int>();
        public readonly Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>> MeasureResults = new Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>>();
        

        public MeasureManager(IEnumerable<int> numOfEls,List<ISortingMethod> sortingMethods,Action? updateChart,TimeSpan? maxduration = null)
        {
            this.NumOfEls = numOfEls;
            foreach (var method in sortingMethods)
                MeasureResults.Add(method, new Dictionary<int, TimeSpan?>());
            MaxDuration = maxduration ?? TimeSpan.FromSeconds(10);
            UpdateChart = updateChart;
        }

        private static List<int> GenerateRandomList(int n)
        {
            List<int> list = new List<int>();
            Random random = new();

            for (int i = 0; i < n; i++)
            {
                list.Add(random.Next());
            }

            return list;
        }
        private TimeSpan MeasureSortingTime<T>(List<T> list, ISortingMethod sortingAlgorithm) where T : IComparable<T>
        {
            if (FailedAlgorithms.ContainsKey(sortingAlgorithm) && FailedAlgorithms[sortingAlgorithm]<=list.Count)
            {
                throw new TimeoutException();
            }
            var cancellationTokenSource = new CancellationTokenSource();
            var stopwatch = new Stopwatch();

            
            var sortingTask = Task.Run(() =>
            {
                bool ISorted(IList<T> list)
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
                    //TODO
                    //throw new InvalidOperationException("List is not sorted");
                }
            }, cancellationTokenSource.Token);

            

            if (Task.WaitAny(new[] { sortingTask }, MaxDuration) != -1)
            {
                // Сортування завершилося вчасно
                cancellationTokenSource.Cancel();
                return stopwatch.Elapsed;
            }
            else
            {
                
                cancellationTokenSource.Cancel();
                FailedAlgorithms.Add(sortingAlgorithm,list.Count);
                // Сортування триває довше, ніж максимально допустимо
                throw new TimeoutException();
            }
        }
        public void Measure()
        {
            Task.Run(() => MeasureMethods());
   
        }
        private void MeasureMethods()
        {
            foreach(var method in MeasureResults.Keys)
            {
                MeasureResults[method].Clear();
            }
            foreach (var item in NumOfEls)
            {
                MeasureMethodsFor(item);

                UpdateChart!();
            }
        }

        private void MeasureMethodsFor(int itemCount)
        {
            var list = GenerateRandomList(itemCount);

            Parallel.ForEach(MeasureResults.Keys, (sortMethod) =>
            {
                TimeSpan? performase;
                try
                {
                    performase = MeasureSortingTime(CopyList(list), sortMethod);
                }
                catch (TimeoutException)
                {
                    performase = null;
                }
                MeasureResults[sortMethod].Add(itemCount, performase);
            });
        }
        private List<T> CopyList<T>(List<T> sourse)
        {
            List<T> copy = new List<T>();
            foreach (var item in sourse)
            {
                copy.Add(item);
            }
            return copy;
        }
    }
    
}
