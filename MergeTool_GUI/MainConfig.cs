using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MergeTool_GUI
{
    enum WorkOutConflict
    {
        None = 0,
        AutoSolve = 1,
        Ignore = 2,
        Stop = 3,
        Max = 4,
    }

    internal class MainConfig
    {
        public string SrcPath { set; get; }
        public string DstPath { set; get; }

        private List<string> MileStoneList = new List<string>();

        public string MergedMsg { get => "Merged Revision(s) "; }

        private WorkOutConflict ToConflictParse { get; }
        public WorkOutConflict ConflictSolve
        {
            get =>
                ToConflictParse > WorkOutConflict.None && ToConflictParse < WorkOutConflict.Max ?
                ToConflictParse : WorkOutConflict.None;
        }

        private ObservableCollection<string> AllVersionCollection;

        public void SetAllVersionList(ObservableCollection<string> InputList) { AllVersionCollection = InputList; }
        public ObservableCollection<string> GetAllVersionList() { return AllVersionCollection; }

        public long StartRevision { get; }
        public long EndRevision { get; }

        private List<string> JiraFixVersionList = new List<string>();
        private List<string> JiraEpicKeyList = new List<string>();

        public List<string> GetMileStoneList() { return MileStoneList; }
        public List<string> GetJiraFixVersionList() { return JiraFixVersionList; }
        public List<string> GetJiraEpicKeyList() { return JiraEpicKeyList; }
    }

    public class MileStoneVersion : INotifyPropertyChanged
    {
        private string InnerVersion;
        public string Version
        {
            get => InnerVersion;
            set
            { 
                InnerVersion = value; 
                INotifyPropertyChanged("Version");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void INotifyPropertyChanged(string PropName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
    }
}
