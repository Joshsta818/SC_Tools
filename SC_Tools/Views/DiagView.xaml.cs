using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SC_Tools.Diagnostic;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;


namespace SC_Tools.Views
{
    /// <summary>
    /// Interaction logic for DiagView.xaml
    /// </summary>
    public partial class DiagView : MetroWindow
    {

        DiagnosticProcess diagProc = new DiagnosticProcess();
        public DiagView()
        {
            InitializeComponent();
        }

        private async void BtnDiagCleanup_Click(object sender, RoutedEventArgs e)
        {
            BtnDiagCleanup.IsEnabled = false;
            var progress = new Progress<int>(value => diagBar.Value = value);
            await Task.Run(() =>
            {
                ((IProgress<int>)progress).Report(0);
                ((IProgress<int>)progress).Report(diagProc.RemoveDirectories());
                ((IProgress<int>)progress).Report(diagProc.GetSysInfo());
            });
            BtnDiagProcess.IsEnabled = true;
        }

        private async void BtnDiagProcess_Click(object sender, RoutedEventArgs e)
        {
            BtnDiagProcess.IsEnabled = false;
            var progress = new Progress<int>(value => processBar.Value = value);
            await Task.Run(() =>
            {
                ((IProgress<int>)progress).Report(0);
                ((IProgress<int>)progress).Report(diagProc.ProxyCheck());
                Process[] p =Process.GetProcessesByName("regedit");
                p[0].WaitForExit();
                ((IProgress<int>)progress).Report(diagProc.ExecuteRK());
                ((IProgress<int>)progress).Report(diagProc.ExecueteADW());
            });
            BtnDiagProcess.IsEnabled = true;
            BtnDiagCleanup.IsEnabled = true;
        }
    }
}
