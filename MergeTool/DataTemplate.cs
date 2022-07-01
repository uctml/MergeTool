using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeTool
{
    internal class DataTemplate
    {
        private static readonly DataTemplate _Instance = new DataTemplate();

        public static DataTemplate Instance { get { return _Instance; } }

        public string SrcPath { set; get; }
        public string DstPath { set; get; }
        public string TrunkPath { set; get; }

        private Array MileStoneArray;

        public string CommitMsg { set; get; }

        public long StartRevision { set; get; }
        public long EndRevision { set; get; }

        public Array GetMileStoneArray() { return MileStoneArray; }

        public void InitSetting(string inputCommitMsg, string inputSrcPath, string inputDstPath,
            string inputStartRevision, string inputEndRevision, Array inputMilestones)
        {
            CommitMsg = inputCommitMsg;
            SrcPath = inputCommitMsg;
            DstPath = inputDstPath;
            StartRevision = long.Parse(inputStartRevision);
            EndRevision = long.Parse(inputEndRevision);
            MileStoneArray = inputMilestones;
        }
    }
}
