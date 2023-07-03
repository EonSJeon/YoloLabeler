using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UI_Test.NShape
{
    /// <summary>
    /// ImageListPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageListPanel : UserControl
    {
        public ShapeEditor parent;
        private string PjtDir = R.imageSourceDir;
        ObservableCollection<ImgFile> listImgFile = new ObservableCollection<ImgFile>();

        public ImageListPanel()
        {
            InitializeComponent();
        }

        public void scanDirectory()
        {
            Scan(PjtDir);
            TableImage.ItemsSource = listImgFile;
        }

        private void DataGrid_Select(object sender, SelectionChangedEventArgs e)
        {
            ImgFile mfile = TableImage.SelectedItem as ImgFile;
            if (mfile != null)
            {
                Debug.WriteLine("\nSelected_FileName  ==> " + mfile.FullPath);
                //Debug.WriteLine("\nSelected_FileName  ==> " + mfile.Name);
                Image img = (Image)UiCache.Find("UImage");
                if (img != null)
                {
                    BitmapImage bimg = new BitmapImage(new Uri(mfile.FullPath, UriKind.RelativeOrAbsolute));
                    img.Source = bimg;
                    mfile.Height = bimg.Height;
                    mfile.Width = bimg.Width;
                }
                LabelListPanel labelListPanel = LabelListPanel.Inst;
                if (labelListPanel != null)
                {
                    mfile.img = img;
                    labelListPanel.MyFile = mfile;
                }

            }
        }

        public void Scan(string path)
        {
            try
            {
                listImgFile.Clear();
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray(); // 날짜순 정렬
                var Count = 0;
                foreach (var file in files)
                {
                    if (file.Name.Contains(".txt") || file.Name.Contains(".json"))
                    {
                        continue;
                    }
                    ImgFile node = new ImgFile(file, Count++);
                    listImgFile.Add(node);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to scan {ex.Message}");
            }
        }
    }
    
}
