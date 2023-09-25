using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;

namespace lab6
{
    internal class ChartManager
    {
        private readonly SeriesCollection SeriesViews = new();
        public Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>> MeasureResults;
        private readonly HashSet<ISortingMethod> View = new ();
        private Action<string> Log;
        public ChartManager(CartesianChart cartesianChart, Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>>  measureResults) 
        {
            MeasureResults = measureResults;
            cartesianChart.Series = SeriesViews;
            foreach (var item in measureResults.Keys)
            {
                View.Add(item);
            }
        }
        public void SetLog(Action<string> log)
        {
            Log = log;
        }
        public void ShowLine(ISortingMethod sortingMethod)
        {
            View.Add(sortingMethod);
            UpdateChart();
        }
        public void HideLine(ISortingMethod sortingMethod)
        {
            View.Remove(sortingMethod);
            UpdateChart();
        }
        public void UpdateChart()
        {
            SeriesViews.Clear();
            foreach (var sortingMethod in MeasureResults.Keys)
            {
                if (!View.Contains(sortingMethod))
                    continue;

                var ticks = ConvertDictionaryToLong(MeasureResults[sortingMethod]);

                SeriesViews.Add(new LineSeries
                {
                    Title = sortingMethod.Name,
                    Values = new ChartValues<double>(ticks)

                });
            }
            Log("Chart updated");

        }
        public static List<double> ConvertDictionaryToLong(Dictionary<int, TimeSpan?> nullableDictionary)
        {
            List<double> ticksList = new();

            foreach (var kvp in nullableDictionary.Values)
            {
                if (kvp == null)
                {
                    break; // Зупинити обробку, якщо зустріли null
                }

                double ticks = kvp.Value.TotalMilliseconds;
                ticksList.Add(ticks);
            }

            return ticksList;
        }
    }
}
