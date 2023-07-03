using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

using ComboBox = System.Windows.Controls.ComboBox;
using DataGrid = System.Windows.Controls.DataGrid;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Windows.Documents;

namespace UI_Test.NShape
{
    /// <summary>
    /// LabelListPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LabelListPanel : UserControl
    {
        public static LabelListPanel Inst;

        private ObservableCollection<ClassInfo> ClassInfoCollection = null;
        private ObservableCollection<string> ValidClass = null;

        [JsonRepo(Name = "ListLabel")]
        public ObservableCollection<AnoLabel> LabelCollection
        {
            get { return model; }
            set
            {
                DGLabel.ItemsSource = value;

                //LabelCollection의 멤버 추가 및 삭제
                value.CollectionChanged += OnCollectionChanged;

                //LabelCollection의 각 멤버의 성질 변화
                foreach (var item in value)
                {
                    AnoLabel label = item;
                    item.isClosed = true;//(전상언) true여야 하는 것 아님?
                    item.PropertyChanged += (s, e) =>
                    {
                        switch (e.PropertyName)
                        {
                            case "Click":
                                Node_Select(item);
                                break;
                            case "LabelIndex":
                                //debug
                                {
                                    Debug.WriteLine("LabelIndex Changed");
                                }
                                break;
                        }
                    };
                }

            }
        }

        //  private ImgFile OldFile = null;
        public ImgFile _MyFile;
        public LabelModel model = new LabelModel();
        public ImgFile MyFile
        {
            get
            {
                return _MyFile;
            }
            set
            {
                if (_MyFile != null)
                {
                    model.Write(_MyFile);
                    model.WriteYolo(_MyFile);
                }
                model = new LabelModel();
                model.Read(value);
                LabelCollection = model;
                this._MyFile = value;
            }
        }

        public LabelListPanel()
        {
            InitializeComponent();
            LabelListPanel.Inst = this;
            DGLabel.ItemsSource = LabelCollection;

            bottomButtons.btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            model.Write(MyFile);
            WriteYoloForThisFile();
        }

        public void setClassList(ObservableCollection<ClassInfo> classCollection, ObservableCollection<string> vClass)
        {
            ClassInfoCollection = classCollection;
            ValidClass = vClass;
            ComboBoxColumn.ItemsSource = ValidClass;
        }


        public AnoLabel SelectedItem = null;

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<AnoLabel> obsSender = sender as ObservableCollection<AnoLabel>;
            List<AnoLabel> editedOrRemovedItems = new List<AnoLabel>();
            if (e.NewItems != null)
            {
                foreach (AnoLabel newItem in e.NewItems)
                {
                    editedOrRemovedItems.Add(newItem);
                    if (SelectedItem != null)
                    {
                        SelectedItem.Selected = false;
                    }
                    SelectedItem = newItem;
                    SelectedItem.Selected = true;
                }
            }

            if (e.OldItems != null)
            {
                foreach (AnoLabel oldItem in e.OldItems)
                {
                    editedOrRemovedItems.Add(oldItem);
                    oldItem.Selected = false;
                }
            }
        }

        /// <summary>
        /// 테이블에서 선택되었을때 호출된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_Select(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Grd_Select ...");
            DataGrid dg = sender as DataGrid;
            AnoFill? myLabel = dg.SelectedItem as AnoFill;
            if (myLabel != null)
            {
                myLabel.Selected = true;
            }
        }


        /// <summary>
        /// 노드를 선택했을때 호출된다.
        /// </summary>
        /// <param name="node"></param>
        public void Node_Select(AnoLabel node)
        {
            Debug.WriteLine("Node_Select ...");
            foreach (AnoLabel item in model)
            {
                if (item == node)
                {
                    //   item.Selected = true;
                }
                else
                {
                    //  item.Selected = false;
                }
            }
            DGLabel.SelectedItem = node;
        }

        private AnoLabel Clipped = null;

        //Label tab에서 선택했을 때
        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var dg = sender as DataGrid;
                AnoLabel? selectedItem = dg.SelectedItem as AnoLabel;
                if (selectedItem != null)
                {
                    OnKeydownDel(selectedItem);
                }
            }
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var dg = sender as DataGrid;
                if (dg != null)
                {
                    AnoLabel? selectedItem= dg.SelectedItem as AnoLabel;
                    if(selectedItem != null)
                    {
                        OnKeydownCtrlC(selectedItem);
                    }
                    e.Handled = true;
                }
            }
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var dg = sender as DataGrid;
                OnKeydownCtrlV();
                e.Handled = true;
            }
        }

        //Functions for Copy, Paste, Delete of AnoLabel
        //Connected to the Label Views
        internal void OnKeydownCtrlC(AnoLabel anoLabel)
        {
            Clipped = anoLabel;
        }
        internal void OnKeydownCtrlV()
        {
            if (Clipped != null)
            {
                AnoLabel copyedAnoLabel = Clipped.Clone();
                copyedAnoLabel.MovePoints(new Vector(10, 10), true);
                model.Add(copyedAnoLabel);
            }
        }
        internal void OnKeydownDel(AnoLabel anoLabel)
        {
            var result = MessageBox.Show("Sure Label Delete?", "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Debug.WriteLine(" DG-YES2");
                model.Remove(anoLabel);
                anoLabel.Dispose();
                Debug.WriteLine(" DG-YES " + anoLabel);
            }
            else
            {
                Debug.WriteLine(" DG-NO");
            }
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("UcLabel.xaml.cs/OnComboBoxSelectionChanged Operating");
            AnoLabel myLabel = DGLabel.SelectedItem as AnoLabel;
            //(전상언) SelectedItem과 SelectionChanged event가 생긴 AnoLabel이 달라질 수도 있으므로 유의할 것!
            string selectedClassName = ((ComboBox)(e.Source)).SelectedItem.ToString();
            Debug.WriteLine("selectedClassName: " + selectedClassName);

            foreach (ClassInfo cic in ClassInfoCollection) // Title이 같은 클래스정의를 연결
            {
                if (cic.Title == selectedClassName)
                {
                    myLabel.classInfo = cic;
                    return;
                }
            }
            Debug.Fail("Something Wrong!");
        }

        //public void ChangeImage(ImgFile mfile)
        //{
        //    model.Write(OldFile);
        //    model.Read(mfile);
        //}

        //JSON File의 Save와 Load에 관련된 함수들====================================================
        private void MenuLoad(object sender, RoutedEventArgs e)
        {
            model.Read(MyFile);
            LabelCollection = model;
        }
        private void MenuSave(object sender, RoutedEventArgs e)
        {
            model.Write(MyFile);
        }


        //Yolo File의 Save와 Load에 관련된 함수들====================================================
        private void MenuSaveYoLo(object sender, RoutedEventArgs e)
        {
            ClassListPanel classListPanel = ClassListPanel.Inst;
            classListPanel.GenYoloFIleAll();
        }

        private void MenuClean(object sender, RoutedEventArgs e)
        {
            foreach (AnoLabel label in model)
            {
                label.Dispose();
            }
            model.Clear();
            LabelCollection = model;
        }

        private void WriteYoloForThisFile()
        {
            ClassListPanel classListPanel = ClassListPanel.Inst;
            classListPanel.GenYoloFile(MyFile);
        }

        private void WriteYoloForAllFiles()
        {

            ClassListPanel classListPanel = ClassListPanel.Inst;
            classListPanel.GenYoloFIleAll();
        }

    }
}
