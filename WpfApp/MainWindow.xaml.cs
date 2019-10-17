using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        
        //scale
        private double k = 20;

        //polyline id
        int id = 0;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Points = new ObservableCollection<Data>();
            Points.Add(new Data { X = 0, Y = 0 });
            Points.Add(new Data { X = 1, Y = 1 });
            Points.Add(new Data { X = 2, Y = 4 });
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

            //Path a = new Path();
            //PathSegment s = new PathSegment();
            //PathFigure b = new PathFigure();
            //b.StartPoint = (Point)Points[0];
            //Points[0]
            //b.Segments.Add(new BezierSegment(Points.Select(x => new Point { X = x.X * k + w / 2, Y = h / 2 - x.Y * k })));

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
    }

    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString() { return $"{X} {Y}"; }
    }
}
