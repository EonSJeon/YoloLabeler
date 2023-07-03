using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

/*
 * 
 * 박스, 원, 확대/축소, 그림대조. : 2일
 * 클래스 관리, : 2일
 * 
 * 
 */
namespace UI_Test.NShape
{

    public partial class ShapeEditor : UserControl
    {
        public static ShapeEditor Inst = null;
        internal Point sp, mp;
        private AnoLabel anoLabel;
        private List<BrushView> BrushList = new List<BrushView>();
        public ObservableCollection<ClassInfo> classes = new ObservableCollection<ClassInfo>();
        Scaler scaler = null;
        private Button prevButton;
        Modes mode = Modes.BoxView;

        public ClassListPanel _panelClass;
        public ColorListPanel _panelColor;
        public ImageListPanel _panelImage;
        public LabelListPanel _panelLabel;
        public PropertyPanel _panelProperty;


        private enum Modes
        {
            BoxView,
            CurveView,
            BrushView
        };

        private enum MouseMode
        {
            NONE,
            DRAG,
            PANNING
        }
        MouseMode mouseMode = MouseMode.NONE;

        

        //Constructor
        public ShapeEditor()
        {

            Inst = this;
            InitializeComponent();
            //UcClass(), UcImage()에 파일로부터 읽어오는 것 포함되어 있음
            //UcClass.[Model].Read() & UcImage.[Model].Scan()
            UiCache.ScanAll(this);

            panelLabel.LabelCollection = GetLabelList();
            panelClass.ClassListValueChanged += new EventHandler(notifyClassListChange);
            panelLabel.setClassList(panelClass.ClassInfoCollection, panelClass.ValidClass);


            scaler = new Scaler(UCanvas, UBorder);
            anoLabel = null;

            if (R.projectType == ProjectType.Detection)
            {
                mode = Modes.BoxView;
            }
            else
            {
                mode = Modes.CurveView;
            }

            Canvas _canvas = UCanvas;

            _canvas.MouseDown += (s, e) =>
            {
                e.Handled = true;
                sp = mp = e.GetPosition(_canvas);
                /*
                 * 오른쪽 마우스 버튼을 누르면 Panning
                 */
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        {
                            ClassInfo classInfo = null; //  UcClass.classNone;
                            if (panelClass.DGClass.SelectedItem != null)
                            {
                                classInfo = panelClass.DGClass.SelectedItem as ClassInfo;
                            }
                            else
                            {
                                classInfo = panelClass.DGClass.Items[0] as ClassInfo;
                            }
                            
                            if(classInfo.Visibility == Visibility.Hidden)
                            {
                                return;
                            }

                            mouseMode = MouseMode.DRAG;
                            Debug.WriteLine(mode);
                            Debug.WriteLine("Mouse Down : mode={0} node={1} ", mode, anoLabel);
                            switch (mode)
                            {
                                case Modes.CurveView:
                                    {
                                        var labels = GetLabelList();
                                        if (labels != null)
                                        {
                                            if (anoLabel != null && (anoLabel.GetType() == typeof(CurveView) && anoLabel.isClosed))
                                            {
                                                // JJY 그리다가 오류난 경우에 대책필요.
                                                Debug.WriteLine("New PolyPath Generated");

                                                DPoint dp = ((CurveView)anoLabel).FirstPoint;
                                                while (true)
                                                {

                                                    Debug.WriteLine(dp.X + ", " + dp.Y);
                                                    dp = dp.NextPoint;
                                                    if (dp == ((CurveView)anoLabel).FirstPoint)
                                                    {
                                                        break;
                                                    }
                                                }

                                                anoLabel = null;
                                            }
                                            if (anoLabel == null || anoLabel.GetType() != typeof(CurveView))
                                            {
                                                anoLabel = new CurveView();
                                                
                                                anoLabel.LabelIndex = labels.Count;
                                                labels.Add(anoLabel);
                                                anoLabel.Parent = labels;
                                            }
                                        }
                                        break;
                                    }
                                case Modes.BrushView:
                                    {
                                        anoLabel = new BrushView();
                                        Debug.WriteLine("New Brush Generated");
                                        break;
                                    }
                                case Modes.BoxView:
                                    {
                                        anoLabel = new BoxView();
                                        Debug.WriteLine("New Box Generated");
                                        break;
                                    }
                            }
                            anoLabel.classInfo= classInfo;
                            anoLabel.First(sp, _canvas);

                            Mouse.Capture(_canvas);
                            break;
                        }
                        
                    case MouseButton.Right:
                        {
                            mouseMode = MouseMode.PANNING;
                            break;
                        }
                }
            };

