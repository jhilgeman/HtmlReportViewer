using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HtmlReportViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = (ViewModel)DataContext;
            webViewBrowser.EnsureCoreWebView2Async();
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if(!string.IsNullOrEmpty(viewModel.Folder))
                {
                    fbd.SelectedPath = viewModel.Folder;
                }

                if(fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                viewModel.LoadHtmlFilesFromFolder(fbd.SelectedPath);
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            webViewBrowser.Source = new Uri(viewModel.SelectedFile.Info.FullName); // NavigateToString(viewModel.SelectedFile?.Contents);
        }
    }
}
