using UI_Test.NShape;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI_Test.NShape
{
    public class DPoint
    {
        //Coordinates
        public double X
        {
            get { return Canvas.GetLeft(path); }
            set { Canvas.SetLeft(path, value); }
        }
        public double Y
        {
            get { return Canvas.GetTop(path); }
            set { Canvas.SetTop(path, value); }
        }

        public double Radius
        {
            get { return circle.RadiusX; }
            set
            {
                circle.RadiusX = value * scale;
                circle.RadiusY = value * scale;
            }
        }

        internal DPoint NextPoint
        {
            get { return _NextPoint; }
            set
            {
                _NextPoint = value;
                NextLine.Dest = value;
            }
        }

        public Visibility Visibility {
            get
            {
                if (path.Visibility != NextLine.Visibility)
                {
                    Debug.Fail("Something Wrong! : 원과 선의 Visibility가 불일치");
                }
                return path.Visibility;
            }
            set
            {
                if (NextLine != null)
                {
                    NextLine.Visibility = value;
                }
                path.Visibility = value;
            }
        }

        internal DLine NextLine = null;
        internal DPoint PrevPoint = null;
        private DPoint _NextPoint = null;
        private CurveView curveView = null;
        internal EllipseGeometry circle = new EllipseGeometry();
        internal Path path = new Path();
        internal Double scale = 1.0;
        internal Point sp, mp;
        internal bool drag = false;

        internal Canvas _canvas;
        public EventHandler<DPointEventArgs> LocationChanged;
        public EventHandler PointRemoved;
        static int g_id = 8001;
        public int id = g_id++;

        public int BoxDirection = 0;

        //Constructor
        public DPoint(Point p, Canvas canvas, CurveView curveView)
        {
            SetPos(p.X, p.Y);
            init(p, canvas,curveView);
        }
        public DPoint(DPoint dp, Canvas canvas, CurveView curveView)
        {
            SetPos(dp.X, dp.Y);
            init(new Point(dp.X, dp.Y), canvas, curveView);
        }
        public DPoint(double x,double y, Canvas canvas, CurveView curveView)
        {
            SetPos(x,y);
            init(new Point(x,y), canvas, curveView);
        }

        private void init(Point p, Canvas canvas, CurveView curveView)
        {
            _canvas = canvas;
            ScaleTransform scaleTransform = Scaler.GetScaleTransform(_canvas);
            if (curveView != null)
            {
                NextLine = new DLine(this, canvas);
                this.curveView = curveView;
            }
            
            if (scaleTransform != null)
            {
                scale = 1.0 / scaleTransform.ScaleX;
                Radius = 4;
                scaleTransform.Changed += (s, e) =>
                {
                    ScaleTransform st = s as ScaleTransform;
                    scale = 1.0 / st.ScaleX;
                    Radius = 4;
                };
            }
            path.Data = circle;
            path.Fill = Brushes.LightCyan;
            path.Stroke = Brushes.Black;
            {
                path.StrokeThickness = 1 * scale;
            }
            Draw(p.X, p.Y, _canvas);
        }
       

        public void SetPos(double x, double y)
        {
            this.X = x; this.Y = y;
        }


        public override string ToString()
        {
            return String.Format("DPoint[{0} - {1} - {2}]", PrevPoint.id, id, NextPoint.id);
        }

        private DPoint Draw(double x, double y, Canvas canvas)
        {
            this.X = x;
            this.Y = y;

            _canvas = canvas;
            canvas.Children.Add(path);
            Canvas.SetZIndex(path, 10);

            /*
             * Mover
             */
            path.MouseDown += (s, e) =>
            {
                e.Handled = true;
                drag = true;
                sp = mp = e.GetPosition(canvas);
                Mouse.Capture(path);
                /*
                 * 종료처리
                 */
                if (curveView != null)
                {
                    if (this == curveView.FirstPoint)
                    {
                        if (curveView.Count() > 2)
                        {
                            curveView.Closed();
                        }
                        else
                        {
                            return; // avoid abnormal
                        }
                    }

                    if (e.ClickCount == 2)
                    {
                        drag = false;

                        NextPoint.PrevPoint = PrevPoint;
                        PrevPoint.NextPoint = NextPoint;
                        this.Dispose();
                        // Delete This Point
                        PointRemoved.Invoke(this, null);
                        curveView.Dump("DoubleClick");

                    }
                }

            };
            path.MouseUp += (s, e) =>
            {
                path.ReleaseMouseCapture();
                drag = false;
                e.Handled = true;
                // 기존 마스크가 있으면 결합한다.
            };
            path.MouseMove += (s, e) =>
            {
                mp = e.GetPosition(canvas);
                Vector dv = new Vector(mp.X - sp.X, mp.Y - sp.Y);
                if (drag)
                {
                    Move(dv);
                    DPointEventArgs args = new DPointEventArgs();
                    args.BoxDirection = BoxDirection;
                    LocationChanged.Invoke(this, new DPointEventArgs());
                    sp = mp;
                }
            };
            path.MouseEnter += (s, e) =>
            {
                Radius = 7;
            };
            path.MouseLeave += (s, e) =>
            {
                Radius = 4;
            };
            return this;
        }

        internal void Move(Vector dd)
        {
            X += dd.X; Y += dd.Y;
            if (curveView != null)
            {
                PrevPoint.NextLine.Dest = this;
                NextLine.Source = this;
            }

        }

        internal void Dispose()
        {
            if( NextLine != null)
            {
                NextLine.Dispose();
            }
            _canvas.Children.Remove(path);
        }

        //public DPoint(Canvas canvas, BallVM ballVM) // NotUsed 참고용
        //{
        //    //XBinding(circle, EllipseGeometry.RadiusXProperty, "Size", ballVM);
        //    //XBinding(circle, EllipseGeometry.RadiusYProperty, "Size", ballVM);
        //    //XBinding(circle, EllipseGeometry.CenterProperty, "Center", ballVM);

        //    Monitor.Inst.scaleTransform.Changed += (object sender, EventArgs e) =>
        //    {
        //        scale = (1.0 / Monitor.Inst.scaleTransform.ScaleX);
        //        scale = scale < 1 ? 1 : scale;
        //        path.StrokeThickness = 3 * scale;
        //    };
        //}
    }

    public class DPointEventArgs : EventArgs
    {
        public int BoxDirection;
    }

}
