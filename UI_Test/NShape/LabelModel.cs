using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Point = System.Windows.Point;

namespace UI_Test.NShape
{

    public class LabelModel : ObservableCollection<AnoLabel>
    {
        public LabelListPanel Parent = null;

        double TH;
        double TW;

        public void Add(AnoLabel item)
        {
            base.Add(item);
            Reordering();
        }

        public void Remove(AnoLabel item)
        {
            base.Remove(item);
            Reordering();
        }

        private void Reordering()
        {
            int i = 0;
            foreach (AnoLabel label in this)
            {
                if (label != null)
                {
                    label.LabelIndex = i++;
                }
            }
        }

        public void Read(ImgFile mfile, bool drawFlag=true)
        {
            Debug.WriteLine("LabelModel.cs/Read(ImgFile mfile) Operating");
            TW = mfile.Width;
            TH = mfile.Height;
            string fname = R.projectDir + mfile.Name + ".json";
            Debug.WriteLine("Read.FIle " + mfile);
            Canvas _canvas = (Canvas)UiCache.Find("UCanvas");
            if (_canvas == null)
            {
                Debug.Fail("FindElement.Fail UiCanvas");
                return;
            }

            if ( mfile.img != null )
            {
                _canvas.Children.Clear();
                _canvas.Children.Add(mfile.img);
            }
            
          
            FileInfo fileInfo = new FileInfo(fname);
            if (!fileInfo.Exists)
            {
                return;
            }

            using (StreamReader reader = new StreamReader(fname))
            {
                JObject jRoot = JObject.Parse(reader.ReadToEnd());
                Debug.WriteLine("Load " + fname + " " + jRoot.ToString());

                JArray jarr = (JArray)jRoot["ListLabel"];
                if (jarr != null)
                {
                    //  _listLabel = new ObservableCollection<AnoLabel>();
                    foreach (JObject jobj in jarr)
                    {
                        string _ClassName = (string)(jobj["@Class"]);
                        int _labelClassIndex = (int)(jobj["LabelClassIndex"]);
                        if (_ClassName == null)
                        {
                            Debug.WriteLine("[Error] @Class 항목이 없음");
                        }

                        dynamic Target;

                        switch (_ClassName)
                        {
                            case "BoxView":
                                Target = new BoxView();
                                break;
                                
                            case "CurveView":
                                Target = new CurveView();
                                break;
                            case "BrushView":
                                Target = new BrushView();
                                break;

                            default:
                                Target = new BoxView();
                                break;
                        }

                        Target.classInfo = FindClassInfo(_labelClassIndex);//나중에 JsonAnot.Decode에 포함할 것(전상언)
                        JsonAnot.Decode(Target, jobj);
                        
                        if (drawFlag)
                        {
                            Target.Build(_canvas);
                        }
                        Add(Target);
                        Target.Parent = this;

                    }
                }
            }
        }

        
        private ClassInfo FindClassInfo(int classindex)
        {
            ClassListPanel classListPanel = ClassListPanel.Inst;
            // return classListPanel.ClassInfoCollection.ToList().First(x=>x.Index==classindex);
            foreach (ClassInfo classInfo in classListPanel.ClassInfoCollection)
            {
                Debug.WriteLine(classInfo.Title);
                if (classInfo.Index == classindex)
                {
                    return classInfo;
                }
            }

            //ClassListPanel에서 못 찾았을 때
            ClassInfo UnClassified = new ClassInfo();
            UnClassified.Index = classindex;
            UnClassified.Title = "UnClassified";
            classListPanel.ClassInfoCollection.Add(UnClassified);
            return UnClassified;
            
            //Debug.Fail("LabelModel.cs/ FindClassInfo(int classindex) : Something Wrong!");
            //return null;
        }

        public void Write(ImgFile mfile)
        {
            string fname = R.projectDir + mfile.Name + ".json";
            JObject jRoot = new JObject();
            JsonAnot.Encode(LabelListPanel.Inst, jRoot);
            Debug.WriteLine("Write.FIle " + mfile);
            File.WriteAllText(fname, jRoot.ToString());

            // JsonAnot.Load3(this, @"C:/Temp/xy.json"); // 추상클래스 때문에 제대로 못읽음.
        }
        //===========================================================================================

        

