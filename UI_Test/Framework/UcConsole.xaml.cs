using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace UI_Test
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>

    // 메모리 누수를 발생시킬수 있음

    public partial class UiConsole : UserControl
    {
        static int MAX = 50000;
        static int MIN = 2000;
        static StringBuilder ConsoleBuffer = new StringBuilder();
        static public BlockingCollection<string> Que_Text = new BlockingCollection<string>();
        DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        SolidColorBrush bgColor = new SolidColorBrush(Color.FromArgb(255, (byte) 0x22, (byte)0x22, (byte)0x22));

        bool TextStop = false;
        static int WaitMsgCount = 0;
        static int TotalCount = 0;

        public UiConsole()
        {
            InitializeComponent();
            ConsoleBuffer.EnsureCapacity(MAX *2);
            timer.Interval = TimeSpan.FromSeconds(0.1d);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        string s;
        void timer_Tick(object sender, EventArgs e)
        {
            if ( TotalCount > MAX *.7)
            {
                ConsoleBuffer.Clear(); // 메모리누수 방지.
                return;
            }
            if ( WaitMsgCount > 0)
            {
                if (!TextStop)  // continue go
                {
                    //계속죽음
                  //  Debug.WriteLine(" ConsoleBuffer.Length = " + ConsoleBuffer.Length);
                    s = ConsoleBuffer.ToString();
                    this.ttbox.Text += s;
                    ConsoleBuffer.Clear();
                    ttbox.SelectionStart = ttbox.Text.Length;
                    ttbox.CaretIndex = ttbox.Text.Length;
                    ttbox.ScrollToEnd();
                    WaitMsgCount = 0;
                }
                else // continue stop
                {
                    //BtnGo.Content = "Wait (" + WaitMsgCount + ")";

                }
                BtnClear.Content = "Clear("+TotalCount+")";
            }
        }

        static public string GetTime()
        {
            DateTime nowDate = DateTime.Now;
            string sdate = String.Format(
                                 "{0}/{1}_{2:00}:{3:00}:{4:00} ",
                                 nowDate.Month, nowDate.Day,
                                 nowDate.Hour, nowDate.Minute, nowDate.Second);
            return sdate;
        }

        internal static void WriteLine(string fmt, params object[] var)
        {
            string text = String.Format(fmt, var);
            ConsoleBuffer.Append("\n" + GetTime() + text);
            WaitMsgCount++;
            TotalCount = ConsoleBuffer.Length;
        }
        internal static void WriteLine(Exception e)
        {
            WriteLine("{0}", e.Message);
            WriteLine("{0}", e.StackTrace);
        }

        //private void Click_BtnGo(object sender, RoutedEventArgs e)
        //{
        //    TextStop = false;
        //    ttbox.Background = bgColor;
        //    BtnGo.Content = "Continue";
        //}

        private void MDown_EndFlag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TextStop)
            {
                TextStop= false;
               
                ttbox.Background = Brushes.DarkBlue;
                UiConsole.WriteLine("--> Tsxt stop");

            } else
            {
                TextStop= true;
                ttbox.Background = Brushes.Black;
                UiConsole.WriteLine("--> Tsxt go");
              
            }
        }

        private void Click_BtnClear(object sender, RoutedEventArgs e)
        {
            ConsoleBuffer.Clear();
            TotalCount = 0;
            BtnClear.Content = "Clear";
            this.ttbox.Text = "Cleared";
        }

        internal static void Box(string msg)
        {
            WriteLine("");
            WriteLine("*******************************************");
            WriteLine("***   {0,20}", msg);
            WriteLine("*******************************************");
        }
    }

    public class DVConsole_XX : ScrollViewer
    {

        TextBlock tb = new TextBlock();
        static public BlockingCollection<string> textQ = new BlockingCollection<string>();
        public DVConsole_XX()
        {
            this.Content = tb;
            this.Margin = new Thickness(5);
            tb.Foreground = Brushes.White;
            tb.TextWrapping = TextWrapping.Wrap;

            Thread thread = new Thread(new ThreadStart(RunShow));
            thread.Start();
        }
        internal void RunShow()
        {
            DateTime LastUpdate = DateTime.Now;
            while (true)
            {
                /*
                 * 메시지가 들어오지 않으면 보이지 않는 결점.
                 */
                string text = textQ.Take();
                DateTime nowDate = DateTime.Now;
                string sdate = String.Format(
                                   "{0}/{1}_{2,2}:{3,2}:{4,2} ",
                                   nowDate.Month, nowDate.Day,
                                   nowDate.Hour, nowDate.Minute, nowDate.Second);
                tb.Dispatcher.Invoke(() =>
                {
                    tb.Text += "\n" + sdate + text;
                    if (tb.Text.Length > 6000)
                    {
                        tb.Text = tb.Text.Remove(0, tb.Text.Length - 6000);
                    }
                    this.ScrollToBottom();
                });

                //ConsoleBuffer.Append("\n" + sdate + text);
                //if ((DateTime.Now - LastUpdate).TotalSeconds > 1)
                //{
                //    LastUpdate = DateTime.Now;
                //    ttbox.Dispatcher.Invoke(
                //        () =>
                //        {
                //            this.ttbox.Text = ConsoleBuffer.ToString();
                //            ttbox.SelectionStart = ttbox.Text.Length;
                //            ttbox.CaretIndex = ttbox.Text.Length;
                //            ttbox.ScrollToEnd();

                //            if (ConsoleBuffer.Length > MAX)
                //            {
                //                string atext = ConsoleBuffer.ToString();
                //                ConsoleBuffer.Clear();
                //                ConsoleBuffer.Append(atext.Substring(atext.Length - MIN));
                //            }
                //        }
                //        );
                //}
            }

        }

        internal static void WriteLine(string fmt, params object[] var)
        {
            string text = String.Format(fmt, var);
            textQ.Add(text);
        }

      
    }


}
