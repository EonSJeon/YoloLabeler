using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
// 선크기 맞추기

namespace UI_Test.NShape
{
    public class BrushView : AnoLabel
    {
        Polyline _freeLine = new Polyline();
        public int Thickness { get; set; } = 20;
        Point p0;
        public PointCollection Points
        {
            get
            {
                return this.polygon.Points;
            }
            set
            {
                Debug.WriteLine("The Visibility of the BrushView has been changed.");
                polygon.Points = value;
            }
        }//(전상언) 나중에 헷갈릴 가능성? =>GetStringPoint()


        //public override bool IsSelected
        //{
        //    get { return true; }
        //    set { }
        //}
        //public override Visibility Visibility
        //{
        //    get => _Visibility;
        //    set
        //    {
                
        //        Visibility = value;
        //        _Visibility = value;
        //    }
        //}

        //Constructor
        public BrushView()
        {
            _freeLine = new Polyline
            {
                StrokeDashCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                Stroke = Brushes.LightSeaGreen,
                StrokeThickness = Thickness,
                Opacity = 0.5,
            };
        }


        public override void First(Point p, Canvas canvas)
        {
            _canvas = canvas;
            _freeLine.Points.Add(p);
            _canvas.Children.Add(_freeLine);
            p0 = p;
        }
        public override void Next(Point p, Vector delta)
        {
            if (Distance(p0, p) > 15)
            {
                _freeLine.Points.Add(p);
                p0 = p;
            }
        }
        public override bool Ending(Point p)
        {
            _freeLine.Points.Add(p);
            GetOutline(Thickness);
            
            {
                _freeLine.Points.Clear();
                _canvas.Children.Remove(_freeLine);
            }
            return true;
        }



        private void GetOutline(int Thickness)
        {
            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();
            foreach (Point point in _freeLine.Points)
            {
                points.Add(new OpenCvSharp.Point(point.X, point.Y));
            }

            /*
             *  자유곡선
             */
            List<List<OpenCvSharp.Point>> xpoints = new List<List<OpenCvSharp.Point>>() { points };
            Mat mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
            mat.Polylines(xpoints, false, Scalar.White, Thickness);

            /*
             * 아웃라인
             */
            var max = 0.0;
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
            List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();

            OpenCvSharp.Point[] max_contour = new OpenCvSharp.Point[] { };

            foreach (OpenCvSharp.Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                if (length > max)
                {
                    max = length;
                    max_contour = p;
                    //  conture = Cv2.ApproxPolyDP(p, length * 0.005, true);
                }
                if (length > 100) // 100
                {
                    new_contours.Add(p);
                }
            }

            /*
             *  검증
             */
            {
                mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
                mat.Polylines(new_contours, true, Scalar.White, 1);
            }

            Draw(max_contour, _canvas);

        }

        public bool IsOverlaped(BrushView dst)
        {
            return Intersection.PointCollectionsOverlap_Fast(
                polygon.Points,
                dst.polygon.Points);
        }
        internal void Merge(BrushView brushView)
        {
            Mat mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
            AreaMerge(polygon.Points, brushView.polygon.Points);
        }

        internal void AreaMerge(PointCollection a, PointCollection b)
        {
            OpenCvSharp.Point[] aset = new OpenCvSharp.Point[a.Count];
            int Count = 0;
            foreach (Point wp in a)
            {
                aset[Count++] = new OpenCvSharp.Point(wp.X, wp.Y);
            }
            Count = 0;
            OpenCvSharp.Point[] Bset = new OpenCvSharp.Point[b.Count];
            foreach (Point wp in b)
            {
                Bset[Count++] = new OpenCvSharp.Point(wp.X, wp.Y);
            }
            Mat mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
            List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]> { aset, Bset };
            Cv2.FillPoly(mat, new_contours, Scalar.White);
            Cv2.ImWrite("LineContoure_union.png", mat); // 아웃라인 출력
            MatOutline(mat);
        }
        internal void AreaMinus(PointCollection a, PointCollection b)
        {
            OpenCvSharp.Point[] aset = new OpenCvSharp.Point[a.Count];
            int Count = 0;
            foreach (Point wp in a)
            {
                aset[Count++] = new OpenCvSharp.Point(wp.X, wp.Y);
            }
            Count = 0;
            OpenCvSharp.Point[] Bset = new OpenCvSharp.Point[b.Count];
            foreach (Point wp in b)
            {
                Bset[Count++] = new OpenCvSharp.Point(wp.X, wp.Y);
            }
            Mat mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
            List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]> { aset };
            Cv2.FillPoly(mat, new_contours, Scalar.White);

