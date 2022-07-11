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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MergeTool_GUI;

namespace MergeTool_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainConfig Config = new MainConfig();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Config;
        }

        private void OpenIssueWindow(object sender, RoutedEventArgs e)
        {
            IssueList IssueListWindow = new IssueList(Config);
            IssueListWindow.Show();
        }
    }
}
