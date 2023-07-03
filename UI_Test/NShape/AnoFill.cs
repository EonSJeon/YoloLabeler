
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Brushes = System.Windows.Media.Brushes;
using System.Windows;

using System.Windows.Media;
using System.Diagnostics;
using System;
using System.ComponentModel;
using UI_Test;
using UI_Test.NShape;

namespace UI_Test.NShape
{
    public class AnoFill : INotifyPropertyChanged
    {
        public Canvas _canvas;
        public Polygon polygon = new Polygon();
        public TextBlock Title = new TextBlock();
        private Point sp, mp;//start point, mouse point

        public event EventHandler<Vector> AreaMoved;
        public event EventHandler<Point> MouseClicked;
        //RoutedEvent 개념으로 나중에 삭제되어야 함.
        public event EventHandler CtrlCKeyDown;
        public event EventHandler CtrlVKeyDown;
        public event EventHandler DelKeyDown;
        public event PropertyChangedEventHandler? PropertyChanged;


        [JsonRepo(Name = "Name")]
        public string Name { get; set; }

        public Visibility Visibility
        {
            get => polygon.Visibility;
            set
            {
                polygon.Visibility = value;
                Title.Visibility = value;
            }
        }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                Title.Foreground = value ? Brushes.Red : Brushes.Green;
                polygon.Fill = value ? Brushes.LightCoral : Brushes.LightSeaGreen;
                _Selected = value;
            }
        }

        public Point Location
        {
            get
            {
                double mx = Double.MaxValue;
                double my = Double.MaxValue;
                foreach (Point p in polygon.Points)
                {
                    mx = (p.X < mx) ? p.X : mx;
                    my = (p.Y < my) ? p.Y : my;
                }
                return new Point(mx, my);
            }
        }

        private enum MouseMode
        {
            NONE,
            CLICKED,
            DRAGED,
        }
        private MouseMode _mouseMode = MouseMode.NONE;

        //Constructor
        public AnoFill()
        {
            polygon.Stroke = Brushes.Yellow;
            polygon.Opacity = 0.4;
            polygon.Fill = Brushes.LightSeaGreen;
            polygon.StrokeThickness = 1;
            polygon.Focusable = true;

            polygon.MouseDown += (s, e) =>
            {
                _mouseMode = MouseMode.CLICKED;
                Mouse.Capture(polygon);
                sp = mp = e.GetPosition(_canvas);
                Selected = true;
                polygon.Focus();
                e.Handled = true;
            };
            polygon.MouseUp += (s, e) =>
            {
                if (_mouseMode == MouseMode.CLICKED)
                {
                    if (MouseClicked != null)
                    {
                        MouseClicked.Invoke(this, sp);
                    }
                }
                _mouseMode = MouseMode.NONE;
                polygon.ReleaseMouseCapture();
                e.Handled = true;
            };
            polygon.MouseMove += (s, e) =>
            {
                if (_mouseMode != MouseMode.NONE)
                {
                    mp = e.GetPosition(_canvas);
                    Vector dv = Point.Subtract(mp, sp);
                    if (dv.Length > 3)
                    {
                        _mouseMode = MouseMode.DRAGED;
                        Move(dv);
                        if (AreaMoved != null)
                        {
                            AreaMoved.Invoke(this, dv);
                        }
                        sp = mp;
                    }
                }
                e.Handled = true;
            };
            polygon.KeyDown += (s, e) =>
            {
                if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    CtrlCKeyDown.Invoke(this, null);
                    e.Handled = true;
                }
                if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    CtrlVKeyDown.Invoke(this, null);
                    e.Handled = true;
                }
                if (e.Key == Key.Delete)
                {
                    DelKeyDown.Invoke(this, null);
                    e.Handled = true;
                }
            };
        }
        public void Move(Vector delta)
        {
            PointCollection pointCollection = new PointCollection();
            var points = polygon.Points;
            foreach (Point p in points)
            {
                pointCollection.Add(Point.Add(p, delta));
            }
            polygon.Points = pointCollection;
            points = pointCollection;

            Point loc = Location;
            Canvas.SetTop(Title, loc.Y);
            Canvas.SetLeft(Title, loc.X);
        }


        //Draw Polymorphism
        internal void Draw(OpenCvSharp.Point[] contour, Canvas canvas)//BrushView
        {
            PointCollection pointCollection = new PointCollection();
            foreach (OpenCvSharp.Point p in contour)
            {
                pointCollection.Add(new Point(p.X, p.Y));
            }
            _SetPointCollection(pointCollection, canvas);
        }
        internal void Draw(DPoint first, Canvas canvas)//CurveView
        {
            if (first != null)
            {
                PointCollection pointCollection = new PointCollection();
                DPoint np = first;
                do
                {
                    pointCollection.Add(new Point(np.X, np.Y));
                    if (np.NextPoint == null)
                    {
                        return;
                    }
                    np = np.NextPoint;
                } while (np != first);
                _SetPointCollection(pointCollection, canvas);
            }
        }
        internal void Draw(DPoint[] points, Canvas canvas) //BoxView
        {

            PointCollection pointCollection = new PointCollection();
            foreach (DPoint np in points)
            {
                pointCollection.Add(new Point(np.X, np.Y));
            }
            _SetPointCollection(pointCollection, canvas);

        }

        private void _SetPointCollection(PointCollection pointCollection, Canvas canvas)
        {
            this._canvas = canvas;
            polygon.Points = pointCollection;
            if (!_canvas.Children.Contains(polygon))
            {
                _canvas.Children.Add(polygon);
            }

            //temp (전상언) 이 위치 애매함
            {
                Point loc = Location;
                Canvas.SetTop(Title, loc.Y);
                Canvas.SetLeft(Title, loc.X);
                Title.Foreground = Brushes.Green;
                Title.FontSize = 20;
                if (!_canvas.Children.Contains(Title))
                {
                    _canvas.Children.Add(Title);
                }
            }
        }

        public virtual void Dispose()
        {
            if (_canvas.Children.Contains(polygon))
            {
                _canvas.Children.Remove(polygon);
            }
            if (_canvas.Children.Contains(Title))
            {
                _canvas.Children.Remove(Title);
            }
            Debug.WriteLine("Delete anoFill");
        }
    }


}
