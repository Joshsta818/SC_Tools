using MahApps.Metro.Controls;
using SC_Tools.Views;
using SC_Tools.Services;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace SC_Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnTuneUp_Click(object sender, RoutedEventArgs e)
        {
            TuneupIRView IRView = new TuneupIRView();
            IRView.ShowDialog();
        }

        private void BtnDiag_Click(object sender, RoutedEventArgs e)
        {
            //var controller = await this.ShowProgressAsync("Please wait...", "Progress message");
            DiagView diagView = new DiagView();
            diagView.ShowDialog();
        }

        private void BtnProd_Click(object sender, RoutedEventArgs e)
        {
            ProductsView prodView = new ProductsView();
            prodView.ShowDialog();
        }
    }
}
