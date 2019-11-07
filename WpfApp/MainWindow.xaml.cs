using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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
using Microsoft.Win32;

namespace WpfApp
{
    public interface IDialogService
    {
        void ShowMessage(string message);   // показ сообщения
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Data> Points;

        private Dictionary<int, List<Data>> bindings;
        private int maxBind = 1;
        
        //scale
        private double k = 20;

        //polyline id
        int id = 0;

        public string FilePath { get; set; }

        public void ShowMessage(string message) {
            MessageBox.Show(message);
        }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Points = new ObservableCollection<Data>();
            Points.Add(new Data { X = 0, Y = 0 });
            Points.Add(new Data { X = 1, Y = 1 });
            Points.Add(new Data { X = 2, Y = 4 });
            bindings = new Dictionary<int, List<Data>>();
            bindings[maxBind] = Points.ToList();
            label1.Content = "Table " + maxBind.ToString();
            comboBoxTable.Items.Add(maxBind);
            DataGrid1.ItemsSource = Points;
            PaintNet();
            Paint();
        }

        private void PaintNet() {
            double w = ArtGrid.Width;
            double h = ArtGrid.Height;
            Line a = new Line();
            double t = k;
            int i = 1;

            a.Y1 = 0;
            a.Y2 = h;
            a.X1 = w / 2;
            a.X2 = w / 2;
            i++; a.Stroke = Brushes.Red;
            ArtGrid.Children.Add(a);
            a = new Line();
            a.X1 = 0;
            a.X2 = w;
            a.Y1 = h / 2;
            a.Y2 = h / 2;
            a.Stroke = Brushes.Red;
            ArtGrid.Children.Add(a);

            i = 1;
            while(t < h/2) {

                a = new Line();
                a.X1 = 0;
                a.X2 = w;
                a.Y1 = h/2 + t;
                a.Y2 = h/2 + t;
                a.Stroke = Brushes.Black;
                ArtGrid.Children.Add(a);

                a = new Line();
                a.X1 = 0;
                a.X2 = w;
                a.Y1 = h/2 - t;
                a.Y2 = h/2 - t;
                a.Stroke = Brushes.Black;
                ArtGrid.Children.Add(a);
                i++;
                t = i * k;
            }

            i = 1;
            t = k;
            while(t < w / 2) {

                a = new Line();
                a.Y1 = 0;
                a.Y2 = h;
                a.X1 = w / 2 + t;
                a.X2 = w / 2 + t;
                a.Stroke = Brushes.Black;
                ArtGrid.Children.Add(a);

                a = new Line();
                a.Y1 = 0;
                a.Y2 = h;
                a.X1 = w / 2 - t;
                a.X2 = w / 2 - t;
                a.Stroke = Brushes.Black;
                ArtGrid.Children.Add(a);
                i++;
                t = i * k;
            }

        }

        private void Paint() {
            Polyline a = new Polyline();
            double w = ArtGrid.Width;
            double h = ArtGrid.Height;
            a.Stroke = Brushes.SlateGray;
            a.StrokeThickness = 2;
            a.Points = new PointCollection(Points.Select(x => new Point { X = x.X * k + w/2, Y = h/2 - x.Y * k}));
            
            id = ArtGrid.Children.Add(a);
        }

        private void UpdateGraphic() {
            if(Points.Count < 2)
                return;
            ArtGrid.Children.RemoveAt(id);
            Paint();

            //PathFigure b = new PathFigure();
            //b.StartPoint = new Point(Points[0].X, Points[0].Y);
            //b.Segments.Add(new BezierSegment(Points.Select((x,i) => { if(i != 0) return new Point { X = x.X * k + w / 2, Y = h / 2 - x.Y * k }; })));

            //PathGeometry myPathGeometry = new PathGeometry();
            //myPathGeometry.Figures.Add(b);

            //// Display the PathGeometry. 
            //Path myPath = new Path();
            //myPath.Stroke = Brushes.Black;
            //myPath.StrokeThickness = 1;
            //myPath.Data = myPathGeometry;
        }

        private void Points_CollectionChanged(object sender, DataGridCellEditEndingEventArgs e) {
            UpdateGraphic();
        }

        private void Points_CollectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateGraphic();
        }

        private void Points_CollectionChanged(object sender, SelectedCellsChangedEventArgs e) {
            UpdateGraphic();
        }

        private void OpenFile(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
                FilePath = openFileDialog.FileName;
            else
                return;

            string fileText = File.ReadAllText(FilePath);
            List<string> tmp = fileText.Split('\n').ToList(); tmp.RemoveAt(tmp.Count - 1);
            Points = new ObservableCollection<Data>();
            tmp.ForEach(x => Points.Add(new Data() {
                X = Convert.ToDouble(x.Split(' ')[0]),
                Y = Convert.ToDouble(x.Split(' ')[1])
            }));
            maxBind++;
            bindings.Add(maxBind, Points.ToList());
            comboBoxTable.Items.Add(maxBind);
            label1.Content = "Table " + maxBind.ToString();
            comboBoxTable.SelectedItem = maxBind;
            DataGrid1.ItemsSource = Points;
            UpdateGraphic();
            MessageBox.Show("Файл загружен");
        }

        private void SaveFile(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if(saveFileDialog.ShowDialog() == true)
                FilePath = saveFileDialog.FileName;
            else
                return;

            string Text = "";
            foreach(var a in Points) {
                Text += a.ToString() + '\n';
            }
            File.WriteAllText(FilePath, Text);
            MessageBox.Show("Файл сохранен");

        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int i = Convert.ToInt16(comboBoxTable.SelectedItem);
            Points = new ObservableCollection<Data>(bindings[i]);
            label1.Content = "Table " + i.ToString();
            comboBoxTable.SelectedItem = i;
            DataGrid1.ItemsSource = Points;
            UpdateGraphic();
        }

        private void RemoveFile(object sender, EventArgs e) {
            if(bindings.Count == 1) {
                MessageBox.Show("Нельзя удалить последний");
                return;
            }
            bindings.Remove((int)comboBoxTable.SelectedItem);
            int i = comboBoxTable.Items.IndexOf((int)comboBoxTable.SelectedItem);
            var tmp = bindings.LastOrDefault();
            Points = new ObservableCollection<Data>(tmp.Value);
            label1.Content = "Table " + tmp.Key.ToString();
            comboBoxTable.SelectedItem = tmp.Key;
            comboBoxTable.Items.RemoveAt(i);
            DataGrid1.ItemsSource = Points;
            UpdateGraphic();
        }

    }

    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString() { return $"{X} {Y}"; }
    }
}
