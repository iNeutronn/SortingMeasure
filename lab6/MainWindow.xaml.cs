using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] CountOfElementsToMeasure = { 1024, 4096, 16384, 65536, 262144, 1048576, 4194304 };
        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }

        private readonly MeasureManager measureManager; 
        private readonly ChartManager chartManager;
        private List<ISortingMethod> sortingMethods = new()
            {
                new SelectionSort(),
                new ShellSort(),
                new QuickSort(),
                new MergeSort(),
                new CountingSort()
            };

        public MainWindow()
        {
            InitializeComponent();
            LogTextBox.Text = string.Empty;
            YFormatter = value => value.ToString("N");
            XFormatter = value => value.ToString("N");
            
            TimeAxis.LabelFormatter = YFormatter;
            CountOfElsLabel.LabelFormatter = XFormatter;
            CountOfElsLabel.Labels = CountOfElementsToMeasure.OrderBy(x => x).Select(x => x.ToString()).ToArray();

            measureManager = new MeasureManager(CountOfElementsToMeasure, sortingMethods, null);
        
            measureManager.SetLog((text) => Dispatcher.Invoke(() => Log(text)));
            chartManager = new ChartManager(MainChart, measureManager.MeasureResults);
            chartManager.SetLog((text) => Dispatcher.Invoke(() => Log(text)));
            measureManager.UpdateChart = () => Dispatcher.Invoke(()=>chartManager.UpdateChart());
            
        }
        private void Log(string message)
        {
            LogTextBox.Text += message + "\n";
            ScrollViewer.ScrollToVerticalOffset(double.MaxValue);
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
           measureManager.Measure();
        }

        private void SelectionSortCheked(object sender, RoutedEventArgs e)
        {
            chartManager.ShowLine(sortingMethods.Single(x =>x.Name== "Selection sort"));
        }
        private void SelectionSortUnCheked(object sender, RoutedEventArgs e)
        {
            chartManager.HideLine(sortingMethods.Single(x => x.Name == "Selection sort"));
        }
        private void ShellSortCheked(object sender, RoutedEventArgs e)
        {
            chartManager.ShowLine(sortingMethods.Single(x => x.Name == "Shell sort"));
        }
        private void ShellSortUnCheked(object sender, RoutedEventArgs e)
        {
            chartManager.HideLine(sortingMethods.Single(x => x.Name == "Shell sort"));
        }
        private void QuickSortCheked(object sender, RoutedEventArgs e)
        {
            chartManager.ShowLine(sortingMethods.Single(x => x.Name == "Quick sort"));
        }
        private void QuickSortUnCheked(object sender, RoutedEventArgs e)
        {
            chartManager.HideLine(sortingMethods.Single(x => x.Name == "Quick sort"));
        }
        private void MergeSortCheked(object sender, RoutedEventArgs e)
        {
            chartManager.ShowLine(sortingMethods.Single(x => x.Name == "Merge sort"));
        }
        private void MergeSortUnCheked(object sender, RoutedEventArgs e)
        {
            chartManager.HideLine(sortingMethods.Single(x => x.Name == "Merge sort"));
        }
        private void CountingSortCheked(object sender, RoutedEventArgs e)
        {
            //chartManager.ShowLine(sortingMethods.Single(x => x.Name == "Counting sort"));
        }
        private void CountingSortUnCheked(object sender, RoutedEventArgs e)
        {
            //chartManager.HideLine(sortingMethods.Single(x => x.Name == "Counting sort"));
        }
    }
}
