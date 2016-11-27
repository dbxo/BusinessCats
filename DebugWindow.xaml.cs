using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices; //APIs
using System.Text; //StringBuilder
using System.Diagnostics;

namespace BusinessCats
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public DebugWindow()
        {
            InitializeComponent();
        }

        public void info(string debug)
        {
            textBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                   new Action(delegate ()
                                   {
                                       textBox.AppendText(debug+"\n");
                                   }));
        }

        const int GWL_STYLE = (-16);
        const UInt32 WS_MAXIMIZE = 0x1000000;
        const UInt32 WS_MINIMIZE = 0x20000000;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        // CommunicatorMainWindowClass
        // LyncConversationWindowClass
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Process p = Process.GetProcessesByName("lync").First();
            foreach(var handle in EnumerateProcessWindowHandles(p.Id))
            {
                //int style = GetWindowLong(handle, GWL_STYLE);
                string estilo = "";
                if (IsIconic(handle))
                {
                    estilo = "Minimizada";
                } else 
                {
                    estilo = "Maximizada";
                }
                ShowWindowAsync(handle, SW_SHOWMINIMIZED);
                info(handle.ToString()+":"+GetWindowClass(handle)+":"+GetWindowTextRaw(handle)+":"+estilo);
            }
        }

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)] //
            public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;

        public static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate correct string length first
            int length = (int)SendMessage((int) hwnd, WM_GETTEXTLENGTH, 0, 0);
            StringBuilder sb = new StringBuilder(length + 1);
            SendMessage(hwnd, WM_GETTEXT, sb.Capacity, sb);
            return sb.ToString();
        }

        private static string GetWindowClass(IntPtr hWnd)
        {
            int nRet;
            // Pre-allocate 256 characters, since this is the maximum class name length.
            StringBuilder ClassName = new StringBuilder(256);
            //Get the window class name
            nRet = GetClassName(hWnd, ClassName, ClassName.Capacity);
            if (nRet != 0)
            {
                return (ClassName.ToString());
            }
            else
            {
                return "";
            }
        }

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        List<IntPtr> minimizedWindows = new List<IntPtr>();
        /*
        * Minimizes all skype windows
          CommunicatorMainWindowClass
          LyncConversationWindowClass
        */
        public void MinizeSkypeWindows()
        {
            minimizedWindows.Clear();
            Process p = Process.GetProcessesByName("lync").First();
            foreach (var handle in EnumerateProcessWindowHandles(p.Id))
            {
                string className = GetWindowClass(handle);
                if (className.Equals("CommunicatorMainWindowClass") || className.Equals("LyncConversationWindowClass"))
                {
                    if (!IsIconic(handle))
                    {
                        ShowWindowAsync(handle, SW_SHOWMINIMIZED);
                        minimizedWindows.Add(handle);
                    }
                }
            }

        }

        /*
        * Restores all skype windows
        */
        public void RestoreSkypeWindows()
        {
            lock(minimizedWindows)
            {
                foreach (var handle in minimizedWindows)
                {
                    ShowWindowAsync(handle, SW_SHOWNORMAL);                    
                }
            }
            
        }
    }

}
