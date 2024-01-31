using Microsoft.Win32;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC_Tools.MultiUse
{
    public class SharedProcesses
    {
        public SharedProcesses(Logger logToUSe)
        {
            this.log = logToUSe;
        }

        Logger log;

        public  void StopSitServices()
        {
            bool isSitInstalled = false;//check if sit is installed

            string sitDirectory = @"C:\Program Files\SecureIT";//set sit directory path
            string scavpath = @"C:\Program Files\SecureIT\bin\scavcontrol.exe";//set scav path

            try
            {
                //Check to see if sit is installed
                if (Directory.Exists(sitDirectory))
                {
                    isSitInstalled = true;//if installed set check bit to true
                    log.Info("Sit directory Detected");
                }
                else
                {
                    isSitInstalled = false;//if not installed set check bit to false
                    Log.Info("Sit directory has not been detected");
                }
                //if Sit is installed stop services
                if (isSitInstalled)
                {
                    Process p = new Process();//create new process
                    p.StartInfo.FileName = scavpath;//set file name for process
                    p.StartInfo.Arguments = "/stop";//set arguments for process
                    p.Start();//execure process request
                }
            }
            catch (Exception e)
            {
                log.Error("Exception Detected: {0}", e.ToString());
            }


        }

        public void CleanSITLogs()
        {
            try
            {
                //Check if dump folders exists
                bool locExists = Directory.Exists(@"C:\Program Files\SecureIT\bin\dumps");
                if (locExists)
                {
                    log.Info(@"the following directory was detected: C:\Program Files\SecureIT\bin\dumps");
                    DirectoryInfo di = new DirectoryInfo(@"C:\Program Files\SecureIT\bin\dumps");
                    foreach (FileInfo file in di.GetFiles())
                    {
                        log.Info("File to be deleted: {0}", file.Name);
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        log.Info("Directory to be deleted: {0}", dir.Name);
                        dir.Delete();
                    }
                }

                //Remove Old Logs
                locExists = Directory.Exists(@"C:\Program Files\SecureIT\bin\logs");//Check to see if logs folder exists
                if (locExists)
                {
                    log.Info(@"Directory detected: C:\Program Files\SecureIT\bin\logs");
                    Directory.Delete(@"C:\Program Files\SecureIT\bin\logs", true);//If exists Delete directory
                    log.Info("Directory Deleted.");
                }

                //Remove new Logs
                locExists = Directory.Exists(@"C:\programdata\secureit\logs");
                if (locExists)
                {
                    log.Info(@"Directory detected: C:\programdata\secureit\logs");
                    Directory.Delete(@"C:\programdata\secureit\logs", true);
                    log.Info("Directory Deleted.");
                }
                locExists = Directory.Exists(@"C:\ProgramData\SecureIT\Dumps");
                if (locExists)
                {
                    log.Info(@"Directory detected: C:\ProgramData\SecureIT\Dumps");
                    Directory.Delete(@"C:\ProgramData\SecureIT\Dumps", true);
                    log.Info("Directory Deleted.");
                }
                locExists = Directory.Exists(@"C:\Windows\Temp\SecureITU");
                if (locExists)
                {
                    log.Info(@"Directory detected: C:\Windows\Temp\SecureITU");
                    Directory.Delete(@"C:\Windows\Temp\SecureITU", true);
                    log.Info("Directory Deleted.");
                }
            }
            catch (Exception e)
            {
                log.Error("Exception Detected: {0}", e.ToString());
            }

        }

        public void DeleteWriteLogs()
        {
            try
            {
                string keyName = @"Software\Wow64Node\SecureIT";//set key location for 64 bit version
                log.Info(@"Key set to Software\Wow64Node\SecureIT");
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))//setup request pathing
                                                                                         //using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        log.Info("writelgs key detected for 64 bit system. will be deleted!");
                        key.DeleteValue("writelogs");
                        log.Info("writelgs key DELETED!");
                    }
                    else
                    {
                        log.Info("No writelogs key detected continuwing.");
                        //TODO add logging that path was not found
                    }
                }
                keyName = @"Software\SecureIT";//setup pathing for 32 bit system
                log.Info(@"Keyname updated to Software\SecureIT");
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, true))
                //using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        log.Info("writelogs key detected for 32 bit system. Will be deleted");
                        key.DeleteValue("writelogs");
                        log.Info("writelogs DELETED");
                    }
                    else
                    {
                        log.Info("No writelogs key detected.");
                        //TODO add logging that path was not found
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception detected: {0}", e.ToString());
            }


        }

        public void GetSystemInfo()
        {
            log.Info("Starting System info retrival");
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
                log.Error("Exception detected: {0}", e.ToString());
            }

        }

        public void DeleteHostFile()
        {
            if (File.Exists(@"C:\Windows\System32\drivers\etc\hosts"))
            {
                File.Delete(@"C:\Windows\System32\drivers\etc\hosts");
            }
        }

        public void ProxyCheck()
        {
            Process.Start("inetcpl.cpl");
            Process regJump = new Process();
            regJump.StartInfo.FileName = @"C:\SCTools2021\RegJump\regjump.exe";
            regJump.StartInfo.Arguments = @"/accepteula HKLM\System\CurrentControlSet\services\tcpip\parameters";
            regJump.Start();
            regJump.WaitForExit();
        }

        public void ExtractFiles(string arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\SCTools2021\7Zip\7za.exe";
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }

        public void ExecuteRogueKiller(bool architure)
        {
            if (architure)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = @"C:\SCTools2021\RogueKiller\RogueKiller_portable64.exe";
                proc.StartInfo.Arguments = @"-autoscan -autoaccepteula -nopop -vtupload /yes -reportformat txt -portable-license C:\SCTools2021\RogueKiller\rk_config.ini";
                proc.Start();
                proc.WaitForExit();
            }
            else
            {
                Process proc = new Process();
                proc.StartInfo.FileName = @"C:\SCTools2021\RogueKiller\RogueKiller_portable32.exe";
                proc.StartInfo.Arguments = @"-autoscan -autoaccepteula -nopop -vtupload /yes -reportformat txt -portable-license C:\SCTools2021\RogueKiller\rk_config.ini";
                proc.Start();
                proc.WaitForExit();
            }
        }

        public void ExecuteADWCleaner()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\SCTools2021\ADW Cleaner\adwcleaner.exe";
            proc.Start();
            proc.WaitForExit();
        }

        public void OpenAppWiz()
        {
            Process.Start("appwiz.cpl");
        }

        public void CreateRestoreWMICall(string restoreName)
        {
            CallWMIMethod.NRTCRestore(restoreName);
        }
    }
}
