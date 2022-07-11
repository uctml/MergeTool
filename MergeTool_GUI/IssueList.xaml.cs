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
using MergeTool_GUI;

namespace MergeTool_GUI
{
    /// <summary>
    /// Interaction logic for IssueList.xaml
    /// </summary>
    public partial class IssueList : Window
    {
        private MainConfig Config;
        private JiraProcessor JiraProc = new JiraProcessor();

        public IssueList(Object InputedConfig)
        {
            InitializeComponent();
            InitConfig(InputedConfig);
            InitIssueList();
        }

        public void InitConfig(Object InputedConfig)
        {
            Config = (MainConfig)InputedConfig;
            this.DataContext = Config;
        }

        public void InitIssueList()
        {
            //ListBox IssueListBox = MileStoneListXaml;
            Config.SetAllVersionList(JiraProc.GetAllVersion());
            MileStoneListXaml.ItemsSource = Config.GetAllVersionList();

            //foreach (MileStoneVersion EachVerison in Config.GetAllVersionList())
            //{
            //    string Version = EachVerison.Version;
            //    if (Version.Contains("KR") == false)
            //    {
            //        continue;
            //    }
            //    //CheckBox EachVersionCheckBox = new CheckBox();
            //    //EachVersionCheckBox.Name = EachVerison;
            //    //EachVersionCheckBox.Content = EachVerison;
            //    //EachVersionCheckBox.Height = 30;
            //    //EachVersionCheckBox.Width = 50;

            //    ListBoxItem Item = new ListBoxItem();
            //    Item.Name = Version;
            //    Item.Content = Version;

            //    IssueListBox.Items.Add(Item);
            //}
            return;
        }

        public void MergeStart(object sender, RoutedEventArgs e)
        {
            List<string> ResultString = new List<string>();
            Random iRand = new Random();
            //foreach (string Item in MileStoneListXaml.Items)
            //{
                //if (Item.IsSelected == true)
                //{
                //    //pass
                //    //ResultString.Add(Item.Content.ToString());
                //}
                //else
                //{
                //    //pass
                //}
                //if (iRand.Next() % 2 == 0)
                //{
                //    Item.IsSelected = true;
                //}
                //else
                //{
                //    Item.IsSelected = false;
                //}
            //}
            //return;
        }

        public void SelectMileStone(object sender, RoutedEventArgs e)
        {

        }
    }
}
