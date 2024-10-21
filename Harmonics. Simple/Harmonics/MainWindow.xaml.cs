using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Classes;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        Random rnd = new Random();
        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        Point mouse;
        int factor = 25;
        Func<double, double, double> F = (x, n) => 1/n * Math.Sin(x * n);
        Func<double, double, double> F_total = (x, n) =>
        {
            double result = 0;
            for (int i = 1; i < n; ++i)
            {
                result += 1.0 / i * Math.Sin(x * i);
            }

            return result;
        };

        List<Signal> signals = new List<Signal>();

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            width = g.Width;
            height = g.Height;

            mouse = new Point();
            visual = new DrawingVisual();

            axis = new Axis(width, height);
            axis.SetFactor(factor);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            Calculated(2);

            Drawing();
        }
        private void Calculated(int num)
        {
            signals.Clear();
            for (int i = 1; i < num; ++i)
            {
                signals.Add(new Signal(axis, -20, 20, 0.01, F, i, Brushes.White, 0.7));
            }

            signals.Add(new Signal(axis, -20, 20, 0.01, F_total, num, Brushes.Red, 2));
        }
        private void g_MouseMove(object sender, MouseEventArgs e)
        {
            mouse = e.GetPosition(g);
            Drawing();
        }
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                factor -= 1;
            }
            else
            {
                // increasing
                factor += 1;
            }

            if (factor <= 0) factor = 1;
            if (factor >= 58) factor = 58;

            Drawing();
        }
        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                // axis drawing
                axis.Draw(dc, visual);
                axis.SetFactor(factor); // for mouse wheel

                // signals draw
                foreach (var signal in signals)
                {
                    signal.Update();
                    signal.Drawing(dc);
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) => Drawing();
        private void timerTick(object sender, EventArgs e) => Drawing();
        private void sl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var num = (int)e.NewValue;

            if (axis is null) return;
            Calculated(num);

            lb.Content = num;

            Drawing();
        }
    }
}