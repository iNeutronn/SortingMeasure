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
        public ChartManager(CartesianChart cartesianChart, Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>>  measureResults) 
        {
            MeasureResults = measureResults;
            cartesianChart.Series = SeriesViews;
        }
        public void UpdateChart()
        {
            SeriesViews.Clear();
            foreach (var sortingMethod in MeasureResults.Keys)
            {
                var ticks = ConvertDictionaryToLong(MeasureResults[sortingMethod]);

                SeriesViews.Add(new LineSeries
                {
                    Title = sortingMethod.Name,
                    Values = new ChartValues<double>(ticks)

                });
            }

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
