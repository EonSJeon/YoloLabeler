using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UI_Test;
using UI_Test.NShape;

namespace UI_Test.NShape
{
    public abstract class AnoLabel : AnoFill
    {
        [JsonRepo(Name = "@Class")] //Program 내의 Type
        public string _Class
        {
            get => this.GetType().Name;
            set
            {
                return;
            }
        }

        [JsonRepo(Name = "LabelClass")] // Class Name
        public string LabelClass
        {
            get => _LabelClass = classInfo.Title;
            set => _LabelClass = value; // 저장후 복원시 index값이됨.
        }
        private string _LabelClass;

        [JsonRepo(Name = "LabelClassIndex")] // Class의 index
        public int LabelClassIndex
        {
            get => classInfo.Index;
            set
            {
                if (classInfo != null)
                {
                    classInfo.Index = value;
                }
            }
        }

        [JsonRepo(Name = "LabelIndex")]  //  Label의 index
        public int LabelIndex
        {
            get => index;
            set => index = value;
        }
        private int index;

        public ObservableCollection<AnoLabel> Parent;

        private ClassInfo _classInfo = null;
        public ClassInfo classInfo
        {
            get
            {
                return _classInfo;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                else
                {
                    if (_classInfo != null) // old value clear
                    {
                        _classInfo.PropertyChanged -= ClassPropertyChanged;
                    }
                    _classInfo = value;
                    _classInfo.PropertyChanged += ClassPropertyChanged;
                    Visibility = _classInfo.Visibility;
                    Title.Text = _classInfo.Title;
                }
            }
        }
        private void ClassPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ClassInfo _myclassinfo = sender as ClassInfo;
            switch (e.PropertyName)
            {
                case "Visibility":
                    this.Visibility = _myclassinfo.Visibility;
                    break;
                case "Title":
                    Debug.WriteLine("Label : RX Class Name Changed");
                    this.Title.Text = _myclassinfo.Title;
                    LabelClass = _myclassinfo.Title; // important
                    break;
            }
        }
        internal bool isClosed = false;

        //Constructor
        public AnoLabel()
        {
            AreaMoved += (s, e) =>
               {
                   MovePoints(e, false);
               };
            MouseClicked += (s, e) =>
               {
                   LabelListPanel currentLabelListPanel = LabelListPanel.Inst;
                   currentLabelListPanel.DGLabel.SelectedItem = this;
               };
            CtrlCKeyDown += (s, e) =>
                {
                    LabelListPanel currentLabelListPanel = LabelListPanel.Inst;
                    currentLabelListPanel.OnKeydownCtrlC(this);
                };
            CtrlVKeyDown += (s, e) =>
                {
                    LabelListPanel currentLabelListPanel = LabelListPanel.Inst;
                    currentLabelListPanel.OnKeydownCtrlV();
                };
            DelKeyDown += (s, e) =>
                {
                    LabelListPanel currentLabelListPanel = LabelListPanel.Inst;
                    currentLabelListPanel.OnKeydownDel(this);
                };

            classInfo = null;
        }

        //Animation Event Functions
        public abstract void First(Point p, Canvas canvas);
        public abstract void Next(Point p, Vector delta);
        public abstract bool Ending(Point p);

        public abstract void Closed();


        public abstract AnoLabel Clone();
        public abstract void MovePoints(Vector v, bool isFillMove);


        public double Distance(Point pointA, Point pointB)
        {
            return Point.Subtract(pointA, pointB).Length;
        }

        public void SetAnoLabelFields
            (ObservableCollection<AnoLabel> Parent, Canvas _canvas, ClassInfo classInfo)
        {
            this.Parent = Parent;
            this._canvas = _canvas;
            this.classInfo = classInfo;
        }

        
    }
}
