using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Net.NetworkInformation;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Process cmdP;
        public static StreamWriter cmdStreamInput;
        public static StringBuilder cmdOutput = null;

        public const int TRAN_FINISHED = 0x500;
        public static IntPtr main_whandle;
        public static IntPtr text_whandle;
        private static HwndSource m_source;
        public const int WM_VSCROLL = 0x0115;
        public const int SB_BOTTOM = 0x0007;
        public static int WM_SETREDRAW = 0x0B;

        public MainWindow()
        {
            InitializeComponent();
            main_whandle = new WindowInteropHelper(this).Handle;            
        }

        private void OnLoaded(Object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd != null && hwnd != IntPtr.Zero)
            {
                m_source = HwndSource.FromHwnd(hwnd);
                m_source.AddHook(new HwndSourceHook(MessageHookHandler));
            }
        }

        public IntPtr MessageHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == TRAN_FINISHED)
            {
                textBox1.Text = cmdOutput.ToString();
                handled = true;
            }
            return IntPtr.Zero;
        }


        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void strOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            cmdOutput.AppendLine(outLine.Data);

            
            SendMessage(main_whandle, TRAN_FINISHED, 0, 0);
        }

        [DllImport("User32.dll", EntryPoint = "RedrawWindow")]
        public static extern bool RedrawWindows(IntPtr hWnd, IntPtr prect, IntPtr hrgnUpdate, uint flags);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cmdP = new Process();
            cmdP.StartInfo.FileName = "cmd.exe";
            // 是否使用外壳程序   
            cmdP.StartInfo.UseShellExecute = false;
            // 是否在新窗口中启动该进程的值   
            cmdP.StartInfo.CreateNoWindow = true;
            // 重定向输入流  
            cmdP.StartInfo.RedirectStandardInput = true;
            // 重定向输出流
            cmdP.StartInfo.RedirectStandardOutput = true;
            //getmac命令
            string strCmd = "getmac";
            cmdP.Start();
            cmdP.StandardInput.WriteLine(strCmd);
            cmdP.StandardInput.WriteLine("exit");
            // 获取输出信息   
            textBox1.Text = cmdP.StandardOutput.ReadToEnd();
            cmdP.WaitForExit();
            cmdP.Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {


                    //Ping ping = new Ping();
                    //PingReply pingReply = ping.Send("www.baidu.com");
                    //if (pingReply.Status == IPStatus.Success)
                    //{
                    //    textBox1.Text = "当前在线，已ping通！";
                    //}
                    //else
                    //{
                    //    textBox1.Text = "不在线，ping不通！";
                    //}
                    //return;

                    text_whandle = ((HwndSource)PresentationSource.FromVisual(textBox1)).Handle;
                    cmdP = new Process();
                    cmdP.StartInfo.FileName = "cmd.exe";
                    // 是否使用外壳程序   
                    cmdP.StartInfo.UseShellExecute = false;
                    // 是否在新窗口中启动该进程的值   
                    cmdP.StartInfo.CreateNoWindow = true;
                    // 重定向输入流  
                    cmdP.StartInfo.RedirectStandardInput = true;
                    // 重定向输出流
                    cmdP.StartInfo.RedirectStandardOutput = true;
                    cmdOutput = new StringBuilder("");
                    //getmac命令
                    string strCmd = "ping www.baidu.com";
                    cmdP.Start();
                    cmdP.StandardInput.WriteLine(strCmd);
                    cmdP.StandardInput.WriteLine("exit");

                    cmdP.OutputDataReceived += new DataReceivedEventHandler(strOutputHandler);
                    cmdP.EnableRaisingEvents = true;
                    cmdStreamInput = cmdP.StandardInput;
                    cmdP.BeginOutputReadLine();

                    SendMessage(text_whandle, WM_SETREDRAW, 0, 0);


                    SendMessage(text_whandle, WM_VSCROLL, SB_BOTTOM, 50);
                    SendMessage(text_whandle, WM_SETREDRAW, 1, 0);
                    RedrawWindows(text_whandle, IntPtr.Zero, IntPtr.Zero, 1 | 4 | 128);
                    cmdP.WaitForExit();
                    cmdP.Close();
                    textBox1.Text = cmdOutput.ToString();
                    //textBox1.Text = cmdP.StandardOutput.ReadToEnd();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            cmdP = new Process();
            cmdP.StartInfo.FileName = "cmd.exe";
            // 是否使用外壳程序   
            cmdP.StartInfo.UseShellExecute = false;
            // 是否在新窗口中启动该进程的值   
            cmdP.StartInfo.CreateNoWindow = true;
            // 重定向输入流  
            cmdP.StartInfo.RedirectStandardInput = true;
            // 重定向输出流
            cmdP.StartInfo.RedirectStandardOutput = true;
            //shutdown命令 
            string strCmd = "shutdown";
            cmdP.Start();
            cmdP.StandardInput.WriteLine(strCmd);
            cmdP.StandardInput.WriteLine("exit");
            // 获取输出信息   
            textBox1.Text = cmdP.StandardOutput.ReadToEnd();
            cmdP.WaitForExit();
            cmdP.Close();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            text_whandle = ((HwndSource)PresentationSource.FromVisual(textBox1)).Handle;
            cmdP = new Process();
            cmdP.StartInfo.FileName = "cmd.exe";
            // 是否使用外壳程序   
            cmdP.StartInfo.UseShellExecute = false;
            // 是否在新窗口中启动该进程的值   
            cmdP.StartInfo.CreateNoWindow = true;
            // 重定向输入流  
            cmdP.StartInfo.RedirectStandardInput = true;
            // 重定向输出流
            cmdP.StartInfo.RedirectStandardOutput = true;
            cmdOutput = new StringBuilder("");
            //getmac命令
            string strCmd = "getmac";
            cmdP.Start();
            cmdP.StandardInput.WriteLine(strCmd);
            cmdP.StandardInput.WriteLine("exit");

            cmdP.OutputDataReceived += new DataReceivedEventHandler(strOutputHandler);
            cmdP.EnableRaisingEvents = true;
            cmdStreamInput = cmdP.StandardInput;
            cmdP.BeginOutputReadLine();

            SendMessage(text_whandle, WM_SETREDRAW, 0, 0);
           
            SendMessage(text_whandle, WM_VSCROLL, SB_BOTTOM, 50);
            SendMessage(text_whandle, WM_SETREDRAW, 1, 0);
            RedrawWindows(text_whandle, IntPtr.Zero, IntPtr.Zero, 1 | 4 | 128);

            cmdP.WaitForExit();
            cmdP.Close();
            textBox1.Text = cmdOutput.ToString();
        }

        
    }
}
