using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XKLAUNCHER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Process nircmd = new Process();
            Width.Text = Convert.ToString(Properties.Settings.Default.CustomWidth);
            Height.Text = Convert.ToString(Properties.Settings.Default.CustomHeight);
            CustomParamsText.Text = Properties.Settings.Default.CustomParameters;
            CustomParams.IsChecked = Properties.Settings.Default.CustomParamCheck;
            CustomRes.IsChecked = Properties.Settings.Default.CustomResCheck;
            AutoParams.IsChecked = Properties.Settings.Default.AutoParamCheck;
            string nircmdPath = Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.exe");
            string nircmdZipPath = Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.zip");
            Properties.Settings.Default.NativeHeight = Convert.ToInt32(SystemParameters.PrimaryScreenHeight);
            Properties.Settings.Default.NativeWidth = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);
            //https://www.nirsoft.net/utils/nircmd.zip
            if (File.Exists(nircmdPath))
            {
                Console.WriteLine("yes " + Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.exe"));
            }
            else
            {
                Console.WriteLine("no " + Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.exe"));
                Directory.CreateDirectory(@Environment.ExpandEnvironmentVariables(" %APPDATA%/nircmd/"));
                WebClient wc = new WebClient();
                {
                        wc.DownloadFile(
                        // Param1 = Link of file
                        new System.Uri("https://www.nirsoft.net/utils/nircmd.zip"),
                        // Param2 = Path to save
                        @Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.zip")
                    );
                }
                ZipFile.ExtractToDirectory(nircmdZipPath, @Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/"));
                // nircmd.StartInfo.FileName = nircmdPath;
                // nircmd.StartInfo.UseShellExecute = false;
                // nircmd.StartInfo.CreateNoWindow = true;
                // MessageBox.Show("Please click \"Copy to windows directory\"");
                // MessageBox.Show("-setdisplay " + Properties.Settings.Default.CustomWidth + " " + Properties.Settings.Default.CustomHeight + " 32");
                // nircmd.Start();
            }

            Properties.Settings.Default.Save();
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            string nircmdPath = Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.exe");
            string forcedParams = "-hero.title \"xKlan_\" -hero.action Join -hero.url https://discord.gg/ZNpgZaT -hero.video https://cdn.discordapp.com/attachments/516358914108030997/808388742065946624/xtapeta_nocna.mp4 -hero.show";
            if (CustomRes.IsChecked == true)
            {
                forcedParams = forcedParams + " -screen-width " + Properties.Settings.Default.CustomWidth + " -screen-height " + Properties.Settings.Default.CustomHeight;
            }
            else
            {
                forcedParams = forcedParams + " -screen-width " + Properties.Settings.Default.NativeWidth + " -screen-height " + Properties.Settings.Default.NativeHeight;
            }
            bool customParamsChecked;
            if (CustomParamsText.Text.Length != 0 & CustomParams.IsChecked == true)
            {
                customParamsChecked = true;
            }
            else
            {
                customParamsChecked = false;
            }
            string autoParamsFinal = "";
            if (AutoParams.IsChecked == true)
            {
                int coreCount = 0;
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
                int logicalProcessorCount = Environment.ProcessorCount;
                Console.WriteLine(coreCount + " " + logicalProcessorCount);
                ulong systemRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                int systemRamMB = Convert.ToInt32((systemRam / 1024) / 1024);
                Console.WriteLine(systemRamMB + "MB ram");
                autoParamsFinal = "-nolog -high -maxMem=" + Math.Round(systemRamMB * 0.8) + " -malloc=system -force-feature-level-11-0 -cpuCount=" + coreCount + " -exThreads=" + logicalProcessorCount + " -force-d3d11-no-singlethreaded -gc.buffer " + Math.Round(Convert.ToDouble(systemRamMB / 8));
                Console.WriteLine("auto parameters set:" + autoParamsFinal);
            }
            bool ProcShell = false;
            bool ProcWindow = false;
            Process Proc = new Process();

            Proc.StartInfo.FileName = Properties.Settings.Default.SteamDir + "/steam.exe";
            if (customParamsChecked & AutoParams.IsChecked == false)
            {
                Proc.StartInfo.Arguments = "-applaunch 252490 " + CustomParamsText.Text + " " + forcedParams;
                Console.WriteLine("param no auto");
            }
            else if (!customParamsChecked & AutoParams.IsChecked == true)
            {
                Proc.StartInfo.Arguments = "-applaunch 252490 " + autoParamsFinal + " " + forcedParams;
                Console.WriteLine("no param auto");
            }
            else if (customParamsChecked & AutoParams.IsChecked == true)
            {
                Proc.StartInfo.Arguments = "-applaunch 252490 " + CustomParamsText.Text + " " + autoParamsFinal + " " + forcedParams;
                Console.WriteLine("param auto");
            }
            else
            {
                Proc.StartInfo.Arguments = "-applaunch 252490" + " " + forcedParams;
                Console.WriteLine("no param no auto");
            }
            Proc.StartInfo.UseShellExecute = ProcShell;
            Proc.StartInfo.CreateNoWindow = ProcWindow;
            Proc.Start();
            if (CustomRes.IsChecked == true)
            {
                Launchnircmd();
            }
        }
        public async void Launchnircmd()
        {
            string nircmdPath = Environment.ExpandEnvironmentVariables("%APPDATA%/nircmd/nircmd.exe");
            Process nircmd = new Process();
            nircmd.StartInfo.FileName = nircmdPath;
            nircmd.StartInfo.UseShellExecute = false;
            nircmd.StartInfo.CreateNoWindow = true;
            nircmd.StartInfo.Arguments = " setdisplay " + Width.Text + " " + Height.Text + " 32";
            nircmd.Start();
            await Task.Delay(5000);
            while (Process.GetProcessesByName("Rust").Length > 0)
            {
                await Task.Delay(5000);
            }
            
            await Task.Delay(2000);
            while (Process.GetProcessesByName("RustClient").Length > 0)
            {
                await Task.Delay(1000);
            }
            nircmd.StartInfo.FileName = nircmdPath;
            nircmd.StartInfo.UseShellExecute = false;
            nircmd.StartInfo.CreateNoWindow = true;
            nircmd.StartInfo.Arguments = " setdisplay " + Properties.Settings.Default.NativeWidth + " " + Properties.Settings.Default.NativeHeight + " 32";
            nircmd.Start();
        }

        private void ChangeSteamDir_Click(object sender, RoutedEventArgs e)
        {

            var SteamDirSelector = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            string SteamdirSelection;
            SteamDirSelector.ShowDialog();
            SteamdirSelection = SteamDirSelector.SelectedPath;
            Console.WriteLine(SteamdirSelection);
            Properties.Settings.Default["SteamDir"] = SteamdirSelection;
            Properties.Settings.Default.Save();
        }

        private void CustomParamsCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CustomParamCheck = Convert.ToBoolean(CustomParams.IsChecked);
            Properties.Settings.Default.Save();
        }

        private void AutoParamsCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoParamCheck = Convert.ToBoolean(AutoParams.IsChecked);
            Properties.Settings.Default.Save();
        }


        private void Height_ValueChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.CustomHeight = Convert.ToInt32(Height.Text);
            Console.WriteLine(Properties.Settings.Default.CustomWidth);
            Properties.Settings.Default.Save();
        }

        private void Width_ValueChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.CustomWidth = Convert.ToInt32(Width.Text);
            Console.WriteLine(Properties.Settings.Default.CustomWidth);
            Properties.Settings.Default.Save();
        }

        private void CustomResCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CustomResCheck = Convert.ToBoolean(CustomRes.IsChecked);
            Properties.Settings.Default.Save();
        }

        private void CustomParams_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.CustomParameters = CustomParamsText.Text;
            Properties.Settings.Default.Save();
        }
    }
}
