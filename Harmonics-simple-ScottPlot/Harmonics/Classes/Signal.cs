

namespace WpfApp.Classes
{
    class Signal
    {
        double min, max, step;
        Func<double, double, double> f;
        int n;
        public List<double> coordinates;
        public ScottPlot.Color color;
        public string text;
        public bool active = true;

        public Signal(double min, double max, double step, Func<double, double, double> f, int n, string text, ScottPlot.Color color)
        {
            this.min = min;
            this.max = max;
            this.step = step;
            this.f = f;
            this.n = n;
            coordinates = new List<double>();
            this.text = "signal " + text;
            this.color = color;

            UpdateSignal();
        }

        public void UpdateSignal()
        {
            coordinates.Clear();
            for (double x = min; x < max; x += step)
            {
                double y = f(x, n);
                coordinates.Add(y);
            }
        }
    }
}
