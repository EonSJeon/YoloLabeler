using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using static OpenCvSharp.ML.DTrees;
using Point = System.Windows.Point;

// 생성, 이동, 분할
// 최종에서 테두리는 투명적용하지 않아야 함.
namespace UI_Test.NShape
{
    public class CurveView : AnoLabel
    {
        public DPoint LastPoint = null;
        public DPoint FirstPoint = null;

        public CurveView()
        {
            this.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Visibility":
                            Debug.WriteLine("The Visibility of the CurveView has been changed.");
                            DPoint dp = FirstPoint;
                            do
                            {
                                dp = dp.NextPoint;
                                dp.PrevPoint.Visibility = Visibility;
                            } while (dp != FirstPoint && dp != null);
                            if (isClosed)
                            {
                            }
                        break;
                    case "Selected":
                        LabelListPanel.Inst.DGLabel.SelectedItem = this;
                        break;
                }
            };
        }

        public override void MovePoints(Vector v, bool isFillMove)
        {
            DPoint np = FirstPoint;
            do
            {
                np.Move(v);
                if (np.NextPoint == null)
                {
                    break;
                }
                np = np.NextPoint;
            } while (np != FirstPoint);
            if (isFillMove)
            {
                Move(v);
            }
            Name = string.Format("좌표 ({0:N0},{1:N0})", np.X, np.Y);
        }

        public override void First(Point p, Canvas canvas)
        {
            if (!isClosed)
            {
                this._canvas = canvas;
                DPoint dp = AddPoint(p, this);
                if (FirstPoint == null)
                {
                    FirstPoint = dp;
                }
                else
                {
                    AddLine(LastPoint, dp);
                }
                LastPoint = dp;
                Name = string.Format("좌표 ({0:N0},{1:N0})", FirstPoint.X, FirstPoint.Y);
            }
        }
        public override void Next(Point p, Vector delta)
        {
        }
        public override bool Ending(Point p)
        {
            return false; // add 하지 마시요.
        }

        public void Dump(string debug)
        {
            DPoint np = FirstPoint;
            Debug.WriteLine("*********** {0} *************", debug);
            do
            {
                Debug.WriteLine("Dump " + np.ToString());
                if (np.NextPoint == null)
                {
                    break;
                }
                np = np.NextPoint;
            } while (np != FirstPoint);
        }

        //AddPoint Polymorphism
        public DPoint AddPoint(Point p, CurveView curveView)
        {
            var p3 = new DPoint(p, _canvas, curveView);
            nextDPointInit(p3);
            return p3;
        }
        public DPoint AddPoint(DPoint dp, CurveView curveView)
        {
            var p3 = new DPoint(new Point(dp.X, dp.Y), _canvas, curveView);
            nextDPointInit(p3);
            return p3;
        }

        private void nextDPointInit(DPoint dp)
        {
            dp.LocationChanged += (s, e) =>
            {
                Draw(FirstPoint, _canvas);
            };
            dp.PointRemoved += (s, e) =>
            {
                Draw(FirstPoint, _canvas);
            };
            dp.NextLine.LineSplited += EventLineSplit;
        }

        public void AddLine(DPoint p1, DPoint p2)
        {
            p1.NextPoint = p2;
            p2.PrevPoint = p1;
        }
        public override void Closed()
        {
            AddLine(LastPoint, FirstPoint);
            Draw(FirstPoint, _canvas);
            isClosed = true;

            // for debugging
            
        }
        public void EventLineSplit(object pline, Point p)
        {
            DLine myLine = (DLine)pline;
            DPoint myPoint = myLine.parent;
            DPoint np = AddPoint(p, this);
            {
                np.NextPoint = myPoint.NextPoint;
                np.PrevPoint = myPoint;
                myPoint.NextPoint.PrevPoint = np;
                myPoint.NextPoint = np;
            }
            Mouse.Capture(np.path);
            np.sp = p;
            np.drag = true;
            // Draw(FirstPoint, _canvas); Error Point , 선에서 표시한다.
        }

        public override void Dispose()
        {
            _canvas.Children.Remove(polygon);
            DPoint np = FirstPoint;
            do
            {
                np = np.NextPoint;
                np.PrevPoint.Dispose();
            } while (np != FirstPoint && np !=null);
        }

        public override AnoLabel Clone()
        {
            CurveView nCurveView = new CurveView();

            nCurveView.SetAnoLabelFields(this.Parent,this._canvas, this.classInfo);

            DPoint cp = this.FirstPoint;
            DPoint newcp = new DPoint(cp, _canvas, nCurveView);
            nCurveView.FirstPoint = newcp;

            do
            {
                newcp.NextPoint = new DPoint(cp.NextPoint, _canvas, nCurveView);
                newcp.NextPoint.PrevPoint = newcp;

                nCurveView.AddEvent(newcp);

                newcp = newcp.NextPoint;
                cp = cp.NextPoint;
            } while (cp != FirstPoint);

            newcp.NextPoint = nCurveView.FirstPoint;
            nCurveView.FirstPoint.PrevPoint = newcp;

            nCurveView.isClosed = true;

            nCurveView.Draw(nCurveView.FirstPoint, _canvas);
            return nCurveView;
        }

        internal void Build(Canvas canvas)
        {
            _canvas = canvas;
            Draw(FirstPoint, _canvas);
            Debug.WriteLine(FirstPoint, "CurveView:FirstPoint");
        }

        public void AddEvent(DPoint dp)//Logical Function
        {
            dp.LocationChanged += (s, e) =>
            {
                Draw(FirstPoint, _canvas);
            };
            dp.PointRemoved += (s, e) =>
            {
                Draw(FirstPoint, _canvas);
            };
            dp.NextLine.LineSplited += EventLineSplit;
            if (dp == null)
            {
                Debug.WriteLine("dp is null");
            }
        }

        public int Count()
        {
            int count = 0;
            DPoint cp = FirstPoint;
            do
            {
                cp = cp.NextPoint;
                count++;
            } while (cp != FirstPoint && cp != null);

            return count;
        }
    }
    
    internal class CurModel : BoxModel
    {
        private double x;
        private double y;
        public CurveView curView;

        public CurModel(double x, double y) : base(x, y)
        {
            this.x = x;
            this.y = y;
        }
    }

}
