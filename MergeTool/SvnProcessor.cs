using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using MergeTool;

namespace MergeTool
{
    internal class SvnProcessor
    {

        // 마일스톤과 관련된 로그를 출력
        // SrcPath: 원본 경로
        // StartRevision : 시작 Revision
        // EndRevision : 마지막 Revision
        // MileStoneList: 마일 스톤 리스트
        // 수정사항 : Start & End Revision 없어도 동작하도록 + Date || Limit 로도 동작하도록
        public void ShowTaggedLog(string SrcPath, long StartRevision, long EndRevision, List<string> MileStoneList)
        {
            List<SvnLogEventArgs> TaggedLogList = SearchLogsByMileStones(SrcPath, StartRevision, EndRevision, MileStoneList);
            if(TaggedLogList == null)
            {
                Console.WriteLine("MileStoneLoading Fail");
                return;
            }
            foreach (SvnLogEventArgs TaggedLog in TaggedLogList)
            {
                Console.WriteLine("Revision : " + TaggedLog.Revision);
                Console.WriteLine("Comment : " + TaggedLog.LogMessage);
            }
        }

        // 마일스톤과 관련된 모든 리비전을 원본(SrcPath)에서 대상(DstPath)으로 머지 후 커밋함
        // Srcpath : 원본 Path
        // DstPath : 대상 Path
        // MergedMsg : 커밋시의 접두문
        // ConflictSolve : Conflict시의, 해결방안
        // StartRevision : 시작 Revision
        // EndRevision : 마지막 Revision
        // MileStoneList : 태깅된 마일스톤List
        // CommitFlag : Commit 실행여부
        public void MergeAndCommitByMileStoneList(string SrcPath, string DstPath, string MergedMsg, WorkOutConflict ConflictSolve, long StartRevision, long EndRevision, List<string> MileStoneList, bool CommitFlag)
        {
            List<SvnLogEventArgs> TaggedLogList = SearchLogsByMileStones(SrcPath, StartRevision, EndRevision, MileStoneList);
            if(TaggedLogList == null)
            {
                Console.WriteLine("MileStoneLoading Fail");
                return;
            }
            foreach (SvnLogEventArgs TaggedLog in TaggedLogList)
            {
                SvnRevisionRange Range = new SvnRevisionRange(TaggedLog.Revision - 1, TaggedLog.Revision);
                SvnCommitArgs CommitArg = new SvnCommitArgs();
                CommitArg.LogMessage = MergedMsg + " from : " + SrcPath + " Revision : " + TaggedLog.Revision + " Msg : " + TaggedLog.LogMessage;
                if(DoMerge(DstPath, SrcPath, Range) == true)
                {
                    if (CommitFlag == true)
                    {
                        if (DoCommit(DstPath, TaggedLog, MakeCommitLog(MergedMsg, SrcPath, TaggedLog)) == true)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (ConflictSolve == WorkOutConflict.None)
                {
                    // exit
                    return;
                    // ignore
                    // continue;
                }

                switch (ConflictSolve)
                {
                    case WorkOutConflict.AutoSolve:
                        // 고민중...
                        // 적당한 Revision으로 Revert 후, Continue
                        // 에러가 지속적으로 발생한다면 return도 필요해 보임
                        break;
                    case WorkOutConflict.Ignore:
                        break;
                    case WorkOutConflict.Stop:
                        return;
                    default:
                        return;
                }
            }
        }

        // 체리픽 머지
        // DstPath : 머지를 실행할 Path
        // SrcPath : 머지의 원본 Path
        // Range : Merge Revision Range(Start to End)
        public bool DoMerge(string DstPath, string SrcPath, SvnRevisionRange Range)
        {
            SvnMergeArgs MergeArg = new SvnMergeArgs();
            MergeArg.DryRun = true;
            Console.WriteLine("Merge " + Range.EndRevision + " Start");
            using (SvnClient Client = new SvnClient())
            {
                try
                {
                    Client.Merge(DstPath, SrcPath, Range, MergeArg);
                    Client.Merge(DstPath, SrcPath, Range);
                }
                catch (SvnException Error)
                {
                    Console.WriteLine("Merge " + Range.EndRevision + " Is Error");
                    Console.WriteLine(Error.Message);
                    Console.WriteLine(Error.InnerException);
                    return false;
                }
            }
            Console.WriteLine("Merge " + Range.EndRevision + " Is Fin");
            return true;
        }

        // Commit 실행
        // DstPath : 커밋을 실행할 Path
        // TaggedLog : 커밋을 실행할 Log의 Arg
        // CommitLog : 커밋시의 입력할 커밋로그
        public bool DoCommit(string DstPath, SvnLogEventArgs TaggedLog, string CommitLog)
        {
            SvnMergeRange Range = new SvnMergeRange(TaggedLog.Revision - 1, TaggedLog.Revision, false);
            SvnCommitArgs CommitArg = new SvnCommitArgs();
            CommitArg.LogMessage = CommitLog;
            Console.WriteLine("Commit " + TaggedLog.Revision + " Start");
            using (SvnClient Client = new SvnClient())
            {
                try
                {
                    Client.Commit(DstPath, CommitArg);
                }
                catch (SvnException Error)
                {
                    Console.WriteLine("Commit " + TaggedLog.Revision + " Is Error");
                    Console.WriteLine(Error.Message);
                    Console.WriteLine(Error.InnerException);
                    return false;
                }
            }
            Console.WriteLine("Commit " + TaggedLog.Revision + " Is Fin");
            return true;
        }

        // 커밋 로그를 생성한다.
        // 커밋로그는 MegedMsg, SrcPath, Revision, CommitLog(TaggedLog.LogMessage)로 구성된다.
        public string MakeCommitLog(string MergedMsg, string SrcPath, SvnLogEventArgs TaggedLog)
        {
            string[] SrcPathBySlash = SrcPath.Split("/");
            string CommitMsg = MergedMsg + " from : " + SrcPathBySlash.GetValue(SrcPathBySlash.Length) + " Revision : " + 
                TaggedLog.Revision + " Msg : " + TaggedLog.LogMessage;
            return CommitMsg;
        }

        // 마일스톤 태깅된 로그만 찾는다.
        // SrcPath : 원본 Path
        // StartRevision : 시작 Revision
        // EndRevision : 마지막 Revision
        // MileStoneList : 태킹된 마일스톤 리스트
        private List<SvnLogEventArgs> SearchLogsByMileStones(string SrcPath, long StartRevision, long EndRevision, List<string> MileStoneList)
        {
            Collection<SvnLogEventArgs> TotalLogList = new Collection<SvnLogEventArgs>();
            SvnRevisionRange Range = new SvnRevisionRange(StartRevision, EndRevision);
            SvnLogArgs LogArgs = new SvnLogArgs(Range);
            Uri SrcUri = new Uri(SrcPath);

            using (SvnClient Client = new SvnClient())
            {
                try
                {
                    Client.GetLog(SrcUri, LogArgs, out TotalLogList);
                }
                catch (SvnException Error)
                {
                    Console.WriteLine(Error.Message);
                    Console.WriteLine(Error.InnerException);
                    return null;
                }
            }

            List<SvnLogEventArgs> TaggedLogList = new List<SvnLogEventArgs>();
            if (MileStoneList.Count > 0)
            {
                foreach (SvnLogEventArgs EachLog in TotalLogList)
                {
                    foreach (string MileStone in MileStoneList)
                    {
                        if (EachLog.LogMessage.Contains(MileStone))
                        {
                            TaggedLogList.Add(EachLog);
                            break;
                        }
                    }
                }
            }
            else
            {
                TaggedLogList = TotalLogList.Cast<SvnLogEventArgs>().ToList();
            }
            return TaggedLogList;
        }
    }
}
