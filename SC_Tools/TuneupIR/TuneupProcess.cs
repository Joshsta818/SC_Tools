using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SC_Tools.Services;
using NLog;
using SC_Tools.MultiUse;

namespace SC_Tools.TuneupIR
{
    public class TuneupProcess
    {
        bool is64Bit = Environment.Is64BitOperatingSystem;
        public string bbRemoved = string.Empty;
        public static Logger tuneupLog = LogManager.GetLogger("TuneupLogs");
        SharedProcesses sharedProcesses = new SharedProcesses(tuneupLog);
        string extraCaseNotes = string.Empty;


        public void PreTuneUpChecks()
        {
            PreTuneUpCleanup();
            GetSystemInfo();
        }



        #region Pre-Tuneup Methods
        /// <summary>
        /// Runs method that takes care of all pretuneup checksincluding
        /// Cleanup old scan directories
        /// Service states
        /// Check if sit is installed
        /// cleanup old log paths
        /// </summary>
        private void PreTuneUpCleanup()
        {
            //CleanPastCaseNotes();
            //RemoveDirectories();
            //SCCleanerReplacement();
        }

        public int CleanPastCaseNotes()
        {

            try
            {
                List<string> filesToClean = new List<string>();

                string filePath = Path.GetDirectoryName(AppContext.BaseDirectory);
                filePath = Path.Combine(filePath, "Data", "SysInfoOutput.txt");
                filesToClean.Add(filePath);
                filePath = Path.GetDirectoryName(AppContext.BaseDirectory);
                filePath = Path.Combine(filePath, "Data", "CaseNotes.txt");
                filesToClean.Add(filePath);

                foreach (string s in filesToClean)
                {
                    if (File.Exists(s))
                    {
                        File.WriteAllText(s, string.Empty);
                        tuneupLog.Info(s + " current file being worked on");
                    }
                }
            }
            catch (Exception e)
            {
                tuneupLog.Error("The Following Error has occured: " + e.ToString());
            }
            return 25;

        }

        public  int RemoveDirectories()
        {
            //Build list of Directories
            List<string> dirListings = new List<string>();
            dirListings.Add(@"C:\sc_tools");
            dirListings.Add(@"C:\programdata\RogueKiller");
            dirListings.Add(@"C:\AdwCleaner");
            dirListings.Add(@"C:\sccleaner");

            try
            {
                tuneupLog.Info("Starting Remove Directory Calls");
                //For each item in the list delete
                foreach (var item in dirListings)
                {
                    tuneupLog.Info("File being worked on:  {0}", item);
                    if (Directory.Exists(item))
                    {
                        tuneupLog.Info("{0} exists", item);
                        Directory.Delete(item, true);
                        tuneupLog.Info("{0} deleted", item);
                    }
                }
            }
            catch (Exception e)
            {
                tuneupLog.Error("Exception Detected: {0}", e.ToString());
            }
            return 50;
        }

        public  int SCCleanerReplacement()
        {
            //Run the methods responsible for replacine the SCCleaner batch file. 
            sharedProcesses.StopSitServices();
            sharedProcesses.CleanSITLogs();
            sharedProcesses.DeleteWriteLogs();
            //StopSitServices();
            //CleanSITLogs();
            //DeleteWriteLogs();
            return 75;
        }

        public int GetSystemInfo()
        {
            tuneupLog.Info("Starting System info retrival");
            try
            {
                string output = "";
                var proc = new ProcessStartInfo("cmd.exe", "/c systeminfo")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = @"C:\Windows\System32\"
                };
                Process p = Process.Start(proc);
                p.OutputDataReceived += (sender, args1) => { output += args1.Data + Environment.NewLine; };
                p.BeginOutputReadLine();
                p.WaitForExit();
                Console.WriteLine(output);

                string filePath = Path.GetDirectoryName(System.AppContext.BaseDirectory);
                filePath = System.IO.Path.Combine(filePath, "Data", "SysInfoOutput.txt");

                File.WriteAllText(filePath, output);
            }
            catch (Exception e)
            {
                tuneupLog.Error("Exception detected: {0}", e.ToString());
            }
            return 100;
        }
        #endregion

        #region Tuneup Process Methods
        /*
         * Download the required applications for tuneup/IR Process
         * Create Restore Point
         */

        public int DownlaodTools()
        {
            FTPService.SetupClient();
            FTPService.DownloadDirectory();
            return 8;
        }

        public int DeleteHostFile()
        {
            this.sharedProcesses.DeleteHostFile();
            return 16;
        }

        public int PreCleanRestore()
        {
            this.sharedProcesses.CreateRestoreWMICall("Pre scan restore point");
            return 24;
        }

        public int ExecuteProxyRegFix()
        {
            string filePath = Properties.Resources.WindowsRegFix;

            Process proxyRegFix = Process.Start("regedit.exe", filePath);
            proxyRegFix.WaitForExit();
            return 30;
        }

        public int ProxyCheck()
        {
            this.sharedProcesses.ProxyCheck();
            return 38;
        }

        public int BleachBit()
        {
            string args = @"e C:\SCTools2021\BleachBit\BleachBit.zip -oC:\SCTools2021\BleachBit -y";
            ExtractFiles(args);
            extraCaseNotes += ExecuteBleachBit();
            return 46;
        }

        private void ExtractFiles(string arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\SCTools2021\7Zip\7za.exe";
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }

        private string ExecuteBleachBit()
        {
            string removed = string.Empty;
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\SCTools2021\BleachBit\bleachbit_console.exe";
            proc.StartInfo.Arguments = @"--all-but-warning --clean";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            string[] stringseperators = new string[] { "\r\n" };
            string[] lines = output.Split(stringseperators, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Contains("Disk space recovered:"))
                {
                    removed += line;
                }
            }
            proc.WaitForExit();
            return removed;
        }

        public int ExecuteCleanup()
        {
            Process.Start(@"C:\SCTools2021\Windows Cleanup\cleanup.exe");
            return 54;
        }

        public int PreRKRestore()
        {
            this.sharedProcesses.CreateRestoreWMICall("Pre-RK Restore");
            return 62;
        }

        public int ExecuteRK()
        {
            this.sharedProcesses.ExecuteRogueKiller(is64Bit);
            return 70;
        }

        public int OpenAppWiz()
        {
            this.sharedProcesses.OpenAppWiz();
            return 78;
        }

        public int ExportSITRegistries()
        {
            if (is64Bit)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "reg";
                //proc.StartInfo.Arguments = @"export Hkey_local_Machine\Software\Wow6432Node\SecureIT\ C:\SCtools2021\export.txt";
                proc.StartInfo.Arguments = @"export HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\JosephTest C:\SCtools2021\export.txt";
                proc.Start();
                proc.WaitForExit();
            }
            else
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "reg";
                //proc.StartInfo.Arguments = @"export Hkey_local_Machine\Software\SecureIT\ C:\SCtools2021\export.txt";
                proc.StartInfo.Arguments = @"export HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\JosephTest C:\SCtools2021\export.txt";
                proc.Start();
                proc.WaitForExit();
            }
            return 85;
        }

        public int ExecuteADW()
        {
            this.sharedProcesses.ExecuteADWCleaner();
            return 100;
        }
        #endregion
    }
}