            Cv2.ImWrite("LineContoure_union1.png", mat); // 아웃라인 출력
            new_contours = new List<OpenCvSharp.Point[]> { Bset };
            Cv2.FillPoly(mat, new_contours, Scalar.Black);
            Cv2.ImWrite("LineContoure_union2.png", mat); // 아웃라인 출력
            MatOutline(mat);
        }

        /// <summary>
        /// 현재 MAT 이미지에 있는 최대외곽선을 anoFill에 넣는다.
        /// </summary>
        /// <param name="mat"></param>
        internal void MatOutline(Mat mat)
        {
            /*
            * 아웃라인
            */
            var max = 0.0;
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mat, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);

            {
                Mat matnew = new Mat(1200, 1700, MatType.CV_8UC1, 0);
                mat.Polylines(contours, true, Scalar.White);

                Cv2.ImWrite("LineContour.png", mat);
            }

            List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();

            OpenCvSharp.Point[] max_contour = new OpenCvSharp.Point[] { };
            foreach (OpenCvSharp.Point[] p in contours)
            {
                double length = Cv2.ArcLength(p, true);
                if (length > max)
                {
                    max = length;
                    max_contour = p;
                    //  conture = Cv2.ApproxPolyDP(p, length * 0.005, true);
                }
                if (length > 100) // 100
                {
                    new_contours.Add(p);
                }
            }
            /*
             *  검증
             */
            {
                Cv2.ImWrite("LineContoure.png", mat); // 라인출력
                mat = new Mat(1200, 1700, MatType.CV_8UC1, 0);
                mat.Polylines(new_contours, true, Scalar.White);
                Cv2.ImWrite("LineContoure2.png", mat); // 아웃라인 출력
            }
            Draw(max_contour, _canvas);
        }

        public override void Closed()
        {

        }
        

        public override void MovePoints(Vector v, bool isFillMove)
        {
            if (isFillMove)
            {
                Move(v);
            }

        }

        //Convert btw OpenCvSharpPointArray and PointCollection
        //But where should it be defined? Here? or AnoFill.cs? (전상언)
        public OpenCvSharp.Point[] ConvertIntoOpenCvSharpPointArray(PointCollection pointCollection)
        {
            List<OpenCvSharp.Point> openCvSharpPoints = new List<OpenCvSharp.Point>();
            foreach (Point p in pointCollection)
            {
                openCvSharpPoints.Add(new OpenCvSharp.Point(p.X, p.Y));
            }
            return openCvSharpPoints.ToArray();
        }
        public PointCollection ConvertIntoPointCollection(OpenCvSharp.Point[] openCvSharpPoints)
        {
            PointCollection pointCollection = new PointCollection();
            foreach (OpenCvSharp.Point p in openCvSharpPoints)
            {
                pointCollection.Add(new Point(p.X, p.Y));
            }
            return pointCollection;
        }

        public override AnoLabel Clone()
        {
            BrushView nBrushView = new BrushView();

            nBrushView.SetAnoLabelFields(this.Parent, this._canvas, this.classInfo);

            nBrushView.Points = this.Points;

            nBrushView.Draw
                (
                    nBrushView.ConvertIntoOpenCvSharpPointArray(nBrushView.Points),
                    nBrushView._canvas
                );

            return nBrushView;
        }
    }
}




