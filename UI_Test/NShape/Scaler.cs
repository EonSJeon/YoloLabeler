using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UI_Test.NShape
{
    internal class Scaler
    {
        private ScaleTransform invTransform = new ScaleTransform();
        private ScaleTransform scaleTransform = new ScaleTransform();
        private TransformGroup transformGroup = new TransformGroup();
        private TranslateTransform transform = new TranslateTransform();
        private Canvas _canvas = null;
        private Border _border = null;
        private double Scale
        {
            get
            {
                return scaleTransform.ScaleX;
            }
            set
            {
                scaleTransform.ScaleX = value;
                scaleTransform.ScaleY = value;
                if (scaleTransform.ScaleX < 1)
                {
                    invTransform.ScaleX = 1.0 / scaleTransform.ScaleX;
                    invTransform.ScaleY = 1.0 / scaleTransform.ScaleY;
                }
                else
                {
                    invTransform.ScaleX = 1.0;
                    invTransform.ScaleY = 1.0;
                }
            }
        }
        public Scaler(Canvas canvas, Border border)
        {
            this._canvas = canvas;
            this._border = border;
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(transform);
            canvas.RenderTransform = transformGroup;
            canvas.MouseWheel += MouseWheel;
        }

        new private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point sp = e.GetPosition(_canvas);
            var OldScale = Scale;
            double step = e.Delta > 0 ? .05 : -.05;
            Scale += step; // 스케일 적용

            /*
             * 특정범위로 제한
             */
            if (Scale > 0 && Scale < 4.0 && Scale > 0.06)
            {
            }
            else
            {   // Rollback
                Scale = OldScale;
                return;
            }

            //MainWindow window = DvGlobal.MainWindow;
            //Vector ViewPort = new Vector(
            //    window.MainBorder.ActualWidth,
            //    window.MainBorder.ActualHeight);

            //var CanvasSize = new Vector(_canvas.Width, _canvas.Height);
            //Vector r = (ViewPort - CanvasSize * Scale) / 2;

            //if (r.X > 0 || r.Y > 0) // R<1.0 , 모니터 화면보다 작게 표시될때. / Panning 과 충돌됨.
            //{
            //    transform.X = r.X;
            //    transform.Y = r.Y;
            //}
            //else
            //{
            /*
             * 확대에 따른 센터보정
             */
            Point ep = e.GetPosition(_canvas);
            Vector v = sp - ep;
            transform.X -= v.X * Scale;
            transform.Y -= v.Y * Scale;

            //    if (transform.X > 0)
            //    {
            //        transform.X = 0;
            //    }
            //    if (transform.Y > 0)
            //    {
            //        transform.Y = 0;
            //    }
            //}

        }

        public void PanningMove(Vector delta)
        {
            /*
                   * 전체화면보다 적은배율에서만 Panning 작동함.
                   */

            //var FitR = Math.Min((Outline.X / this.Width), (Outline.Y / this.Height));
            //if (FitR < PlayBase.ScaleR || true) // JJY TODO
            //{
            delta *= Scale;
            transform.X -= delta.X;
            transform.Y -= delta.Y;
            //  ScreenLimit();
            //   _mp = e.GetPosition(_canvas);
            //  }

        }

        private void FitCenter()
        {
            // 현재 보이는 크기 / 출력창크기
            var vx = _canvas.ActualWidth * Scale / _border.ActualWidth;
            var vy = _canvas.Height * Scale / _border.ActualHeight;
        }

        public static ScaleTransform GetScaleTransform(Canvas canvas)
        {
            TransformGroup tg = (TransformGroup)(canvas.RenderTransform);
            foreach (var st in tg.Children)
            {
                if (st.GetType() == typeof(ScaleTransform))
                {
                    ScaleTransform scaleTransform = (ScaleTransform)st;
                    return scaleTransform;
                }
            }
            return null;
        }
    }
}
