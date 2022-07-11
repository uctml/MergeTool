using System;
using MergeTool;
using Microsoft.Extensions.Configuration;
using SharpSvn;

class Program
{
    static public int Main(string[] Args)
    {
        if (Args.Length == 0)
        {
            Console.WriteLine("Please Input Argument");
            return 1;
        }
        else
        {
            MainConfig Config = new MainConfig(Args);
            SvnProcessor SvnInstance = new SvnProcessor();
            JiraProcessor JiraInstance = new JiraProcessor();
            JiraInstance.Init();
            JiraInstance.WriteFileWherePickedByMileStone(Config.GetJiraEpicKeyList(), Config.GetJiraFixVersionList());

            switch (Args[0].ToLower())
            {
                    // log
                case "log":
                    SvnInstance.ShowTaggedLog(Config.SrcPath, Config.StartRevision, Config.EndRevision, Config.GetMileStoneList());
                    break;
                    // merge
                case "merge":
                    SvnInstance.MergeAndCommitByMileStoneList(Config.SrcPath, Config.DstPath, Config.MergedMsg, Config.ConflictSolve, Config.StartRevision, Config.EndRevision, Config.GetMileStoneList(), false);
                    break;
                    // merge with commit
                case "commit":
                    SvnInstance.MergeAndCommitByMileStoneList(Config.SrcPath, Config.DstPath, Config.MergedMsg, Config.ConflictSolve, Config.StartRevision, Config.EndRevision, Config.GetMileStoneList(), true);
                    break;
                default:
                    Console.WriteLine("Input Error");
                    break;
            }
        }
        return 0;
     }
}
