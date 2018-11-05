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
        List<OS> osList = new List<OS>();
        List<Hardware> hwList = new List<Hardware>();
        List<pProcess> ProcessLst = new List<pProcess>();
        List<sService> ServicesList = new List<sService>();
        diagList tDiag = new diagList();

        public void dgClear(Boolean loadAct=false)
        {
            if (optEnvironment.IsChecked == true || loadAct==true)
            {
                envVar_dg.Items.Clear();
                envVar_dg.Items.Refresh();
            }
            if (optOS.IsChecked == true || loadAct == true)
            {
                OS_dg.Items.Clear();
                OS_dg.Items.Refresh();
            }
            if (optHardware.IsChecked == true || loadAct == true)
            {
                hardware_dg.Items.Clear();
                hardware_dg.Items.Refresh();
            }
            if (optProcess.IsChecked == true || loadAct == true)
            {
                Process_dg.Items.Clear();
                Process_dg.Items.Refresh();
            }
            if (optServices.IsChecked == true || loadAct == true)
            {
                Services_dg.Items.Clear();
                Services_dg.Items.Refresh();
            }
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
            Mouse.OverrideCursor = Cursors.Wait;
            Regex regex = new Regex(@"^[a-zA-Z0-9-]*$");
            if (regex.IsMatch(expName.Text))
            {
                

                Boolean sfileFlag = false;

                tDiag.fName = expName.Text;
                if (envList.Count > 0)
                {
                    sfileFlag = true;
                    tDiag.envList = envList;
                }
                if (osList.Count > 0)
                {
                    sfileFlag = true;
                    tDiag.osList = osList;
                }
                if (hwList.Count > 0)
                {
                    sfileFlag = true;
                    tDiag.hardwareList = hwList;
                }
                if(ProcessLst.Count > 0)
                {
                    sfileFlag = true;
                    tDiag.procList = ProcessLst;
                }
                if (ServicesList.Count > 0)
                {
                    sfileFlag = true;
                    tDiag.srvList = ServicesList;
                }
                if (sfileFlag == true)
                {
                    string output = JsonConvert.SerializeObject(tDiag);
                    System.IO.File.WriteAllText(expName.Text + ".json", output);
                }
                else
                {
                    MessageBox.Show("no data to save");
                }
                
            }
            else
            {
                MessageBox.Show("fName is not correct");
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            
            dgClear();
            Boolean optSl = false;

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
            /*EnviropmentVars-->*/
            //Cursor.Current = Cursors.WaitCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            if (optEnvironment.IsChecked == true)
            {
                
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                string name = (string)env.Key;

                    
                if (name == "ProgramFiles" || name== "SystemRoot" || name=="OS")
                {
                      string value = (string)env.Value;
                        var envItem = new envVars { vName = name, vVal = value };
                        envVar_dg.Items.Add(envItem);
                        envList.Add(envItem);
                    }
                }
                optSl = true;
            
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
            }
            /*EnviropmentVars<--*/

            /*OS-->*/
            if (optOS.IsChecked == true)
            {
                optSl = true;
                var osInfo = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (var item in osInfo.Get())
                {
                    var osBN = new OS { osName = "BuildNumber", osVal = item["BuildNumber"].ToString() };
                    OS_dg.Items.Add(osBN);
                    osList.Add(osBN);
                    var osBT = new OS { osName = "BuildType", osVal = item["BuildType"].ToString() };
                    OS_dg.Items.Add(osBT);
                    osList.Add(osBT);
                    var osCap = new OS { osName = "Caption", osVal = item["Caption"].ToString() };
                    OS_dg.Items.Add(osCap);
                    osList.Add(osCap);
                    var osCds = new OS { osName = "CodeSet", osVal = item["CodeSet"].ToString() };
                    OS_dg.Items.Add(osCds);
                    osList.Add(osCds);
                    var osContC = new OS { osName = "CountryCode", osVal = item["CountryCode"].ToString() };
                    OS_dg.Items.Add(osContC);
                    osList.Add(osContC);
                    var osCTZ = new OS { osName = "CurrentTimeZone", osVal = item["CurrentTimeZone"].ToString() };
                    OS_dg.Items.Add(osCTZ);
                    osList.Add(osCTZ);
                    var osInsD = new OS { osName = "InstallDate", osVal = item["InstallDate"].ToString() };
                    OS_dg.Items.Add(osInsD);
                    osList.Add(osInsD);
                    var osLocDT = new OS { osName = "LocalDateTime", osVal = item["LocalDateTime"].ToString() };
                    OS_dg.Items.Add(osLocDT);
                    osList.Add(osLocDT);
                    var osMan = new OS { osName = "Manufacturer", osVal = item["Manufacturer"].ToString() };
                    OS_dg.Items.Add(osMan);
                    osList.Add(osMan);
                    var osNm = new OS { osName = "Name", osVal = item["Name"].ToString() };
                    OS_dg.Items.Add(osNm);
                    osList.Add(osNm);
                    var osLan = new OS { osName = "OSLanguage", osVal = item["OSLanguage"].ToString() };
                    OS_dg.Items.Add(osLan);
                    osList.Add(osLan);
                    var osSN = new OS { osName = "SerialNumber", osVal = item["SerialNumber"].ToString() };
                    OS_dg.Items.Add(osSN);
                    osList.Add(osSN);
                }
            }
            /*OS--<*/

            /*hardware-->*/
            if (optHardware.IsChecked == true)
            {
                optSl = true;

                //baseBoard-->
                int hwMBoard_cnt = 0;
                var hwMBoard = new ManagementObjectSearcher("select * from Win32_BaseBoard");
                foreach (var item in hwMBoard.Get())
                {
                    hwMBoard_cnt++;
                    string mbManuf = "-";
                    if (item["Manufacturer"] != null)
                    {
                        mbManuf = item["Manufacturer"].ToString();
                    }
                    var hwMBoardManuf = new Hardware { paramName = "motherBoard-Manufacturer",
                        paramVal= mbManuf,  hwNum=hwMBoard_cnt.ToString() };
                    hardware_dg.Items.Add(hwMBoardManuf);
                    hwList.Add(hwMBoardManuf);

                    string mbModel = "-";
                    if (item["Model"] != null)
                    {
                        mbModel = item["Model"].ToString();
                    }
                    var hwMBoardModel = new Hardware { paramName = "motherBoard-Model",
                        paramVal= mbModel,  hwNum=hwMBoard_cnt.ToString() };
                    hardware_dg.Items.Add(hwMBoardModel);
                    hwList.Add(hwMBoardModel);


                }

                //processor-->
                int hwProcessor_cnt = 0;
                var hwProcessor = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (var item in hwProcessor.Get())
                {
                    hwProcessor_cnt++;
                    string hwProcName_str = "-";
                    if (item["Name"] != null)
                    {
                        hwProcName_str = item["Name"].ToString();
                    }
                    var hwProcName = new Hardware { paramName = "processor", paramVal = hwProcName_str, hwNum= hwProcessor_cnt.ToString() };
                    hardware_dg.Items.Add(hwProcName);
                    hwList.Add(hwProcName);
                }
                //graphics-->
                int hwGpu_cnt = 0;
                var hwGpu = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (var item in hwGpu.Get())
                {
                    hwGpu_cnt++;
                    string hwGpuName_str = "-";
                    if (item["Name"] != null)
                    {
                        hwGpuName_str = item["Name"].ToString();
                    }
                    var hwGpuName = new Hardware { paramName = "graphics", paramVal = hwGpuName_str, hwNum= hwGpu_cnt.ToString() };
                    hardware_dg.Items.Add(hwGpuName);
                    hwList.Add(hwGpuName);
                }
                //audio-->
                int hwSound_cnt = 0;
                var hwSound = new ManagementObjectSearcher("select * from Win32_SoundDevice");
                foreach (var item in hwSound.Get())
                {
                    hwSound_cnt++;
                    string hwAudioName_str = "-";
                    if (item["Name"] != null)
                    {
                        hwAudioName_str = item["Name"].ToString();
                    }
                    var hwAudioName = new Hardware { paramName = "Audio", paramVal = hwAudioName_str, hwNum= hwSound_cnt.ToString() };
                    hardware_dg.Items.Add(hwAudioName);
                    hwList.Add(hwAudioName);
                }
                //Discs-->
                int hwDiscs_cnt = 0;
                var hwDiscs = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                foreach (var item in hwDiscs.Get())
                {
                    hwDiscs_cnt++;
                    string hwDiscIntf_str = "-";
                    if (item["interfaceType"] != null)
                    {
                        hwDiscIntf_str = item["interfaceType"].ToString();
                    }
                    var hwDiscIntf = new Hardware { paramName = "Disk-interfaceType", paramVal = hwDiscIntf_str, hwNum = hwDiscs_cnt.ToString() };
                    hardware_dg.Items.Add(hwDiscIntf);
                    hwList.Add(hwDiscIntf);

                    string hwDiskModel_str = "-";
                    if (item["Model"] != null)
                    {
                        hwDiskModel_str = item["Model"].ToString();
                    }
                    var hwDiskModel = new Hardware { paramName = "Disk-model", paramVal = hwDiskModel_str, hwNum= hwDiscs_cnt.ToString() };
                    hardware_dg.Items.Add(hwDiskModel);
                    hwList.Add(hwDiskModel);

                    double totalSize = Convert.ToDouble(item["Size"]);
                    var hwDiskSize = new Hardware { paramName = "Disk-size",
                        paramVal = Math.Round((totalSize / 1e9), 2).ToString() + " GB", hwNum = hwDiscs_cnt.ToString()
                    };
                    hardware_dg.Items.Add(hwDiskSize);
                    hwList.Add(hwDiskSize);
                }
                //TotalVisibleMemorySize-->
                var hwRam = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                double ramTotal;
                
                foreach (var item in hwRam.Get())
                {
                    ramTotal = Convert.ToDouble(item["TotalVisibleMemorySize"]);
                    var hwRamValue = new Hardware { paramName = "TotalVisibleMemorySize", paramVal = Math.Round((ramTotal / 1e6), 2).ToString() + " GB", hwNum="-" };
                    hardware_dg.Items.Add(hwRamValue);
                    hwList.Add(hwRamValue);
                }  
                
                int nwNum = 0;
                var osInfo = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter where PhysicalAdapter=true");
                foreach (var item in osInfo.Get())
                {
                    nwNum++;
                    string hwAdaptType_str = "-";
                    if (item["AdapterType"] != null)
                    {
                        hwAdaptType_str = item["AdapterType"].ToString();
                    }
                    var hwAdaptType = new Hardware { paramName = "Adapter-Type", paramVal = hwAdaptType_str, hwNum= nwNum.ToString() };
                    hardware_dg.Items.Add(hwAdaptType);
                    hwList.Add(hwAdaptType);

                    string hwAdaptName_str = "-";
                    if (item["Name"] != null)
                    {
                        hwAdaptName_str = item["Name"].ToString();
                    }
                    var hwAdaptName = new Hardware { paramName = "Adapter-Name", paramVal = hwAdaptName_str, hwNum = nwNum.ToString() };
                    hardware_dg.Items.Add(hwAdaptName);
                    hwList.Add(hwAdaptName);

                    string hwAdaptSpeed_str = "-";
                    if (item["Speed"] != null)
                    {
                        hwAdaptSpeed_str = item["Speed"].ToString();
                    }
                    var hwAdaptSpeed = new Hardware { paramName = "Adapter-Speed", paramVal = hwAdaptSpeed_str, hwNum = nwNum.ToString() };
                    hardware_dg.Items.Add(hwAdaptSpeed);
                    hwList.Add(hwAdaptSpeed);
                }
            }
            /*hardware<--*/

            /*process-->*/
            if (optProcess.IsChecked == true)
            {
                optSl = true;
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
            }


            /*process<--*/

            /*services-->*/
            if (optServices.IsChecked == true)
            {
                optSl = true;
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
            }
            Mouse.OverrideCursor = Cursors.Arrow;
            /*services<--*/
            if (optSl == false)
            {
                MessageBox.Show("select some options first");
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("click load");
            Mouse.OverrideCursor = Cursors.Wait;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                dgClear(true);
                envList.Clear();
                osList.Clear();
                hwList.Clear();
                ProcessLst.Clear();
                ServicesList.Clear();
                var fullPath = openFileDialog.FileName;
                var fName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                expName.Text = fName;
                string diagRes = System.IO.File.ReadAllText(fullPath);
                tDiag = JsonConvert.DeserializeObject<diagList>(diagRes);
                if (tDiag.envList != null)
                {
                    if (tDiag.envList.Count > 0)
                    {
                        foreach (envVars diagEnv in tDiag.envList)
                        {
                            envVar_dg.Items.Add(diagEnv);
                            envList.Add(diagEnv);
                        }
                    }
                }
                if (tDiag.osList != null)
                {
                    if (tDiag.osList.Count > 0)
                    {
                        foreach (OS diagOS in tDiag.osList)
                        {
                            OS_dg.Items.Add(diagOS);
                            osList.Add(diagOS);
                        }
                    }
                }
                if (tDiag.hardwareList != null)
                {
                    if (tDiag.hardwareList.Count > 0)
                    {
                        foreach (Hardware diagHw in tDiag.hardwareList)
                        {
                            hardware_dg.Items.Add(diagHw);
                            hwList.Add(diagHw);
                        }
                    }
                }
                if (tDiag.procList != null)
                {
                    if (tDiag.procList.Count > 0)
                    {
                        foreach (pProcess diagP in tDiag.procList)
                        {
                            Process_dg.Items.Add(diagP);
                            ProcessLst.Add(diagP);
                        }
                    }
                }
                if (tDiag.srvList != null)
                {
                    if (tDiag.srvList.Count > 0)
                    {
                        foreach (sService diagS in tDiag.srvList)
                        {
                            Services_dg.Items.Add(diagS);
                            ServicesList.Add(diagS);
                        }
                    }
                }
               
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("click check");
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://rightjoint.ru/win-pc-info");
        }
    }

    public class diagList
    {
        public string fName { get; set; }
        public List<envVars> envList { get; set; }
        public List<OS> osList { get; set; }
        public List<Hardware> hardwareList { get; set; }
        public List<pProcess> procList { get; set; }
        public List<sService> srvList { get; set; }
    }

    public class envVars
    {
        public string vName { get; set; }
        public string vVal { get; set; }
    }


    public class OS
    {
        public string osName { get; set; }
        public string osVal { get; set; }
    }

    public class Hardware
    {
        public string paramName { get; set; }
        public string hwNum { get; set; }
        public string paramVal { get; set; }
    }

    public class pProcess
    {
        public string pName { get; set; }
        public string PID { get; set; }
        public string pPath { get; set; }
    }

    public class sService
    {
        public string sName { get; set; }
        public string sDName { get; set; }
        public string sSTName { get; set; }
        public string sDescr { get; set; }
        public string sPath { get; set; }
    }
}
