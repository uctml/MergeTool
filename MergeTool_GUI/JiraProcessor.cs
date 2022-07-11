using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.SDK;
using Jira.SDK.Domain;

namespace MergeTool_GUI
{
    internal class JiraProcessor
    {
        private Jira.SDK.Jira JiraObject = new Jira.SDK.Jira();

        // 초기설정 (jira connect)
        public JiraProcessor()
        {
            try
            {
                string JUid = "magatroid@npixel.co.kr";
                string JUPw = "YtnIEhMPF1HkBo4Qe0z5C1FB";
                JiraObject.Connect("https://npixel.atlassian.net", JUid, JUPw);
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
                Console.WriteLine(Error.InnerException);
            }
        }

        public ObservableCollection<string> GetAllVersion()
        {
            ObservableCollection<string> AllVersionList = new ObservableCollection<string>();
            Project GranSagaProject = JiraObject.GetProject("GRAN");
            List<ProjectVersion> GranVersionList = GranSagaProject.ProjectVersions;
            foreach (ProjectVersion GranVersion in GranVersionList)
            {
                //AllVersionList.Add(new MileStoneVersion() { Version = GranVersion.Name });
                AllVersionList.Add(GranVersion.Name);
            }
            return AllVersionList;
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
    }
}
