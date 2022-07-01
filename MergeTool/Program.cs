using System;
using MergeTool;
using Microsoft.Extensions.Configuration;

class Program
{
    static public int Main(string[] Args)
    {
        if (Args.Length == 0)
        {
            Console.WriteLine("Please Input Argument");
            return 0;
        }
        else
        {
            Dictionary<string, string> SwitchMappings = new Dictionary<string, string>()
            {
                { "-cmsg", "CommitMsgValue" },
                { "-src", "SrcPathValue" },
                { "-dst", "DstPathValue" },
                { "-sr", "StartRevisionValue" },
                { "-er", "EndRevisionValue" },
                { "--ms", "MileStonesValue" },
            };

            IConfigurationBuilder MainBuilder = new ConfigurationBuilder().AddCommandLine(Args, SwitchMappings);
            IConfigurationRoot MainConfig = MainBuilder.Build();

            DataTemplate DTInstance = DataTemplate.Instance;

            DTInstance.InitSetting(MainConfig["CommitMsgValue"], MainConfig["SrcPathValue"], 
                MainConfig["DstPathValue"], MainConfig["StartRevisionValue"], MainConfig["EndRevisionValue"],
                MainConfig.GetSection("ms").GetChildren().Select(x => x.Value).ToArray());
            SvnProcesser SvnInstance = new SvnProcesser();
            SvnInstance.Init();
            SvnInstance.SetDestDirectory(DTInstance.DstPath);
            SvnInstance.SearchLogsByMileStones(DTInstance.SrcPath, DTInstance.StartRevision, 
                DTInstance.EndRevision, DTInstance.GetMileStoneArray());

            switch (Args[0].ToLower())
            {
                    // log (DstLog)
                case "log":
                    SvnInstance.SvnShowLog(DTInstance.StartRevision);
                    break;
                    // merge
                case "merge":
                    SvnInstance.SvnMerge(DTInstance.SrcPath, DTInstance.DstPath, DTInstance.GetMileStoneArray());
                    break;
                    // merge with commit
                case "commit":
                    SvnInstance.SvnMergeAndCommit(DTInstance.SrcPath, DTInstance.DstPath, DTInstance.CommitMsg, DTInstance.GetMileStoneArray());
                    break;
                    // show merge list
                case "showmergelist":
                    SvnInstance.SvnShowMergeList(DTInstance.SrcPath, DTInstance.GetMileStoneArray());
                    break;
                default:
                    Console.WriteLine("Input Error");
                    break;
            }
        }
        return 0;
    }
}
