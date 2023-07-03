using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UI_Test.NShape;

namespace UI_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        protected bool isDragging;
        private Point clickPosition;
        private bool isMaximized = false;

        public MainWindow()
        {
            
            SetupWindow setup = new SetupWindow();
            if(setup.ShowDialog() == true)
            {
                R.printValue();
            }


            InitializeComponent();

            if(R.projectType == ProjectType.Detection)
            {
                canvasMain.btnModeBrush.Visibility = Visibility.Hidden;
                canvasMain.btnModePolyPath.Visibility = Visibility.Hidden;
            }
            else
            {
                canvasMain.btnModeBox.Visibility = Visibility.Hidden;
            }

            Debug.WriteLine("MainWindow : Setup Done");
            canvasMain.panelImage.scanDirectory();
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableControl = sender as Border;
            clickPosition = e.GetPosition(this.Parent as UIElement);
            draggableControl.CaptureMouse();
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as Border;
            draggable.ReleaseMouseCapture();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as Border;

            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(this.Parent as UIElement);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                transform.X = currentPosition.X - clickPosition.X;
                transform.Y = currentPosition.Y - clickPosition.Y;
            }
        }

        private void OnMouseLetfDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (isMaximized == false)
                {
                    this.WindowState = WindowState.Maximized;
                    isMaximized = true;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    isMaximized = false;
                }
            }
            this.DragMove();
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void btnMinWindow_Click(object sender, RoutedEventArgs e)
        {
           this.WindowState = WindowState.Minimized;
        }

        private void btnFullWindo_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized == false)
            {
                this.WindowState = WindowState.Maximized;
                isMaximized = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                isMaximized = false;
            }
        }
    }

}
