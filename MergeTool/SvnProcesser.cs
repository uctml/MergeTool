using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeTool
{
    internal class SvnProcesser
    {
        private CommandLine Cmd = new CommandLine();
        private SvnClient Client = new SvnClient();

        public void Init()
        {
            Cmd.Init();
        }

        // WorkingDirectory 값 넣기
        public void SetDestDirectory(string Directory)
        {
            Cmd.SetDestDirectory(Directory);
        }

        // Merge
        public void SvnMerge(string SrcPath, string DstPath, Array MileStoneArray)
        {
            foreach (long item in MileStoneArray)
            {
                if (Cmd.Execute("svn merge -c" + item + " " + SrcPath + " " + DstPath) != 0)
                {
                    Console.WriteLine("Merge " + item + " Is Error");
                }
                else
                {
                    Console.WriteLine("Merge " + item + " is Success");
                }
            }
            Console.WriteLine("Merge Finish");
        }

        // Merge 후 Commit
        public void SvnMergeAndCommit(string SrcPath, string DstPath, string CommitMsg, Array MileStoneArray)
        {
            Console.WriteLine("Merge Item Count : " + MileStoneArray.Length);
            foreach (long item in MileStoneArray)
            {
                if (Cmd.Execute("svn merge -c" + item + " " + SrcPath + " " + DstPath) != 0)
                {
                    Console.WriteLine("Merge " + item + " Is Error");
                    continue;
                }
                else
                {
                    Console.WriteLine("Merge " + item + " is Success");
                }
                if (Cmd.Execute("svn commit -m\"" + CommitMsg + "\"") != 0)
                {
                    Console.WriteLine("Commit " + item + " Is Error");
                    continue;
                }
                else
                {
                    Console.WriteLine("Commit " + item + " is Success");
                }
                if (Cmd.Execute("svn update") != 0)
                {
                    Console.WriteLine("Update " + item + " Is Error");
                    continue;
                }
            }
            Console.WriteLine("Commit Finish");
        }

        // Log 출력
        public void SvnShowLog(long StartRevision)
        {
            if (Cmd.Execute("svn log -r " + StartRevision + ":HEAD") != 0)
            {
                Console.WriteLine("show log Error");
            }
            Console.WriteLine(Cmd.ReadToEnd());
        }
        // MergeList출력
        public void SvnShowMergeList(string SrcPath, Array MileStoneArray)
        {
            foreach (long Revision in MileStoneArray)
            {
                if (Cmd.Execute("svn log -r " + Revision + " " + SrcPath) != 0)
                {
                    Console.WriteLine("log : " + Revision + " is Error");
                    continue;
                }
                Console.WriteLine(Cmd.ReadToEnd());
            }
        }

        // 마일스톤 태깅된 로그만 찾는다.
        public Collection<SvnLogEventArgs> SearchLogsByMileStones(string SrcPath, long StartRevision, long EndRevision, Array MileStoneArray)
        {
            Collection<SvnLogEventArgs> LogArgs = new Collection<SvnLogEventArgs>();
            SvnRevisionRange Range = new SvnRevisionRange(StartRevision, EndRevision);
            SvnLogArgs LogParam = new SvnLogArgs(Range);
            Uri SrcUri = new Uri(SrcPath);

            try
            {
                Client.GetLog(SrcUri, LogParam, out LogArgs);
            }
            catch (Exception ErrorException)
            {
                Console.WriteLine("GetLogError");
                Console.WriteLine(ErrorException.Message);
            }

            if (MileStoneArray.Length > 0)
            {
                foreach (SvnLogEventArgs LogItem in LogArgs)
                {
                    foreach (string MileStone in MileStoneArray)
                    {
                        if (!LogItem.LogMessage.Contains(MileStone))
                        {
                            LogArgs.Remove(LogItem);
                        }
                    }
                }
            }
            return LogArgs;
        }
    }
}
