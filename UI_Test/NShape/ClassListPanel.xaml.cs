using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DataGrid = System.Windows.Controls.DataGrid;
using File = System.IO.File;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace UI_Test.NShape
{
    /// <summary>
    /// ClassListPanel.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class ClassListPanel : UserControl
    {

        //더미 클래스 UcClass의 Constructor에서만 초기화시키고 건들지 말 것!
        // public static ClassInfo classNone = new ClassInfo();
        //classNone의 Visibility를 표기할지 안 할지는 우선 구현하고 나중에 결정!


        private bool isEditting;

        [JsonRepo(Name = "ClassInfoCollection")]
        public ObservableCollection<ClassInfo> ClassInfoCollection { get; set; }
        public static ClassListPanel Inst = null;
        public ObservableCollection<string> ValidClass { get; set; }

        //Constructor
        public ClassListPanel()
        {
            Inst = this;
            InitializeComponent();
            isEditting = false;
            ClassInfoCollection = new ObservableCollection<ClassInfo>();
            ValidClass = new ObservableCollection<string>();
            //   ClassInfoCollection.Add(UcClass.classNone);
            Read();

            if (ClassInfoCollection.Count == 0)
            {
                ClassInfo None = new ClassInfo();
                None.Title = "None";
                None.Visibility = Visibility.Visible;
                None.Index = 0;
                ClassInfoCollection.Add(None);
            }

            DGClass.AutoGenerateColumns = false;
            DGClass.CanUserAddRows = false;
            DGClass.ItemsSource = ClassInfoCollection;

            foreach (var cic in ClassInfoCollection)
            {
                ValidClass.Add(cic.Title);
            }

            bottomButtons.btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            GenYoloFIleAll();
        }

        public event EventHandler ClassListValueChanged;
        

        private void DataGrid_Select_xx(object sender, SelectionChangedEventArgs e)
        {

        }


        private void DataGridClass_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            
            DataGridCellInfo cellInfo = DGClass.CurrentCell;
            DGClass.BeginEdit();
            isEditting = true;
        }

        private void DataGridClass_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null)
            {
                switch (e.Key)
                {
                    case Key.Delete: // 사용중인지를 검사해야 한다.
                        ClassInfo classInfo = DGClass.SelectedItem as ClassInfo;
                        if (classInfo.Title != "None") // None은 지울수 없음.
                        {
                            if (classInfo != null)
                            {
                                var result = MessageBox.Show("Sure Class Delete?", "Delete", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    ClassInfoCollection.Remove(classInfo);
                                }
                            }
                        }
                        e.Handled = true;
                        break;
                    case Key.Enter:
                    case Key.Tab:
                        if (isEditting == true)
                        {
                            isEditting = false;
                            DGClass.CommitEdit();
                        }
                        else
                        {
                            ClassInfoCollection.Add(new ClassInfo()
                            {
                                Index = ClassInfoCollection.Count,
                                Title = "New Class"+"("+ ClassInfoCollection.Count + ")",
                                Visibility = Visibility.Visible
                            });
                            e.Handled = true;
                        }

                        // CommitEdit을 2번 호출해야 DataGrid 변경 사항이 제대로 저장이 됨
                        DGClass.CommitEdit();
                        DGClass.CommitEdit();

                        DGClass.Items.Refresh();
                        ValidClass = getValidClass();
                        if (ClassListValueChanged != null)
                        {
                            ClassListValueChanged(this, e);
                        }

                        break;
                    default:
                        break;
                }
                Write();

            }

            
        }

        private ObservableCollection<string> getValidClass()
        {
            ObservableCollection<string> retVal = new ObservableCollection<string>();
            foreach (var cic in ClassInfoCollection)
            {
                retVal.Add(cic.Title);
            }
            return retVal;
        }


        private void DataGridClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassListValueChanged != null)
            {
                ClassListValueChanged(this, e);
            }
        }


        //JSON File의 Save와 Load에 관련된 함수들====================================================
        private void MenuLoad(object sender, RoutedEventArgs e)
        {
            Read();
        }
        private void MenuSave(object sender, RoutedEventArgs e)
        {
            Write();
        }

        public void Read()
        {
            Debug.WriteLine("UcClass.xaml.cs/Read() Operating");
            string fname = R.projectDir + "ClassInfo.json";

            FileInfo fileInfo = new FileInfo(fname);
            if (!fileInfo.Exists)
            {
                return;
            }
            using (StreamReader reader = new StreamReader(fname))
            {
                JObject jRoot = JObject.Parse(reader.ReadToEnd());
                //Debug.WriteLine(jRoot);
                JArray jarr = (JArray)jRoot["ClassInfoCollection"];
                if (jarr != null)
                {
                    foreach (JObject jobj in jarr)
                    {
                        ClassInfo nClassInfo = new ClassInfo();
                        JsonAnot.Decode(nClassInfo, jobj);
                        ClassInfoCollection.Add(nClassInfo);
                    }
                }

            }
            var ExistNone = false;
            foreach (ClassInfo classInfo in ClassInfoCollection)
            {
                if (classInfo.Title != null && classInfo.Title.Equals("None"))
                {
                    ExistNone = true;
                }
            }
            if (!ExistNone)
            {
                InsertNoneClass();
            }
            Debug.WriteLine("Read ClassInfo FIle");
        }
        private void InsertNoneClass()
        {
            ClassInfo ClassNone = new ClassInfo()
            {
                Title = "None",
                Index = 0,
                Visibility = Visibility.Visible
            };
            ClassInfoCollection.Insert(0, ClassNone);
        }
        public void Write()
        {
            Debug.WriteLine("UcClass.xaml.cs/Write() Operating");
            string fname = R.projectDir + "ClassInfo.json";
            JObject jRoot = new JObject();
            JsonAnot.Encode(this, jRoot);
            Debug.WriteLine("Write ClassInfo FIle");
            File.WriteAllText(fname, jRoot.ToString());

            // JsonAnot.Load3(this, @"C:/Temp/xy.json"); // 추상클래스 때문에 제대로 못읽음.
        }

        //===========================================================================================


        public int Global_Yolo = 0;
        public void ListMarkClear()
        {
            foreach (var ci in ClassInfoCollection)
            {
                ci.YoloIndex = -1;
            }
        }


        public void GenYoloFIleAll()
        {
            Debug.WriteLine("UcClass.xaml.cs/GenYoloFileAll() Operating");
            string path = @"C:\Temp\Yolo\image";
            Global_Yolo = 0;
            ListMarkClear();
            
            //(전상언) 억지로 고침.
            {
                LabelListPanel currentLabelListPanel = LabelListPanel.Inst;
                currentLabelListPanel.model.Write(currentLabelListPanel._MyFile);
            }

            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray(); // 날짜순 정렬
                var Count = 0;
                foreach (var file in files)
                {
                    if (file.Name.Contains(".txt") || file.Name.Contains(".json"))
                    {
                        continue;
                    }
                    ImgFile imgFile = new ImgFile(file, Count++);

                    BitmapImage bimg = new BitmapImage(new Uri(imgFile.FullPath, UriKind.RelativeOrAbsolute));
                    imgFile.Height = bimg.Height;
                    imgFile.Width = bimg.Width;
                    GenYoloFile(imgFile);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to scan {ex.Message}");
            }
        }

        public void GenYoloFile(ImgFile imgFile)
        {
            Debug.WriteLine("ClassListPanel : GenYoloFile() Operating");
            LabelModel model = new LabelModel();
            Debug.WriteLine(imgFile.Name);

            try
            {
                model.Read(imgFile,false);
                
                //Set YoloIndex
                foreach (var label in model)
                {
                    foreach (ClassInfo cic in ClassInfoCollection)
                    {
                        if (cic.Title == label.classInfo.Title)
                        {
                            if (cic.YoloIndex == -1)
                            {
                                cic.YoloIndex = Global_Yolo++;
                            }
                            break;
                        }
                    }
                }

                model.WriteYolo(imgFile);
            }
            catch (Exception ex)
            {
                Debug.Fail("JsonAnno.GenYoloFile::" + ex.ToString());
            }
        }

    }

    public class ClassInfo : INotifyPropertyChanged
    {
        [JsonRepo(Name = "Index")]
        public int Index { get; set; }

        [JsonRepo(Name = "Title")]
        public string Title { get; set; }

        [JsonRepo(Name = "Visibility")]
        public Visibility Visibility { get; set; }

        public bool TempVisibility
        {
            get
            {
                return (Visibility == Visibility.Visible);
            }
            set
            {
                Visibility = (value ? Visibility.Visible : Visibility.Hidden);
                //OnPropertyChanged("TempVisibility");
            }
        }

        public int YoloIndex { get; set; }

        public bool isChanged { get; set; }
        public string prevTitle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string name) {
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if(handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(name));
        //    }
        //}

        //Constructor
        public ClassInfo()
        {
            PropertyChanged += (s, e) =>
            {
                Debug.WriteLine("ClassInfo : Property " + e.PropertyName + " has been changed.");
            };
        }
    }
}
