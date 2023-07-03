
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI_Test.NShape
{
    internal class BoxView : AnoLabel
    {
        [JsonRepo(Name = "X")]
        public double X
        {
            get { return Canvas.GetLeft(boundBox); }
            set { Canvas.SetLeft(boundBox, value); }
        }
        [JsonRepo(Name = "Y")]
        public double Y
        {
            get { return Canvas.GetTop(boundBox); }
            set { Canvas.SetTop(boundBox, value); }
        }

        [JsonRepo(Name = "Height")]
        public double Height
        {
            get { return boundBox.Height; }
            set { boundBox.Height = value; }
        }

        [JsonRepo(Name = "Width")]
        public double Width
        {
            get { return boundBox.Width; }
            set { boundBox.Width = value; }
        }

        Rectangle boundBox = new Rectangle();//(전상언) 테두리 선
        Point startPoint;
        DPoint[] dPoints = new DPoint[4];

        //Constructor
        public BoxView()
        {
            Selected = false;
            this.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Visibility":
                        boundBox.Visibility = Visibility;
                        if (dPoints != null)
                        {
                            foreach (DPoint p in dPoints)
                            {
                                if (p != null)
                                {
                                    p.Visibility = Visibility;
                                }
                            }
                        }
                        break;
                    case "Selected":
                        LabelListPanel.Inst.DGLabel.SelectedItem = this;
                        break;
                }
            };
        }


        public override void MovePoints(Vector e, bool isFIllMove=true)
        {
            X += e.X; Y += e.Y;
            startPoint.X += e.X; startPoint.Y += e.Y;
            foreach (DPoint p in dPoints)
            {
                p.Move(e);
            }
            if (isFIllMove)
            {
                 Move(e);
            }
            Name = string.Format("좌표 ({0:N0},{1:N0})", X, Y);
        }

        //Animation Functions
        public override void First(Point p, Canvas canvas)
        {
            this._canvas = canvas;
            startPoint = p;
            X = p.X; Y = p.Y;
             _canvas = canvas;
            initBox();
        }
        public override void Next(Point p, Vector delta)
        {
            X = startPoint.X; Y = startPoint.Y;
            var w = p.X - startPoint.X;
            var h = p.Y - startPoint.Y;
            Width = w < 0 ? 10 : w;
            Height = h < 0 ? 10 : h;
            FitPoint();
        }
        public override bool Ending(Point p)
        {
            var tw = p.X - startPoint.X;
            var th = p.Y - startPoint.Y;
            if (tw > 10 && th > 10)
            {
                Width = tw; Height = th;
                FitPoint();
                 Draw(dPoints, _canvas);
                Name = string.Format("좌표 ({0:N0},{1:N0})", X, Y);
                return true;
            }
            else
            {
                _canvas.Children.Remove(boundBox);
                
                Dispose();
               
                return false;
            }
        }

        public void initBox()
        {
            boundBox.Stroke = Brushes.Green;
            boundBox.StrokeThickness = 1;
            if (!_canvas.Children.Contains(boundBox))
            {
                _canvas.Children.Add(boundBox);
            }
            initDPoints();
        }
        private void initDPoints()
        {
            dPoints[0] = AddPoint(new Point(X, Y), 1);
            dPoints[1] = AddPoint(new Point(X + Width, Y), 2);
            dPoints[2] = AddPoint(new Point(X + Width, Y + Height), 3);
            dPoints[3] = AddPoint(new Point(X, Y + Height), 4);
        }

        private void FitPoint()
        {
            dPoints[0].SetPos(X, Y);
            dPoints[1].SetPos(X + Width, Y);
            dPoints[2].SetPos(X + Width, Y + Height);
            dPoints[3].SetPos(X, Y + Height);
             Draw(dPoints, _canvas);
            Name = string.Format("좌표 ({0:N0},{1:N0})", X, Y);
        }

        public DPoint AddPoint(Point p, int direction)
        {
            var np = new DPoint(p, _canvas, null);
            np.BoxDirection = direction;
            np.LocationChanged += (s, e) =>
            {
                DPoint myPoint = s as DPoint;
                switch (myPoint.BoxDirection)
                {
                    case 1:
                        X = myPoint.X;
                        Width -= myPoint.X - startPoint.X;
                        startPoint.X = myPoint.X;
                        Y = myPoint.Y;
                        Height -= myPoint.Y - startPoint.Y;
                        startPoint.Y = myPoint.Y;
                        break;
                    case 2:
                        Width += (myPoint.X - Width) - startPoint.X;
                        startPoint.X = (myPoint.X - Width);
                        Y = myPoint.Y;
                        Height -= myPoint.Y - startPoint.Y;
                        startPoint.Y = myPoint.Y;
                        break;
                    case 3:
                        Width = myPoint.X - startPoint.X;
                        Height = myPoint.Y - startPoint.Y;
                        break;
                    case 4:
                        X = myPoint.X;
                        Width -= myPoint.X - startPoint.X;
                        startPoint.X = myPoint.X;
                        Height += (myPoint.Y - Height) - startPoint.Y;
                        startPoint.Y = (myPoint.Y - Height);
                        break;
                }
                FitPoint();

            };
            return np;
        }
        internal void Build(Canvas canvas)
        {
            _canvas = canvas;
            startPoint = new Point(X, Y);
            initBox();
            Selected = false;
            Draw(dPoints, _canvas);
        }

        public override void Closed()
        {
            Debug.WriteLine("Closed 193");
        }
        public override void Dispose()
        {
            // 가상메소드를 override하면 자식 클래스에서 재정의된 메소드를 호출함
            if (_canvas.Children.Contains(boundBox))
            {
                _canvas.Children.Remove(boundBox);
            }
            if (_canvas.Children.Contains(polygon))
            {
                _canvas.Children.Remove(polygon);
            }
            if (_canvas.Children.Contains(Title))
            {
                _canvas.Children.Remove(Title);
            }
            foreach (DPoint dpoint in dPoints)
            {
                dpoint.Dispose();
            }
            Debug.WriteLine("Delete BoundBox");
        }

        public override AnoLabel Clone()
        {
            BoxView nBoxView = new BoxView();

            nBoxView.SetAnoLabelFields(Parent, _canvas, classInfo);
            //debug
            {
                Debug.WriteLine("************temp1: "+nBoxView.classInfo.Title);
            }
            
            nBoxView.X = this.X;
            nBoxView.Y = this.Y;
            nBoxView.Width = this.Width;
            nBoxView.Height = this.Height;
            nBoxView.Build(_canvas);

            //debug
            {
                Debug.WriteLine("*************classInfo.Title  : " + nBoxView.classInfo.Title);
                Debug.WriteLine("*************text.Text: "+nBoxView.Title.Text);
            }

            return nBoxView;
        }


    }

    public class BoxModelList : ObservableCollection<BoxModel>
    {
        public BoxModelList()
        {
            for (int x = 1; x < 5; x++)
            {
                this.Add(new BoxModel(x * 50, 80));
            }
        }
    }

    public class BoxModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal BoxView view;

        public double X { get; set; }
        public double Y { get; set; }
        public double Y2 { get; set; }
        public double BY { get; set; } = 80;

        public BoxModel(double x, double y)
        {
            this.X = x;
            this.Y = (int)y;
        }
    }
}