        public void WriteYolo(ImgFile file)
        {
            string labelPath = R.projectDir + file.Name + ".txt";
            double TW = file.Width;
            double TH = file.Height;
            using (StreamWriter writer = File.CreateText(labelPath))
            {
                foreach (AnoLabel anoLabel in this)
                {
                    Debug.WriteLine(anoLabel.Name, "LabelModel : writeYolo, anoLabel.Name");
                    //ClassName
                    int cl = anoLabel.classInfo.YoloIndex;
                    string geoInfo="";//Geometric Infomation
                    Debug.WriteLine(R.projectType);
                    //Coordinates
                    switch (R.projectType)
                    {
                        case ProjectType.Detection:
                            if (anoLabel is BoxView)
                            {
                                BoxView boxView = anoLabel as BoxView;
                                var cx = (boxView.X + boxView.Width / 2) / TW;
                                var cy = (boxView.Y + boxView.Height / 2) / TH;
                                var w = boxView.Width / TW;
                                var h = boxView.Height / TH;
                                geoInfo=String.Format(" {0:F6} {1:F6} {2:F6} {3:F6}", cx, cy, w, h);
                            }
                            writer.WriteLine("{0}{1}", cl, geoInfo);
                            break;

                        case ProjectType.Segmentation:
                            if(anoLabel is CurveView)
                            {
                                Debug.WriteLine(cl);
                                writer.WriteLine(cl);
                                foreach (Point point in anoLabel.polygon.Points)
                                {
                                    Debug.WriteLine(point, "LabelModel - anoLabel.polygon.Points");
                                    double x = point.X / TW;
                                    double y = point.Y / TH;
                                    geoInfo = String.Format(" {0:F6} {1:F6}", x, y);
                                    writer.WriteLine(geoInfo);
                                }
                            }
                            break;
                        case ProjectType.KeyPoint:
                            break;
                    }
                    
                };
            }
        }
    }
}

//private void WriteYoloBrushView(ImgFile file)
//{
//    string labelPath = UcImage.PjtDir + "/seg/BrushView/" + file.Name + ".txt";
//    double TW = file.Width;
//    double TH = file.Height;
//    Debug.WriteLine("WriteYolo File = " + labelPath);
//    using (StreamWriter writer = File.CreateText(labelPath))
//    {
//        foreach (AnoLabel anoLabel in _listLabel)
//        {
//            if (!(anoLabel is BrushView))
//            {
//                continue;
//            }
//            BrushView brushView = anoLabel as BrushView;

//            //Class
//            int cl = brushView.LabelClassIndex - 1;

//            //Coordinates
//            string coordinateString = "";


//            foreach (Point point in brushView.polygon.Points)
//            {
//                double x = point.X / TW;
//                double y = point.Y / TH;
//                coordinateString += String.Format(" {0:F6} {1:F6}", x, y);
//            }
//            writer.WriteLine("{0}{1}", cl, coordinateString);

//        }
//    }
//}

//private void WriteYoloCurveView(ImgFile file)
//{
//    string labelPath = UcImage.PjtDir + "/seg/CurveView/" + file.Name + ".txt";
//    double TW = file.Width;
//    double TH = file.Height;
//    Debug.WriteLine("WriteYolo File = " + labelPath);

//    using (StreamWriter writer = File.CreateText(labelPath))
//    {
//        foreach (AnoLabel anoLabel in _listLabel)
//        {
//            if (!(anoLabel is CurveView))
//            {
//                continue;
//            }

//            CurveView curveView = anoLabel as CurveView;


//            //Class
//            int cl = anoLabel.LabelClassIndex - 1;

//            //Coordinates
//            string coordinateString = "";
//            DPoint firstDPoint = curveView.FirstPoint;
//            DPoint dPoint = firstDPoint;

//            do
//            {
//                double x = dPoint.X / TW;
//                double y = dPoint.Y / TH;
//                Debug.WriteLine("From WriteYoloSegmentation");
//                Debug.WriteLine("{0} {1}", x, y);
//                coordinateString += string.Format(" {0:F6} {1:F6}", x, y);
//                if (dPoint.NextPoint == null)
//                {
//                    return;
//                }
//                dPoint = dPoint.NextPoint;
//            } while (dPoint != firstDPoint);
//            writer.WriteLine("{0}{1}", cl, coordinateString);
//        }
//    }
//}

//private string CoordinatesString_For_Polygon(Polygon polygon, double TW, double TH)
//{
//    string coordinateString = "";
//    foreach (Point point in polygon.Points)
//    {
//        double x = point.X / TW;
//        double y = point.Y / TH;
//        coordinateString += String.Format(" {0:F6} {1:F6}", x, y);
//    }
//    return coordinateString;
//}

//public void ReadYolo(ImgFile file)
//{
//    Canvas canvas = (Canvas)UiCache.Find("UCanvas");
//    if (canvas == null)
//    {
//        return;
//    }

//    {
//        _listLabel.Clear();
//        canvas.Children.Clear();
//        canvas.Children.Add(file.img);
//    }
//    ReadYoloBoxView(file, canvas);
//    ReadYoloCurveView(file, canvas);
//    ReadYoloBrushView(file, canvas);
//}
//private void ReadYoloBoxView(ImgFile file, Canvas canvas)
//{
//    //string labelPath = UcImage.PjtDir + "/det/BoxView/" + file.Name + ".txt";
//    //if (!File.Exists(labelPath))
//    //{
//    //    return;
//    //}

