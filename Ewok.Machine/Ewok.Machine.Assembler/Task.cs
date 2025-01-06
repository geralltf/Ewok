using System;

using System.Diagnostics;

namespace Ewok.Machine.Assembler {
    public class Task {
        public static string StartTask(string exe,string args,bool wait) {
            Process pAS11M;
            ProcessStartInfo siAS11M;

            pAS11M=new Process();
            siAS11M=new ProcessStartInfo(exe,args);
            // AS11M might not be able to take a bin file directive?
            siAS11M.CreateNoWindow=true;
            siAS11M.RedirectStandardOutput=true;
            siAS11M.RedirectStandardInput=true;
            siAS11M.UseShellExecute=false;
            pAS11M.StartInfo=siAS11M;
            pAS11M.Start();
            //pAS11M.StandardInput.WriteLine("AS11M.EXE \""+asmFile+"\""); // Execute the assembler
            pAS11M.WaitForExit();

            return pAS11M.StandardOutput.ReadToEnd();
        }
    }
}
