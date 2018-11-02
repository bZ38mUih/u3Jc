using System;
using System.Diagnostics; //for process
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Management;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
//using 
//using System.ServiceProcess;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<envVars> envList = new List<envVars>();
        List<Hardware> hwList = new List<Hardware>();
        List<pProcess> ProcessLst = new List<pProcess>();
        List<sService> ServicesList = new List<sService>();
        diagList tDiag = new diagList();

        public void dgClear()
        {
            envVar_dg.Items.Clear();
            envVar_dg.Items.Refresh();
            hardware_dg.Items.Clear();
            hardware_dg.Items.Refresh();
            Process_dg.Items.Clear();
            Process_dg.Items.Refresh();
            Services_dg.Items.Clear();
            Services_dg.Items.Refresh();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Regex regex = new Regex(@"^\w+$");
            //Regex regex = new Regex(@"^[0-9-]*$");
            Regex regex = new Regex(@"^[a-zA-Z0-9-]*$");
            if (regex.IsMatch(expName.Text))
            {
                tDiag.fName = expName.Text;
                tDiag.srvList = ServicesList;
                tDiag.procList = ProcessLst;
                tDiag.envList = envList;
                tDiag.hardwareList = hwList;
                string output = JsonConvert.SerializeObject(tDiag);
                System.IO.File.WriteAllText(expName.Text+".json", output);
            }
            else
            {
                MessageBox.Show("fName is not correct");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            
            dgClear();
            /*
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                string name = (string)env.Key;
                string value = (string)env.Value;
                var machineName = new envVars { vName = "envir-"+name, vVal = value };
                envVar_dg.Items.Add(machineName);
                envList.Add(machineName);
            }
            */

            /*EnviropmentVars-->
            var machineName = new envVars { vName = "MachineName", vVal = Environment.MachineName };
            envVar_dg.Items.Add(machineName);
            envList.Add(machineName);

            var osVersion = new envVars { vName = "OSVersion", vVal = Environment.OSVersion.ToString() };
            envVar_dg.Items.Add(osVersion);
            envList.Add(osVersion);

            var systemDirectory = new envVars { vName = "SystemDirectory", vVal = Environment.SystemDirectory };
            envVar_dg.Items.Add(systemDirectory);
            envList.Add(systemDirectory);

            var userName = new envVars { vName = "UserName", vVal = Environment.UserName };
            envVar_dg.Items.Add(userName);
            envList.Add(userName);

            var version = new envVars { vName = "Version", vVal = Environment.Version.ToString() };
            envVar_dg.Items.Add(version);
            envList.Add(version);

            var is64BitOS = new envVars { vName = "Is64BitOS", vVal = Environment.Is64BitOperatingSystem.ToString() };
            envVar_dg.Items.Add(is64BitOS);
            envList.Add(is64BitOS);
            /*EnviropmentVars<--*/

            /*hardware-->*/
            /*
foreach (var prop in item.Properties)
{
    string propVal = "-";
    if (prop.Value != null)
    {
        propVal = prop.Value.ToString();
    }
    var hwProcName = new Hardware { paramName = prop.Name, paramVal = propVal };
    hardware_dg.Items.Add(hwProcName);
    hwList.Add(hwProcName);
}
*/
            
            //baseBoard-->
            var hwMBoard = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (var item in hwMBoard.Get())
            {
                string mbModel = "-";
                if (item["Model"] != null)
                {
                    mbModel = item["Model"].ToString();
                }
                var hwMBoardName = new Hardware { paramName = item["Tag"].ToString()+"("+
                    item["Manufacturer"].ToString()+ ")", paramVal = mbModel };
                hardware_dg.Items.Add(hwMBoardName);
                hwList.Add(hwMBoardName);
            }
            //processor-->
            var hwProcessor = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (var item in hwProcessor.Get())
            {
                var hwProcName = new Hardware { paramName = "processor", paramVal = item["Name"].ToString() };
                hardware_dg.Items.Add(hwProcName);
                hwList.Add(hwProcName);
            }
            //graphics-->
            var hwGpu = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (var item in hwGpu.Get())
            {
                var hwGpuName = new Hardware { paramName = "graphics", paramVal = item["Name"].ToString() };
                hardware_dg.Items.Add(hwGpuName);
                hwList.Add(hwGpuName);
            }
            //audio-->
            var hwSound = new ManagementObjectSearcher("select * from Win32_SoundDevice");
            foreach (var item in hwSound.Get())
            {
                var hwAudioName = new Hardware { paramName = "Audio", paramVal = item["Name"].ToString() };
                hardware_dg.Items.Add(hwAudioName);
                hwList.Add(hwAudioName);
            }
            //Discs-->
            var hwDiscs = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (var item in hwDiscs.Get())
            {
                var hwHDDCaption = new Hardware { paramName = "Disk(" + item["interfaceType"].ToString() + ")-Model", paramVal = item["Model"].ToString() };
                hardware_dg.Items.Add(hwHDDCaption);
                hwList.Add(hwHDDCaption);
                double totalSize = Convert.ToDouble(item["Size"]);
                hwHDDCaption = new Hardware { paramName = "Disk(" + item["interfaceType"].ToString() + ")-Size",
                    paramVal = Math.Round((totalSize / (1024 * 1024)), 2).ToString() + "(GB)" };
                hardware_dg.Items.Add(hwHDDCaption);
                hwList.Add(hwHDDCaption);
            }   
               /*
                var hwHDDCaption = new Hardware { paramName = "Disk("+item["interfaceType"].ToString()+")-Model", paramVal = item["Model"].ToString() };
                hardware_dg.Items.Add(hwHDDCaption);
                hwList.Add(hwHDDCaption);
                hwHDDCaption = new Hardware { paramName = "Disk("+item["interfaceType"].ToString()+")-Size", paramVal = item["Size"].ToString() };
                hardware_dg.Items.Add(hwHDDCaption); 2
                hwList.Add(hwHDDCaption);
                */
            //}
            /*
            var hwRam = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            double ramTotal;
            foreach (var item in hwRam.Get())
            {
                
                foreach (var prop in item.Properties)
                {
                    string propVal = "-";
                    if (prop.Value != null)
                    {
                        propVal = prop.Value.ToString();
                    }
                    var hwHDDCaption = new Hardware { paramName = prop.Name, paramVal = propVal };
                    hardware_dg.Items.Add(hwHDDCaption);
                    hwList.Add(hwHDDCaption);
                }

                /*
                ramTotal = Convert.ToDouble(item["TotalVisibleMemorySize"]);
                var hwRamValue = new Hardware { paramName = "RAM", paramVal = Math.Round((ramTotal / (1024 * 1024)), 2).ToString() + "(GB)" };
                hardware_dg.Items.Add(hwRamValue);
                hwList.Add(hwRamValue);
                */
                
            //}
            
            /*hardware<--*/

            /*process-->
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                try
                {
                    var pTest = new pProcess { pName = theprocess.ProcessName, PID = theprocess.Id.ToString(), pPath = theprocess.MainModule.FileName };
                    Process_dg.Items.Add(pTest);
                    ProcessLst.Add(pTest);
                }
                catch
                {
                    var pTest = new pProcess { pName = theprocess.ProcessName, PID = theprocess.Id.ToString(), pPath = "not allowed" };
                    Process_dg.Items.Add(pTest);
                    ProcessLst.Add(pTest);
                }
            }
            /*process<--*/

            /*services-->
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController theService in scServices)
            {
                if (theService.Status == ServiceControllerStatus.Running)
                {
                    try
                    {
                        ManagementObject wmiService;
                        wmiService = new ManagementObject("Win32_Service.Name='" + theService.ServiceName + "'");
                        wmiService.Get();
                        var sSrv = new sService
                        {
                            sName = theService.ServiceName,
                            sDName = theService.DisplayName,
                            sSTName = wmiService["StartName"].ToString(),
                            sDescr = wmiService["Description"].ToString(),
                            sPath = wmiService["PathName"].ToString()
                        };
                        Services_dg.Items.Add(sSrv);
                        ServicesList.Add(sSrv);
                    }
                    catch
                    { }
                }
            }
            /*services<--*/
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("click load");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                dgClear();
                envList.Clear();
                hwList.Clear();
                ProcessLst.Clear();
                ServicesList.Clear();
                var fullPath = openFileDialog.FileName;
                string diagRes = System.IO.File.ReadAllText(fullPath);
                tDiag = JsonConvert.DeserializeObject<diagList>(diagRes);
                foreach (envVars diagEnv in tDiag.envList)
                {
                    envVar_dg.Items.Add(diagEnv);
                    envList.Add(diagEnv);
                }
                foreach (Hardware diagHw in tDiag.hardwareList)
                {
                    hardware_dg.Items.Add(diagHw);
                    hwList.Add(diagHw);
                }
                foreach (pProcess diagP in tDiag.procList)
                {
                    Process_dg.Items.Add(diagP);
                    ProcessLst.Add(diagP);
                }
                foreach (sService diagS in tDiag.srvList)
                {
                    Services_dg.Items.Add(diagS);
                    ServicesList.Add(diagS);
                }
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("click check");
        }
    }

    public class diagList
    {
        //public string listId { get; set; }
        public string fName { get; set; }
        public List<envVars> envList { get; set; }
        public List<Hardware> hardwareList { get; set; }
        public List<pProcess> procList { get; set; }
        public List<sService> srvList { get; set; }
    }

    public class sService
    {
        public string sName { get; set; }
        public string sDName { get; set; }
        public string sSTName { get; set; }
        public string sDescr { get; set; }
        public string sPath { get; set; }
    }

    public class Hardware
    {
        public string paramName { get; set; }
        public string paramVal { get; set; }
    }

    public class envVars
    {
        public string vName { get; set; }
        public string vVal { get; set; }
    }

    public class pProcess
    {
        public string pName { get; set; }
        public string PID { get; set; }
        public string pPath { get; set; }
    }
}
