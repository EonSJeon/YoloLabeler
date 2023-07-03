using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI_Test.NShape
{
    internal class DLine
    {
        private Canvas _canvas;
        private Line line;
        private Line GLine;
        private Point sp, mp;
        public event EventHandler<Point> LineSplited;
        internal DPoint parent = null;

        public Visibility Visibility
        {
            get
            {
                if (line.Visibility != GLine.Visibility)
                {
                    Debug.Fail("Something Wrong! : line과 GLine의 Visibility가 불일치");
                }
                return line.Visibility;
            }
            set
            {
                line.Visibility = value;
                GLine.Visibility = value;
            }
        }
        public DPoint Source
        {
            set
            {
                line.X1 = value.X; line.Y1 = value.Y;
                GLine.X1 = value.X; GLine.Y1 = value.Y;
            }
        }

        public DPoint Dest
        {
            set
            {
                line.X2 = value.X; line.Y2 = value.Y;
                GLine.X2 = value.X; GLine.Y2 = value.Y;
            }
        }

        public DLine(DPoint dp, Canvas canvas)
        {
            _canvas = canvas;
            line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1;
            Canvas.SetZIndex(line, 9);
            canvas.Children.Add(line);

            // 선을 선택하기 편하게 하기 위하여 이중선을 사용함.
            GLine = new Line();
            GLine.Stroke = Brushes.Transparent;
            GLine.StrokeThickness = 10;
            Canvas.SetZIndex(GLine, 9);
            canvas.Children.Add(GLine);
            parent = dp;
            Source = dp;
            Dest = dp;

            /*
            * 분할기
            */
            GLine.MouseDown += (s, e) =>
            {
                sp = mp = e.GetPosition(canvas);
                e.Handled = true;
                if (LineSplited != null)
                {
                    LineSplited.Invoke(this, mp);
                }
            };
            GLine.MouseEnter += (s, e) =>
            {
                line.Stroke = Brushes.Yellow;
            };
            GLine.MouseLeave += (s, e) =>
            {
                line.Stroke = Brushes.Black;
            };
        }

        internal void Dispose()
        {
            _canvas.Children.Remove(line);
            _canvas.Children.Remove(GLine);
        }
    }
}
