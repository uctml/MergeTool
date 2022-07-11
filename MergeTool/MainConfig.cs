using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MergeTool
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
        public string SrcPath { get => InputConfig["SrcPathValue"]; }
        public string DstPath { get => InputConfig["DstPathValue"]; }

        private List<string> MileStoneList = new List<string>();

        public string MergedMsg { get => "Merged Revision(s) "; }

        private WorkOutConflict ToConflictParse { get => (WorkOutConflict)WorkOutConflict.Parse(typeof(WorkOutConflict), InputConfig["ConflictSolveValue"]); }
        public WorkOutConflict ConflictSolve { get => 
                ToConflictParse > WorkOutConflict.None && ToConflictParse < WorkOutConflict.Max ?
                ToConflictParse : WorkOutConflict.None; }

        public long StartRevision { get => long.Parse(InputConfig["StartRevisionValue"]); }
        public long EndRevision { get => long.Parse(InputConfig["EndRevisionValue"]); }

        private List<string> JiraFixVersionList = new List<string>();
        private List<string> JiraEpicKeyList = new List<string>();

        public List<string> GetMileStoneList() { return MileStoneList; }
        public List<string> GetJiraFixVersionList() { return JiraFixVersionList; }
        public List<string> GetJiraEpicKeyList() { return JiraEpicKeyList; }

        IConfigurationRoot InputConfig;

        public MainConfig(string[] Args)
        {
            Dictionary<string, string> SwitchMappings = new Dictionary<string, string>()
            {
                { "-src", "SrcPathValue" },
                { "-dst", "DstPathValue" },
                // AutoSolve || Ignore || Stop
                { "-cr", "ConflictSolveValue" },
                { "-sr", "StartRevisionValue" },
                { "-er", "EndRevisionValue" },
                { "-ms", "MileStonesValue" },
                { "-jfv", "JiraFixVersionValue" },
                { "-jek", "JiraEpicKeyValue" }
            };

            IConfigurationBuilder MainBuilder = new ConfigurationBuilder().AddCommandLine(Args, SwitchMappings);
            InputConfig = MainBuilder.Build();

            if (InputConfig["MileStonesValue"] != null)
            {
                foreach (string MileStone in InputConfig["MileStonesValue"].Split("+"))
                {
                    MileStoneList.Add(MileStone);
                }
            }

            if (InputConfig["JiraFixVersionValue"] != null)
            {
                foreach (string JiraFixVersion in InputConfig["JiraFixVersionValue"].Split("+"))
                {
                    JiraFixVersionList.Add(JiraFixVersion);
                }
            }

            if (InputConfig["JiraEpicKeyValue"] != null)
            {
                foreach (string JiraEpicKey in InputConfig["JiraEpicKeyValue"].Split("+"))
                {
                    JiraEpicKeyList.Add(JiraEpicKey);
                }
            }
        }
    }
}
