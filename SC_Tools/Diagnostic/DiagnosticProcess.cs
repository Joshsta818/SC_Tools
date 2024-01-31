using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using SC_Tools.MultiUse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SC_Tools.Views;

namespace SC_Tools.Diagnostic
{
    public class DiagnosticProcess
    {

        public static Logger diagLog = LogManager.GetLogger("DiagLogs");
        SharedProcesses sharedProcesses = new SharedProcesses(diagLog);
        bool is64Bit = Environment.Is64BitOperatingSystem;

        #region Process Portion
        public int ProxyCheck()
        {
            sharedProcesses.ProxyCheck();
            return 33;
        }

        public int ExecuteRK()
        {
            sharedProcesses.ExecuteRogueKiller(is64Bit);
            return 66;
        }

        public int ExecueteADW()
        {
            sharedProcesses.ExecuteADWCleaner();
            return 100;
        }

        #endregion

        #region Cleanup portion
        //Remove dirs
        //Getsys Info
        public int RemoveDirectories()
        {
            //Build list of Directories
            List<string> dirListings = new List<string>();
            dirListings.Add(@"C:\sc_tools");
            dirListings.Add(@"C:\programdata\RogueKiller");
            dirListings.Add(@"C:\AdwCleaner");
            dirListings.Add(@"C:\sccleaner");
            dirListings.Add(@"C:\totaldiag");

            try
            {
                diagLog.Info("Starting Remove Directory Calls");
                //For each item in the list delete
                foreach (var item in dirListings)
                {
                    diagLog.Info("File being worked on:  {0}", item);
                    if (Directory.Exists(item))
                    {
                        diagLog.Info("{0} exists", item);
                        Directory.Delete(item, true);
                        diagLog.Info("{0} deleted", item);
                    }
                }
            }
            catch (Exception e)
            {
                diagLog.Error("Exception Detected: {0}", e.ToString());
            }

            return 50;
        }

        public int GetSysInfo()
        {
            this.sharedProcesses.GetSystemInfo();
            return 100;
        }
        #endregion
    }
}
