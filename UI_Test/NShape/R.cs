using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UI_Test.NShape
{
    public enum ProjectType
    {
        Detection,
        Segmentation,
        KeyPoint
    }

    public enum ExportType
    {
        YOLO,
        COCO
    }

    public static class R
    {
        public static string baseDir = @"C:\DIT\";
        public static string projectDir = @"C:\Temp\Yolo";
        public static string imageSourceDir = @"C:\Temp\Yolo\image";
        public static string projectName = "New Project";
        public static ProjectType projectType = ProjectType.Detection;
        public static ExportType exportType = ExportType.YOLO;


        public static void printValue()
        {
            Debug.WriteLine(baseDir, "BaseDir");
            Debug.WriteLine(projectDir, "projectDir");
            Debug.WriteLine(imageSourceDir, "imageSourceDir");
            Debug.WriteLine(projectName, "projectName");
            Debug.WriteLine(projectType, "projectType");
            Debug.WriteLine(exportType, "exportType");
        }
    }

}
