using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CpuInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int interval = 1000;
        int update_number = 0;
        Timer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(0 == getDeviceInfo())
            {
                updateProcess();
            }
        }

        private int getDeviceInfo()
        {
            int ret = -1;

            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c adb shell getprop ro.product.model";
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();

                string outtr = p.StandardOutput.ReadToEnd();
                //MessageBox.Show(outtr);
                if ("" == outtr)
                {
                    devceinfo.Text = "no device";
                }
                else
                {
                    devceinfo.Text = outtr;
                    ret = 0;
                }

                p.Close();
            }
            catch (Exception)
            {

                throw;
            }

            return ret;
        }

        private void updateProcess()
        {
            startTimer();
        }

        private void startTimer()
        {
            interval = int.Parse(interval_time.Text);
            timer = new Timer(startUpdateCpuFreq, null, 0, interval);
        }

        private void startUpdateCpuFreq(object o)
        {
            updateCpuInfo();
        }

        private int updateCpuInfo()
        {
            int cpu_num = 8; // getCpuNumber();
            TextBox[] cpuText = { cpuinfo0, cpuinfo1, cpuinfo2, cpuinfo3, cpuinfo4, cpuinfo5, cpuinfo6, cpuinfo7 };
            string[] cpu_info = { "cpu0", "cpu1", "cpu2", "cpu3", "cpu4", "cpu5", "cpu6", "cpu7" };


            System.Diagnostics.Process p = new System.Diagnostics.Process();

            // 设置属性
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            String command = string.Empty;

            //adb shell cat /sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_cur_freq   
            // 同时执行多条adb shell命令
            for (int i = 0; i < cpu_num; i++)
            {
                if (i == cpu_num - 1)
                {
                    command += "adb shell cat /sys/devices/system/cpu/" + cpu_info[i] + "/cpufreq/cpuinfo_cur_freq";
                }
                else
                {
                    command += "adb shell cat /sys/devices/system/cpu/" + cpu_info[i] + "/cpufreq/cpuinfo_cur_freq" + " & ";
                }
            }
            p.StartInfo.Arguments = "/c " + command;

            try
            {
                // 开启process线程
                p.Start();
                // 获取返回结果，这个是最简单的字符串的形式返回，现在试试以其他的形式去读取返回值的结果。

                string str = string.Empty;
                System.IO.StreamReader readerout = p.StandardOutput;
                string line = string.Empty;
                int i = 0;
                while ((!readerout.EndOfStream) && i < cpu_num)
                {
                    str = readerout.ReadLine();
                    //Console.WriteLine("cpu" + i + "freq:" + str);
                    //cpuText[i].Text = str;

                    if (cpuText[i].Dispatcher.CheckAccess())
                    {
                        cpuText[i].Text = str;
                    }
                    else
                    {
                        Action act = () => { cpuText[i].Text = str; };
                        cpuText[i].Dispatcher.Invoke(act);
                    }

                    i++;
                }
                p.WaitForExit();
                p.Close();

                update_number++;
                Console.WriteLine("update_number:" + update_number);
                //update_num.Text = update_number.ToString();
                if (update_num.Dispatcher.CheckAccess())
                {
                    update_num.Text = update_number.ToString();
                }
                else
                {
                    Action act = () => { update_num.Text = update_number.ToString(); };
                    update_num.Dispatcher.Invoke(act);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return 0;
        }

        private int getCpuNumber()
        {
            int num = 1;

            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c adb shell cat /proc/cpuinfo | grep ^processor | wc -l";
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();

                Thread.Sleep(500);
                string outtr = p.StandardOutput.ReadToEnd();
                MessageBox.Show(outtr);
                cpu_number.Text = outtr;
                num = int.Parse(outtr);

                p.Close();
            }
            catch (Exception)
            {

                throw;
            }

            return num;
        }
    }
}
