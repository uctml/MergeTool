using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MergeTool
{
    internal class CommandLine
    {
        private Process Cmd = null;
        private int WaittingSec = 10000;

        public void Init()
        {
            Cmd = new Process();
            Cmd.StartInfo.FileName = "cmd.exe";
            Cmd.StartInfo.RedirectStandardInput = true;
            Cmd.StartInfo.RedirectStandardOutput = true;
            Cmd.StartInfo.UseShellExecute = false;
        }

        public void SetDestDirectory(string Path)
        {
            Cmd.StartInfo.WorkingDirectory = Path;
        }

        public int Execute(string ExecuteMessage)
        {
            try
            {
                Cmd.Start();
                Cmd.StandardInput.WriteLine(ExecuteMessage);
                Cmd.StandardInput.Flush();
                Cmd.StandardInput.Close();
                Cmd.WaitForExit(WaittingSec);
            }
            catch(Exception ErrorException)
            {
                Console.WriteLine(ErrorException.Message);
                return Cmd.ExitCode;
            }
            if (Cmd.HasExited == false)
            {
                Cmd.Kill();
            }
            return Cmd.ExitCode;
        }

        public string ReadToEnd()
        {
            return Cmd.StandardOutput.ReadToEnd();
        }
    }
}
