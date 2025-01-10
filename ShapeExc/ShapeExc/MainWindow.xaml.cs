using System;
using System.Collections.Generic;
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

namespace ShapeExc
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

    }

    public partial class TestCanvas : Page
    {
        public TestCanvas()
        {
            Canvas myCanvas = new Canvas();

            Line myLine = new Line();

            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;

            myLine.X1 = 1;
            myLine.Y1 = 1;
            myLine.X2 = 50;
            myLine.Y2 = 50;

            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;

            myLine.StrokeThickness = 10;

            myCanvas.Children.Add(myLine);

            this.Content = myCanvas;


        }
    }
}
