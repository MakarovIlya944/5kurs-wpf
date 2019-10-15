using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Data> Points;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Points = new ObservableCollection<Data>();
            Points.Add(new Data { X = 0, Y = 0 });
            Points.Add(new Data { X = 1, Y = 1 });
            Points.Add(new Data { X = 2, Y = 4 });
            DataGrid1.ItemsSource = Points;
        }

        public void Paint() {

        }
    }

    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString() { return $"{X} {Y}"; }
    }
}
