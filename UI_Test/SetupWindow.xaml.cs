using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using UI_Test.NShape;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace UI_Test
{
    /// <summary>
    /// SetupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public class ProjectInfo
    {
        [JsonRepo(Name = "Date")]
        public string Date { get; set; }

        [JsonRepo(Name = "ProjectName")]
        public string ProjectName { get; set; }

        [JsonRepo(Name = "Directory")]
        public string Directory { get; set; }

        [JsonRepo(Name = "ImageSourceDirectory")]
        public string ImageSourceDirectory { get; set; }

        [JsonRepo(Name = "ProjectType")]
        public int ProjectType { get; set; }

        [JsonRepo(Name = "ExportType")]
        public int ExportType { get; set; }

        //Constructor
        public ProjectInfo()
        {

        }
    }

    public partial class SetupWindow : Window
    {
        [JsonRepo(Name = "ProjectInfoCollection")]
        public ObservableCollection<ProjectInfo> ProjectInfoCollection { get; set; }


        public SetupWindow()
        {
            InitializeComponent();
            Console.WriteLine("SetupWindow");
            ProjectInfoCollection = new ObservableCollection<ProjectInfo>();
            DGProject.ItemsSource = ProjectInfoCollection;
            DGProject.IsReadOnly = true;
            Read();

            textProjectName.Text = R.projectName;
            textProjectDir.Text = R.projectDir;
            textImageSourceDir.Text = R.imageSourceDir;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            R.projectName = textProjectName.Text;
            R.projectDir = textProjectDir.Text + "\\";
            R.imageSourceDir = textImageSourceDir.Text + "\\";

            ProjectInfo project = new ProjectInfo();
            project.Date = DateTime.Now.ToString("yyyy/MM/dd");
            project.ProjectName = R.projectName;
            project.Directory = R.projectDir;
            project.ImageSourceDirectory = R.imageSourceDir;
            project.ProjectType = (int)R.projectType;
            project.ExportType = (int)R.exportType;

            ProjectInfoCollection.Add(project);
            Write();
            this.Close();
        }

        private void ProjectDir_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog()
            {
                InitialDirectory = "",
                IsFolderPicker = true
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textProjectDir.Text = folderDialog.FileName;
            }
        }

        private void ImageSourceDir_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog()
            {
                InitialDirectory = "",
                IsFolderPicker = true
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textImageSourceDir.Text = folderDialog.FileName;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void DataGridProject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProjectInfo selected = (ProjectInfo)DGProject.SelectedItem;
            if (selected != null)
            {
                R.projectName = selected.ProjectName;
                R.projectDir = selected.Directory;
                R.imageSourceDir = selected.ImageSourceDirectory;
                R.projectType = (ProjectType)selected.ProjectType;
                R.exportType = (ExportType)selected.ExportType;


                this.Close();
            }
        }


        private void Read()
        {
            Debug.WriteLine("SetupWindow : Read() Operating");
            string fname = R.baseDir + "ProjectInfo.json";

            FileInfo fileInfo = new FileInfo(fname);
            if (!fileInfo.Exists)
            {
                return;
            }
            using (StreamReader reader = new StreamReader(fname))
            {
                JObject jRoot = JObject.Parse(reader.ReadToEnd());
                //Debug.WriteLine(jRoot);
                JArray jarr = (JArray)jRoot["ProjectInfoCollection"];
                if (jarr != null)
                {
                    foreach (JObject jobj in jarr)
                    {
                        ProjectInfo nProjectInfo = new ProjectInfo();
                        JsonAnot.Decode(nProjectInfo, jobj);
                        ProjectInfoCollection.Add(nProjectInfo);
                    }
                }

            }
        }

        public void Write()
        {
            Debug.WriteLine("SetupWindow : Write() Operating");
            string fname = R.baseDir + "ProjectInfo.json";
            JObject jRoot = new JObject();
            JsonAnot.Encode(this, jRoot);
            Debug.WriteLine("Write ClassInfo FIle");
            File.WriteAllText(fname, jRoot.ToString());
        }

        private void type_detection_click(object sender, RoutedEventArgs e)
        {
            R.projectType = ProjectType.Detection;
            Debug.WriteLine(R.projectType);
        }

        private void type_segmentation_click(object sender, RoutedEventArgs e)
        {
            R.projectType = ProjectType.Segmentation;
            Debug.WriteLine(R.projectType);
        }

        private void type_keypoint_click(object sender, RoutedEventArgs e)
        {
            R.projectType = ProjectType.KeyPoint;
            Debug.WriteLine(R.projectType);
        }


        private void export_yolo_click(object sender, RoutedEventArgs e)
        {
            R.exportType = ExportType.YOLO;
        }

        private void export_coco_click(object sender, RoutedEventArgs e)
        {
            R.exportType = ExportType.COCO;
        }

    }
}
