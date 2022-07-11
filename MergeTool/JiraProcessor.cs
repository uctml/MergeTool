using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;
using Jira.SDK;
using Jira.SDK.Domain;
using RestSharp.Authenticators;

namespace MergeTool
{
    internal class JiraProcessor
    {
        private Jira.SDK.Jira JiraObject = new Jira.SDK.Jira();

        // 초기설정 (jira connect)
        public void Init()
        {
            try
            {
                string JUid = "magatroid@npixel.co.kr";
                string JUPw = "YtnIEhMPF1HkBo4Qe0z5C1FB";
                JiraObject.Connect("https://npixel.atlassian.net", JUid, JUPw);
            }
            catch(Exception Error)
            {
                Console.WriteLine(Error.Message);
                Console.WriteLine(Error.InnerException);
            }
        }

        // EpicKeyList에 입력된 Epic들을 로드
        // FixVersionList에 입력된 Epic들을 로드
        // 로드된 Epic을 파일에 저장
        // EpicKeyList : 입력받은 EpicKeyList
        // FixVersionList : 입력받은 FixVersionList
        public void WriteFileWherePickedByMileStone(List<string> EpicKeyList, List<string> FixVersionList)
        {
            List<Epic> PickedEpicList = new List<Epic>();
            PickUpIssue(EpicKeyList, FixVersionList, PickedEpicList);
            SaveJiraPickedIssues(PickedEpicList);
        }

        // EpicKey & FixVersion을 기준으로 EpicList에 Add 실행
        // EpicKeyList : 입력받은 EpicKeyList
        // FixVersionList : 입력받은 FixVersionList
        // PickedEpic : 저장될 EpicList
        public void PickUpIssue(List<string> EpicKeyList, List<string> FixVersionList, List<Epic> PickedEpicList)
        {
            List<Epic> ProjectEpicList = JiraObject.GetProject("GRAN").GetEpics();
            foreach (Epic ProjectEpic in ProjectEpicList)
            {
                foreach (string EpicKey in EpicKeyList)
                {
                    if (ProjectEpic.Key == EpicKey)
                    {
                        PickedEpicList.Add(ProjectEpic);
                        break;
                    }
                    else
                    {
                        foreach (ProjectVersion Version in ProjectEpic.Fields.FixVersions)
                        {
                            foreach (string Filter in FixVersionList)
                            {
                                // fix version은 여러개가 들어갈 수 있으므로, Add후에는 break실행
                                if (Version.Name == Filter)
                                {
                                    PickedEpicList.Add(ProjectEpic);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        // EpicKeyList의 Epic들을 저장합니다.
        // EpicKeyList : 저장할 Epic의 KeyList
        public void SaveJiraPickedIssues(List<Epic> PickedEpicList)
        {
            List<Project> AllProjectList = JiraObject.GetProjects();
            Project PickedProject = JiraObject.GetProject("GRAN");

            string FilePath = @".\..\" + "JiraIssue" + "_" + DateTime.Now.ToString("yyyy-MM-dd.HHmmss") + ".csv";
            using (var writer = new StreamWriter(FilePath, false, Encoding.UTF8))

            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
            }
        }
    }
}
