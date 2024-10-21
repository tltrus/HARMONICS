using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp.Classes
{
    class Signal
    {
        List<Point> points = new List<Point>();
        double min, max, step;
        Func<double, double, double> f;
        Axis axis;
        int n;
        Brush brush;
        double thickness;

        public Signal(Axis axis, double min, double max, double step, Func<double, double, double> f, int n, Brush brush, double thickness)
        {
            this.axis = axis;
            this.min = min;
            this.max = max;
            this.step = step;
            this.f = f;
            this.n = n;
            this.brush = brush;
            this.thickness = thickness;
        }

        public void Update()
        {
            points.Clear();
            for (double x = min; x < max; x += step)
            {
                double y = f(x, n);
                points.Add(new Point(axis.Xto(x), axis.Yto(y)));    // original signal
            }
        }

        public void Drawing(DrawingContext dc) => DrawLine(dc, brush, points, thickness);

        private void DrawLine(DrawingContext dc, Brush brush, List<Point> points, double thickness = 1)
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(points[0], false, false);
                geometryContext.PolyLineTo(points, true, true);
            }
            dc.DrawGeometry(null, new Pen(brush, thickness), streamGeometry);
        }
    }
}