//    //double TW = file.Width;
//    //double TH = file.Height;

//    //string[] lines = File.ReadAllLines(labelPath);
//    string FullPath = UcImage.PjtDir + "/det/BoxView/" + file.Name + ".txt";
//    string[] lines = ReadFileContext(file, FullPath);
//    if (lines == null)
//    {
//        return;
//    }
//    foreach (string line in lines)
//    {
//        if (line != null)
//        {
//            string[] words = line.Split(' ');

//            if ((words.Length - 1) != 4)
//            {
//                Debug.WriteLine("데이터 오류 @ " + FullPath + " : 숫자가 4개가 아님");
//            }
//            switch (ModelType)
//            {
//                case ModelTypes.Detection:
//                    break;
//            }

//            int cl = int.Parse(words[0]) + 1;
//            double cx = Double.Parse(words[1]) * TW;
//            double cy = Double.Parse(words[2]) * TH;
//            double w = Double.Parse(words[3]) * TW;
//            double h = Double.Parse(words[4]) * TH;

//            var Box = new BoxView();
//            Box.SetAnoLabelFields(_listLabel, canvas);

//            Box.X = cx - (w / 2);
//            Box.Y = cy - (h / 2);
//            Box.Width = w;
//            Box.Height = h;

//            Box.Build(canvas);
//            Box.classInfo = FindClassInfoWithClassIndex(cl);
//            _listLabel.Add(Box);
//        }
//    }
//    parent.listLabel = _listLabel;
//}
//private void ReadYoloCurveView(ImgFile file, Canvas canvas)
//{
//    //string labelPath = UcImage.PjtDir + "/seg/CurveView/" + file.Name + ".txt";
//    //if (!File.Exists(labelPath))
//    //{
//    //    return;
//    //}

//    //double TW = file.Width;
//    //double TH = file.Height;

//    //string[] lines = File.ReadAllLines(labelPath);
//    string[] lines = ReadFileContext(file, UcImage.PjtDir + "/seg/CurveView/" + file.Name + ".txt");
//    if (lines == null)
//    {
//        return;
//    }
//    foreach (string line in lines)
//    {
//        if (line != null)
//        {
//            string[] words = line.Split(' ');

//            if ((words.Length - 1) % 2 == 1)
//            {
//                Debug.WriteLine("데이터 오류 @ " + labelPath + " : 숫자가 홀수 개라 짝이 안 맞음");
//            }

//            int cl = int.Parse(words[0]) + 1;
//            List<Point> CurvePoints = new List<Point>();

//            for (int i = 1; i < words.Length; i += 2)
//            {
//                double x = Double.Parse(words[i]) * TW;
//                double y = Double.Parse(words[i + 1]) * TH;
//                Point tempPoint = new Point(x, y);
//                CurvePoints.Add(new Point(x, y));
//            }
//            CurveView Curve = new CurveView();
//            Curve.SetAnoLabelFields(_listLabel, canvas);

//            foreach (Point point in CurvePoints)//Should be modified
//            {
//                Curve.First(point, canvas);
//            }
//            Curve.Closed();

//            Curve.classInfo = FindClassInfoWithClassIndex(cl);
//            _listLabel.Add(Curve);
//        }
//    }
//    parent.listLabel = _listLabel;
//}
//private void ReadYoloBrushView(ImgFile file, Canvas canvas)
//{
//    string labelPath = UcImage.PjtDir + "/seg/BrushView/" + file.Name + ".txt";
//    if (!File.Exists(labelPath))
//    {
//        return;
//    }

//    double TW = file.Width;
//    double TH = file.Height;

//    string[] lines = File.ReadAllLines(labelPath);
//    foreach (string line in lines)
//    {
//        if (line != null)
//        {
//            string[] words = line.Split(' ');

//            if ((words.Length - 1) % 2 == 1)
//            {
//                Debug.WriteLine("데이터 오류 @ " + labelPath + " : 숫자가 홀수 개라 짝이 안 맞음");
//            }

//            BrushView Brush = new BrushView();
//            Brush.SetAnoLabelFields(_listLabel, canvas);

//            int cl = int.Parse(words[0]) + 1;

//            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();
//            for (int i = 1; i < words.Length; i += 2)
//            {
//                double x = Double.Parse(words[i]) * TW;
//                double y = Double.Parse(words[i + 1]) * TH;
//                points.Add(new OpenCvSharp.Point(x, y));
//            }

//            Brush.Draw(points.ToArray(), canvas);
//            Brush.classInfo = FindClassInfoWithClassIndex(cl);
//            parent.listLabel.Add(Brush);
//        }
//    }
//    parent.listLabel = _listLabel;
//}