using ScottPlot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.Classes;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        Random rnd = new Random();
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
            int num = 5;

            sl.Value = num;
            lb.Content = num;

            CreateSignals(num);
            Drawing();
            CreateCheckBoxes(num);
        }
        void CreateCheckBoxes(int num)
        {
            wp.Children.Clear();

            for (int i = 0; i < num; ++i)
            {
                var c = signals[i].color;
                var text = signals[i].text;
                var brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
                CheckBox checkBox = new CheckBox { Content = text, MinHeight = 20, Foreground = brush, Margin = new Thickness(5, 0, 0, 5), IsChecked = true };
                checkBox.Click += CheckBox_Clicked;
                wp.Children.Add(checkBox);
            }
        }
        private void CreateSignals(int num)
        {
            signals.Clear();
            IPalette palette = new ScottPlot.Palettes.Category20();

            for (int i = 1; i < num; ++i)
            {
                var color = palette.GetColor(i);
                signals.Add(new Signal(-20, 20, 0.1, F, i, i.ToString(), color));
            }
            signals.Add(new Signal(-20, 20, 0.1, F_total, num, "total", palette.GetColor(0)));
        }

        private void Drawing()
        {
            WpfPlot1.Plot.Clear();

            foreach (var signal in signals) 
            {
                if (!signal.active) continue; // skip if signal is not active

                var sig = WpfPlot1.Plot.Add.Signal(signal.coordinates);
                sig.LegendText = signal.text;
                sig.Color = signal.color;
            }
            WpfPlot1.Plot.ShowLegend();
            WpfPlot1.Refresh();
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            var listCheckBoxes = GetChildren(wp);

            // 1. Scanning all checkboxes.
            // 2. Find signal item with same name of checkbox
            // 3. Write status of checkbox in signal.active
            foreach (var cb in listCheckBoxes)
            {
                var res = signals.Find(x => x.text == cb.Content.ToString());
                if (res is null) return;

                if (cb.IsChecked == true)
                {
                    res.active = true;
                }
                else
                {
                    res.active = false;
                }
            }

            Drawing();
        }
        private void sl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var num = (int)e.NewValue;

            if (lb == null) return;
            lb.Content = num;

            CreateSignals(num);
            Drawing();
            CreateCheckBoxes(num);
        }


        List<CheckBox> GetChildren(Panel p)
        {
            List<UIElement> UI = p.Children.Cast<UIElement>().ToList();
            List<CheckBox> list = new List<CheckBox>();

            foreach (var elem in UI.OfType<CheckBox>())
            {
                list.Add(elem);
            }

            return list;
        }
    }
}