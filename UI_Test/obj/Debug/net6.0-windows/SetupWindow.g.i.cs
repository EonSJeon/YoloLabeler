﻿#pragma checksum "..\..\..\SetupWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "312BDF3C4CA771AE306BA04B7468E589D74E7AFA"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UI_Test;


namespace UI_Test {
    
    
    /// <summary>
    /// SetupWindow
    /// </summary>
    public partial class SetupWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 84 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DGProject;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textProjectName;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textProjectDir;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnProjectDir;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textImageSourceDir;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnImageSourceDir;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radio_detection;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radio_segmentation;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radio_key_point;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radio_yolo;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton radio_coco;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\..\SetupWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStartNew;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UI_Test;component/setupwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SetupWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\SetupWindow.xaml"
            ((UI_Test.SetupWindow)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DGProject = ((System.Windows.Controls.DataGrid)(target));
            
            #line 91 "..\..\..\SetupWindow.xaml"
            this.DGProject.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.DataGridProject_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.textProjectName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.textProjectDir = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.btnProjectDir = ((System.Windows.Controls.Button)(target));
            
            #line 112 "..\..\..\SetupWindow.xaml"
            this.btnProjectDir.Click += new System.Windows.RoutedEventHandler(this.ProjectDir_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.textImageSourceDir = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.btnImageSourceDir = ((System.Windows.Controls.Button)(target));
            
            #line 117 "..\..\..\SetupWindow.xaml"
            this.btnImageSourceDir.Click += new System.Windows.RoutedEventHandler(this.ImageSourceDir_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.radio_detection = ((System.Windows.Controls.RadioButton)(target));
            
            #line 122 "..\..\..\SetupWindow.xaml"
            this.radio_detection.Click += new System.Windows.RoutedEventHandler(this.type_detection_click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.radio_segmentation = ((System.Windows.Controls.RadioButton)(target));
            
            #line 123 "..\..\..\SetupWindow.xaml"
            this.radio_segmentation.Click += new System.Windows.RoutedEventHandler(this.type_segmentation_click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.radio_key_point = ((System.Windows.Controls.RadioButton)(target));
            
            #line 124 "..\..\..\SetupWindow.xaml"
            this.radio_key_point.Click += new System.Windows.RoutedEventHandler(this.type_keypoint_click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.radio_yolo = ((System.Windows.Controls.RadioButton)(target));
            
            #line 128 "..\..\..\SetupWindow.xaml"
            this.radio_yolo.Click += new System.Windows.RoutedEventHandler(this.export_yolo_click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.radio_coco = ((System.Windows.Controls.RadioButton)(target));
            
            #line 129 "..\..\..\SetupWindow.xaml"
            this.radio_coco.Click += new System.Windows.RoutedEventHandler(this.export_coco_click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.btnStartNew = ((System.Windows.Controls.Button)(target));
            
            #line 131 "..\..\..\SetupWindow.xaml"
            this.btnStartNew.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

