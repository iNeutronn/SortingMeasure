using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] NumOfEls = { 1024, 4096, 16384, 65536, 262144, 1048576, 4194304 };
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }

        private Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>> MeasureResults = new Dictionary<ISortingMethod, Dictionary<int, TimeSpan?>>();

        private SeriesCollection seriesViews = new ();
        private MeasureManager measureManager = new();

        public MainWindow()
        {
            InitializeComponent();
            LogTextBox.Text = string.Empty;
            YFormatter = value => value.ToString("N");
            XFormatter = value => value.ToString("N");
            MainChart.Series = seriesViews;
            TimeAxis.LabelFormatter = YFormatter;
            CountOfElsLabel.LabelFormatter = XFormatter;
            CountOfElsLabel.Labels = NumOfEls.OrderBy(x => x).Select(x => x.ToString()).ToArray();
            MeasureResults.Add(new SelectionSort(), new Dictionary<int, TimeSpan?>());
            MeasureResults.Add(new ShellSort(), new Dictionary<int, TimeSpan?>());
            MeasureResults.Add(new QuickSort(), new Dictionary<int, TimeSpan?>());
            MeasureResults.Add(new MergeSort(), new Dictionary<int, TimeSpan?>());
            //MeasureResults.Add(new CountingSort(), new Dictionary<int, TimeSpan?>());
        }

        private void AllMethodsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SidePanelGrid.Children)
            {
                if (item is CheckBox checkBox)
                {
                    checkBox.IsChecked = true;
                }
            }
            LogTextBox.Text += "All methods selected\n";
        }

        private void NoOneBuuton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SidePanelGrid.Children)
            {
                if (item is CheckBox checkBox)
                {
                    checkBox.IsChecked = false;
                }
            }
            LogTextBox.Text += "No one method selected\n";
        }

        private void MeasugeButton_Click(object sender, RoutedEventArgs e)
        {
            Task sorttask = new Task(MeasureMethods);
            sorttask.Start();
        }

        private void MeasureMethods()
        {
            foreach (var item in NumOfEls)
            {
                MeasureMethodsFor(item);
               
                Dispatcher.Invoke(()=> UpdateChart());
            }
        }

        private void MeasureMethodsFor(int itemCount)
        {
            var list = MeasureManager.GenerateRandomList(itemCount);
            
           Parallel.ForEach(MeasureResults.Keys, (sortMethod) =>
            {
                TimeSpan? performase;
                try
                {
                    performase = measureManager.MeasureSortingTime(CopyList(list), sortMethod);
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
        private void UpdateChart()
        {
            seriesViews.Clear();
            foreach (var sortingMethod in MeasureResults.Keys)
            {
                var ticks = ConvertDictionaryToLong(MeasureResults[sortingMethod]);

                seriesViews.Add(new LineSeries
                {
                    Title = sortingMethod.Name,
                    Values = new ChartValues<double>(ticks)

                });
            }

        }
        public static List<double> ConvertDictionaryToLong(Dictionary<int, TimeSpan?> nullableDictionary)
        {
            List<double> ticksList = new List<double>();

            foreach (var kvp in nullableDictionary)
            {
                if (kvp.Value == null)
                {
                    break; // Зупинити обробку, якщо зустріли null
                }

                double ticks = kvp.Value.Value.TotalMilliseconds;
                ticksList.Add(ticks);
            }

            return ticksList;
        }

    }
}
