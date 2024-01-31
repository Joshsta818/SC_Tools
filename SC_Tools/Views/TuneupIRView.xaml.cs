using MahApps.Metro.Controls;
using SC_Tools.TuneupIR;
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

namespace SC_Tools.Views
{
    /// <summary>
    /// Interaction logic for TuneupIRView.xaml
    /// </summary>
    public partial class TuneupIRView : MetroWindow
    {
        public TuneupIRView()
        {
            InitializeComponent();
        }

        TuneupProcess process = new TuneupProcess();

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            btnCleanup.IsEnabled = false;
            var progress = new Progress<int>(value => preTuneupPrgBar.Value = value);
            await Task.Run(() =>
            {
                ((IProgress<int>)progress).Report(0);
                ((IProgress<int>)progress).Report(process.CleanPastCaseNotes());
                ((IProgress<int>)progress).Report(process.RemoveDirectories());
                ((IProgress<int>)progress).Report(process.SCCleanerReplacement());
                ((IProgress<int>)progress).Report(process.GetSystemInfo());
            });
            btnTuneup.IsEnabled = true;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            btnTuneup.IsEnabled = false;
            var progress = new Progress<int>(value => tuneupPrgBar.Value = value);
            await Task.Run(() =>
            {
                ((IProgress<int>)progress).Report(0);
                ((IProgress<int>)progress).Report(process.DownlaodTools());
                ((IProgress<int>)progress).Report(process.DeleteHostFile());
                ((IProgress<int>)progress).Report(process.PreCleanRestore());
                ((IProgress<int>)progress).Report(process.ExecuteProxyRegFix());
                ((IProgress<int>)progress).Report(process.BleachBit());
                ((IProgress<int>)progress).Report(process.ExecuteCleanup());
                ((IProgress<int>)progress).Report(process.PreRKRestore());
                ((IProgress<int>)progress).Report(process.ExecuteRK());
                ((IProgress<int>)progress).Report(process.OpenAppWiz());
                ((IProgress<int>)progress).Report(process.ExportSITRegistries());
            });
            btnCleanup.IsEnabled = true;
            btnTuneup.IsEnabled = true;
        }
    }
}
