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

        public MainWindow()
        {
            InitializeComponent();
            LogTextBox.Text = string.Empty;
            YFormatter = value => value.ToString("N");
            XFormatter = value => value.ToString("N");
            
            TimeAxis.LabelFormatter = YFormatter;
            CountOfElsLabel.LabelFormatter = XFormatter;
            CountOfElsLabel.Labels = CountOfElementsToMeasure.OrderBy(x => x).Select(x => x.ToString()).ToArray();

            

            List<ISortingMethod> sortingMethods = new()
            {
                new SelectionSort(),
                new ShellSort(),
                new QuickSort(),
                new MergeSort(),
                //new CountingSort()
            };

            measureManager = new MeasureManager(CountOfElementsToMeasure, sortingMethods, null);

            chartManager = new ChartManager(MainChart, measureManager.MeasureResults);
            
            measureManager.UpdateChart = () => Dispatcher.Invoke(()=>chartManager.UpdateChart());
            
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

    }
}
