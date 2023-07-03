using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace UI_Test.NShape
{
    internal class MouseHanderX
    {
        private Canvas uCanvas;
        internal bool drag;
        internal Point sp, mp;

        public event EventHandler MouseDraged;
        public event EventHandler MouseClicked;
        internal Rectangle box = new Rectangle();



        public void BoxOn()
        {
            box.Width = 0;
            box.Height = 0;
            Canvas.SetTop(box, sp.Y);
            Canvas.SetLeft(box, sp.X);
            uCanvas.Children.Add(box);
        }

        public void BoxDraw()
        {
            var dx = mp.X - sp.X;
            var dy = mp.Y - sp.Y;
            box.Width = dx < 0 ? 0 : dx;
            box.Height = dy < 0 ? 0 : dy;
        }


        public MouseHanderX( Object source, UIElement target, Canvas uCanvas)
        {
            this.uCanvas = uCanvas;
            box.Stroke = Brushes.Yellow; 
            box.StrokeThickness = 1;
            target.MouseDown += (s, e) =>
            {
                Mouse.Capture(target);
                sp = mp = e.GetPosition(uCanvas);
                drag = true;
                if (MouseDraged != null)
                {
                    BoxOn();
                }
                e.Handled = true;
            };
            target.MouseUp += (s, e) =>
            {
                target.ReleaseMouseCapture();
                if (drag)
                {
                    drag = false;
                    mp = e.GetPosition(uCanvas);
                    {
                        if (MouseDraged != null)
                        {
                            uCanvas.Children.Remove(box);
                            MouseDraged.Invoke(source, EventArgs.Empty);
                        } else
                        {
                            if ( MouseClicked != null)
                            {
                                MouseClicked(source, EventArgs.Empty);
                            }
                        }
                    }
                    uCanvas.Children.Remove(box);
                    e.Handled = true;
                }
            };
            target.MouseMove += (s, e) =>
            {
                if (drag)
                {
                    mp = e.GetPosition(uCanvas);
                    {
                        if (MouseDraged != null)
                        {
                           
                                BoxDraw();
                            
                        }
                    }
                    e.Handled = true;
                }

            };
        }
    }
}
