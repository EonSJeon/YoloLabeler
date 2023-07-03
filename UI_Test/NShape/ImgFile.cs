using System;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;

namespace UI_Test.NShape
{
    public class ImgFile
    {
        internal Image img;
        internal double Height;
        internal double Width;

        public int Index { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public ImgFile(FileInfo info, int index)
        {
            this.Index = index;
            FullPath = info.FullName;
            Name = info.Name.Substring(0, info.Name.IndexOf('.'));
            Debug.WriteLine(Name);
        }

        public override String ToString()
        {
            return "[" + Name + "]";
        }
    }
}
