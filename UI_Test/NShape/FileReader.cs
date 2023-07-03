using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UI_Test.NShape
{
    internal class FileReader
    {
        ImgFile First = null;
        ImgFile Last = null;
        ImgFile cp = null;
        Image img = null;

        public FileReader(Image image)
        {
            this.img = image;
           
        }

      

        //public void Add(MFile node)
        //{
        //    if (First == null)
        //    {
        //        First = node;
        //        Last = node;
        //        cp = node;
        //    }
        //    else
        //    {
        //        Last.Next = node;
        //        node.Prev = Last;
        //        Last = node;
        //    }

        //}

        //public void Up()
        //{
        //    if (cp != null && cp.Prev != null)
        //    {
        //        cp = cp.Prev;
        //        img.Source = new BitmapImage(new Uri(cp.fullPath, UriKind.RelativeOrAbsolute));
        //    }
        //}
        //public void Down()
        //{
        //    if (cp != null && cp.Next != null)
        //    {
        //        cp = cp.Next;
        //        img.Source = new BitmapImage(new Uri(cp.fullPath, UriKind.RelativeOrAbsolute));
        //    }
        //}
    }

  
}
