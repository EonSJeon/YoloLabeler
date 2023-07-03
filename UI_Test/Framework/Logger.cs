using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;



namespace UI_Test
{
    public enum LogLevel
    {
        ALL = 9,
        Debug = 1, // 프로그램을 디버깅하기 위한 정보저장
        Info = 2,  // 상태변경과 같은 정보를 나타낸다.  
        Warn = 3,  // 처리가능한 문제, 향후 시스템에러의 원인이 될수 있는 경고성메시지를 나타냄
        Error = 4, // 요청을 처리하는중에 문제가 발생한 경우
        Fatal = 5, // 아주 심각한 문제 발생으로 app수행이 불가한 경우
        None = 0
    }



    public class Logger
    {
        static public Dictionary<String, Logger> LogPool = new Dictionary<string, Logger>()
        {
            {"All",new Logger("All", LogLevel.ALL)},
            {"Init",new Logger("Init", LogLevel.Info)},
            {"Platform",new Logger("Platform", LogLevel.None)},
            {"Json",new Logger("Json", LogLevel.None)},
            {"Camera",new Logger("Camera", LogLevel.None)},
            {"Gallery",new Logger("Gallery", LogLevel.None)},
              {"Recipe",new Logger("Recipe", LogLevel.Info)}
        };

        [JsonRepo(Name = "Level")]
        private LogLevel level { get; set; }
        private bool ConsoleFlag = false;
        static public bool pub_mode = false;
        private string FilePath;

        [JsonRepo(Name = "LogName")]
        public String LogName { get; set; }


        public Logger(string LogName, LogLevel level)
        {
            this.LogName = LogName;
            this.level = level;
        }
        public Logger(string LogName)
        {
            this.LogName = LogName;
            this.level = LogLevel.Info;
        }



        static public Logger GetLogger(string clsName, bool Console = false, string File = null)
        {
            Logger nLog;
            if (LogPool.ContainsKey(clsName))
            {
                nLog = LogPool[clsName];
            }
            else
            {
                nLog = new Logger(clsName);
                LogPool.Add(clsName, nLog);
                nLog.ConsoleFlag = Console;
            }
            if (File != null)
            {
                nLog.FilePath = File;
                FileInfo fi = new FileInfo(nLog.FilePath);
                if (fi.Exists)
                {
                    System.IO.File.AppendAllText(nLog.FilePath, "라인 추가됨 ---\n", Encoding.Default);
                }
                else
                {
                    System.IO.File.WriteAllText(nLog.FilePath, "화일 생성됨 ---", Encoding.Default);
                }
            }

            return nLog;
        }

        private void WriteLine(string msg)
        {
            //if (FilePath != null)
            //{
            //    File.AppendAllText(FilePath, msg + "\n");
            //}

            //  using (var sw = new StreamWriter(s, new UnicodeEncoding(false, false)))

            //뭔지는 모르겠지만...    
            //using (var sw = new StreamWriter(FilePath, append: true, Encoding.Default))
            //{
            //    sw.WriteLine(msg);
            //}

        }

        internal void setLogLevel(LogLevel level)
        {
            this.level = level;
        }

        public void Info(string text, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {

            if (GetLogger("All").level == LogLevel.None || this.level < LogLevel.Info)
            {
                return;
            }
            file = file.Substring(file.LastIndexOf("\\"));
            {
                var result = text.Split(new[] { '\r', '\n' });
                string outmsg = "";
                int count = 0;
                foreach (string txt in result)
                {
                    if (txt.Length == 0)
                    {
                        continue;
                    }
                    if (count++ > 0)
                    {
                        outmsg += "\n";
                        outmsg += String.Format("[ {0,-10} .  {1,-80}  ", this.LogName, txt, caller, file);
                    }
                    else
                    {
                        outmsg += String.Format("[ {0,-10} ]  {1,-80} ___ {2}  {3}", this.LogName, txt, caller, file);
                    }

                }


                //   String msg = String.Format("{1,-80} ___ {2}  {3}", this.LogName, text, caller, file);
                //    String outmsg = String.Format("{1,-80} ___ {2}  {3}", this.LogName, text, caller, file);
                if (ConsoleFlag)
                {
                    UiConsole.WriteLine(outmsg);
                }
                else
                {
                    Debug.WriteLine(outmsg);
                }
                WriteLine(outmsg);
                //if (FilePath != null)
                //{
                //    File.AppendAllText(FilePath, outmsg + "\n");
                //}
            }
        }


        internal void infox(string fmt, params object[] var)
        //[CallerMemberName] string caller = "", [CallerFilePath] string file = "",
        {
            if (pub_mode)
            {
                return;
            }
            if (level <= LogLevel.Info)
            {
                string msg = String.Format(fmt, var);
                Debug.WriteLine("INFO | {0}::{1}", LogName, msg);
            }

        }
        public void error(string text, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        //[CallerMemberName] string caller = "", [CallerFilePath] string file = "",
        {
            file = file.Substring(file.LastIndexOf("\\"));
            if (level <= LogLevel.Error)
            {
                //  string msg = String.Format(fmt, var);
                Debug.WriteLine("+=====================================================+");
                Debug.WriteLine("+=============        ERROR       ====================+");
                Debug.WriteLine("+=====================================================+");
                Debug.WriteLine("[  *] {0} ___ {1} @{2}", text, caller, file);
            }

        }
        public void Fatal(string text, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        {
            file = file.Substring(file.LastIndexOf("\\"));
            if (level <= LogLevel.Fatal)
            {
                //  string msg = String.Format(fmt, var);
                Debug.WriteLine("+=====================================================+");
                Debug.WriteLine("+=============        FATAL       ====================+");
                Debug.WriteLine("+=====================================================+");
                Debug.WriteLine("** {0} ___ {1} @{2}", text, caller, file);

            }
        }

        internal void Banner(string msg)
        {
            if (GetLogger("All").level == LogLevel.None || this.level < LogLevel.Info)
            {
                return;
            }
            var result = msg.Split(new[] { '\r', '\n' });
            String outmsg = String.Format("[ {0,-10 } ]  +------------------------------------------------------------------------------------+", LogName, msg);
            int count = 0;
            foreach (string txt in result)
            {
                outmsg += String.Format("\n[ {0,-10 } ]  |    {1,-80}| ", LogName, txt);
            }
            outmsg += String.Format("\n[ {0,-10 } ]  +------------------------------------------------------------------------------------+", LogName, msg);

            if (ConsoleFlag)
            {
                UiConsole.WriteLine(outmsg);
            }
            else
            {
                Debug.WriteLine(outmsg);
            }
        }
    }
}