            _canvas.MouseUp += (s, e) =>
            {
                _canvas.ReleaseMouseCapture();
              
                switch (mouseMode)
                {
                    case MouseMode.DRAG:
                        var labels = GetLabelList();
                        if (labels != null)
                        {
                            if (anoLabel.Ending(sp))//AnoLabel 생성
                            {

                                anoLabel.LabelIndex = labels.Count;
                                anoLabel.Parent = labels;
                                labels.Add(anoLabel); // 무조건 add 하면 안됨.
                                //addLabelInstance(anoLabel);
                                Debug.WriteLine("ShapeEditor - label added");
                            }
                            else
                            {
                                if (anoLabel.GetType() == typeof(BoxView))
                                {
                                    labels.Remove(anoLabel);
                                    anoLabel.Dispose();
                                    //(전상언) Dispose과정이 BoxView 내부와 ShapeEditor로 쪼개져 있어서 헷갈림
                                }
                            }

                            if (anoLabel.GetType() == typeof(BrushView))
                            {
                                if (BrushList.Count > 0)
                                {
                                    BrushView t1 = (BrushView)anoLabel;
                                    foreach (BrushView t in BrushList)
                                    {
                                        if (t1.IsOverlaped(t))
                                        {
                                            Debug.WriteLine("Overlap ************** ");
                                            t.Merge(t1);
                                            labels.Remove(t1);
                                            t1.Dispose();
                                            return;
                                        }
                                    }
                                }
                                Debug.WriteLine(" Not Overlaped ");
                                BrushList.Add((BrushView)anoLabel);
                                anoLabel.SetAnoLabelFields(labels, _canvas, null);
                                //(전상언) class와 관련하여 Merge를 어떻게 할지 아직 정하지 않았음 
                                // 교집합이 있으면 기존 것이랑 합친다.
                            }
                        }
                        mouseMode = MouseMode.NONE;
                     
                        break;
                    case MouseMode.PANNING:
                        break;
                }

                mouseMode = MouseMode.NONE;
                e.Handled = true;
            };

            _canvas.MouseMove += (s, e) =>
            {
                mp = e.GetPosition(_canvas);
                Vector delta = new Vector(sp.X - mp.X, sp.Y - mp.Y);
                switch (mouseMode)
                {
                    case MouseMode.DRAG:
                        anoLabel.Next(mp, delta);
                        break;
                    case MouseMode.PANNING:
                        scaler.PanningMove(delta);
                        break;
                }
                sp = e.GetPosition(_canvas);  //must for panning
                e.Handled = true;
            };
            prevButton = btnModeMove;
        }

        protected void notifyClassListChange(object sender, EventArgs e)
        {
            Debug.WriteLine("ShapeEditor : class list changed, notify");
            panelLabel.setClassList(panelClass.ClassInfoCollection, panelClass.ValidClass);
        }

        private ObservableCollection<AnoLabel> GetLabelList()
        {
            if (panelLabel.model != null )
            {
                return panelLabel.model;    
            }
            return null;
        }


        //Event functions related to the drawing mode select 
        private void mode_boxView_click(object sender, RoutedEventArgs e)
        {
            if (mode == Modes.BoxView)
            {
                return;
            }
            mode = Modes.BoxView;
            btnModeBox.Background = Brushes.SkyBlue;
            prevButton.Background = Brushes.White;
            prevButton = btnModeBox;
            Debug.WriteLine("Mode Changed to BBox");
            //  Debug.WriteLine("Mode Changed : " + alabel.GetType().Name);

        }
        private void mode_curveView_click(object sender, RoutedEventArgs e)
        {
            if (mode == Modes.CurveView)
            {
                return;
            }
            mode = Modes.CurveView;
            btnModePolyPath.Background = Brushes.SkyBlue;
            prevButton.Background = Brushes.White;
            prevButton = btnModePolyPath;
            Debug.WriteLine("Mode Changed to PolyPath");
            //  Debug.WriteLine("Mode Changed : " + alabel.GetType().Name);
        }
        private void mode_brushView_click(object sender, RoutedEventArgs e)
        {
            if (mode == Modes.BrushView)
            {
                return;
            }
            mode = Modes.BrushView;
            btnModeBrush.Background = Brushes.SkyBlue;
            prevButton.Background = Brushes.White;
            prevButton = btnModeBrush;
            Debug.WriteLine("Mode Changed to Brush");
            //Debug.WriteLine("Mode Changed : " + alabel.GetType().Name);
        }


        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BoxModelList list = (BoxModelList)sender;
            if (e.NewItems != null)
            {
                foreach (BoxModel model in e.NewItems)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            //  BoxView view = new BoxView(UCanvas, model, list);
                            //model.view.Initialized(cnavas)
                            break;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (BoxModel model in e.OldItems)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Remove:
                            model.view.Dispose();
                            break;
                    }

                }
            }
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine("ShapeEditor : onpropertychanged, {0}", e.ToString());
        }
    }

}
