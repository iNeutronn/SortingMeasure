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
        public readonly TimeSpan MaxDuration;
        public Action? UpdateChart;

        private readonly IEnumerable<int> NumOfEls;
        private readonly Dictionary<ISortingMethod, int> FailedAlgorithms = new();
        public readonly Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>> MeasureResults = new();
        private Action<string> Log;

        public MeasureManager(IEnumerable<int> numOfEls,List<ISortingMethod> sortingMethods,Action? updateChart,TimeSpan? maxduration = null)
        {
            this.NumOfEls = numOfEls;
            foreach (var method in sortingMethods)
                MeasureResults.Add(method, new Dictionary<int, TimeSpan?>());
            MaxDuration = maxduration ?? TimeSpan.FromSeconds(10);
            UpdateChart = updateChart;
        }
        public void SetLog(Action<string> log)
        {
            Log = log;
        }
        private List<int> GenerateRandomList(int n)
        {
            Log($"Generating {n} random elements");
            List<int> list = new(n);
            Random random = new();

            for (int i = 0; i < n; i++)
            {
                list.Add(random.Next(0,1000000));
            }

            return list;
        }
        private TimeSpan MeasureSortingTime<T>(List<T> list, ISortingMethod sortingAlgorithm) where T : IComparable<T>
        {

            if (FailedAlgorithms.TryGetValue(sortingAlgorithm,out int FailCountOFElements) && FailCountOFElements <= list.Count)
            {
                throw new TimeoutException();
            }
            var cancellationTokenSource = new CancellationTokenSource();
            var stopwatch = new Stopwatch();

            
            var sortingTask = Task.Run(() =>
            {
                static bool ISorted(IList<T> list)
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
            Task.Run(MeasureMethods);
            Log("Measurment Started");
        }
        private void MeasureMethods()
        {
            foreach(var method in MeasureResults.Keys)
            {
                MeasureResults[method].Clear();
            }
            foreach (var item in NumOfEls)
            {
                Log($"Measuring for {item} elements");
                MeasureMethodsFor(item);

                UpdateChart!();
            }
            Log("Measurment Finished");
            GC.Collect();
        }
        private void MeasureMethodsFor(int itemCount)
        {
            var list = GenerateRandomList(itemCount);

            Parallel.ForEach(MeasureResults.Keys, (sortMethod) =>
            {
                TimeSpan? performase;
                try
                {
                    Log($"Measuring for {sortMethod.Name} for {sortMethod.Name} items");
                    performase = MeasureSortingTime(CopyList(list), sortMethod);
                }
                catch (TimeoutException)
                {
                    Log($"FAIDEL measuring for {sortMethod.Name} for {sortMethod.Name} items");
                    performase = null;
                }
                MeasureResults[sortMethod].Add(itemCount, performase);
            });
        }
        private static List<T> CopyList<T>(List<T> sourse)
        {
            List<T> copy = new();
            foreach (var item in sourse)
            {
                copy.Add(item);
            }
            return copy;
        }
    }
    
}
