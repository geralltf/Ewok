using System;
using System.IO;

using Ewok.Machine.Asm;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Ewok.Machine.Asm.S19 {
    public class Program {

        private const string BIN_OP_FILE_EXT=".bin";

        public static void Main(string[] args) {
            /* Emu IDE sends location to ".*.asm.temp" file for this assembler to compile.
            The standard output will be used to notify emu ide should there be any compilation errors.
            If there are no errors, then there is nothing written to the standard output. */

            //args=new string[] {"-asm","C:\\Users\\Morphious\\Desktop\\Ewok.Machine\\Ewok.Machine.Asm.S19\\bin\\Debug\\Test\\test.asm" }; 
            // TODO: REMOVE the above line after testing

            if((args.Length==2)||(args.Length==3)) {
                string arg1CompilationMode;
                string arg2InputASMFile;
                string arg3OutputBinFile;

                if(args.Length==3) {
                    arg3OutputBinFile=args[2];
                }

                if((args.Length==2)||(args.Length==3)) {
                    arg1CompilationMode=args[0];
                    arg2InputASMFile=args[1];

                    switch(arg1CompilationMode) {
                        case "-asm":
                            if(File.Exists(arg2InputASMFile)) {
                                Compile(arg2InputASMFile);
                            }
                            else {
                                Console.WriteLine("Input ASM file could not be found: ("+arg2InputASMFile+")");
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid compilation mode argument specified: ("+arg1CompilationMode+")");
                            break;
                    }
                }
            }
            else {
                Console.WriteLine("Invalid number of arguments specified");
            }
        }

        public static string Compile(string asmFile,out string errors) {
            string s19File=Compile(asmFile);

            Stream s=Console.OpenStandardOutput();
            byte[] buffer;
            buffer=new byte[s.Length];
            s.Read(buffer,0,buffer.Length);
            string outp=System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            errors=outp;

            return s19File;
        }

        public static string Compile(string asmFile) {
            // it is implied that the bin file shall be outputted in the same directory as the asm file 
            // with the same name as the asm file.

            FileInfo fi;
            string binFile;
            string binName;

            fi=new FileInfo(asmFile);

            binName=fi.Name.Replace(fi.Extension,"");
            binFile=fi.Directory.FullName+System.IO.Path.DirectorySeparatorChar+binName+BIN_OP_FILE_EXT;

            return Compile(asmFile,binFile);
        }

        /*  TODO: Stop using AS11M and write a standalone S19 assembler */
        private static string Compile(string asmFile,string binFile) {
            FileInfo fiApp;
            string appPath;
            string output;
            string as11m;

            fiApp=new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            appPath=fiApp.Directory.FullName;
            as11m=appPath+"\\AS11M.EXE";

            preCompilation(asmFile);

            if(!File.Exists(as11m)) {
                File.Copy("C:\\Windows\\AS11M.EXE",as11m); // For unit testing purposes
                if(!File.Exists(as11m)) {
                    throw new Exception("AS11M.EXE not found in: "+appPath);
                }
            }

            output=StartTask("\""+as11m+"\"","\""+asmFile+"\"",true);

            postCompilation(output);

            string fn=(new FileInfo(asmFile)).Name;
            fn=fn.Remove(fn.Length-4,4);
            return appPath+"\\"+fn;
        }

        public static void preCompilation(string asmFile) {
            applyCodeIndentation(asmFile);
        }

        public static void applyCodeIndentation(string asmFile) {
            string[] lines=File.ReadAllLines(asmFile);

            if(lines.Length>0) {
                Regex r;

                r=new Regex("^[a-zA-Z]+[:]",RegexOptions.IgnoreCase);

                for(int i=0;i<lines.Length;i++) {
                    string line=lines[i];
                    
                    if(r.IsMatch(line)) {
                        // not a subroutine label, so dont need to indent this line
                    }
                    else {
                        lines[i]="\t"+line.Trim();
                    }
                }
            }

            File.Delete(asmFile);
            File.WriteAllLines(asmFile,lines);
        }

        private static void postCompilation(string compilerOutput) {
            if(compilerOutput.Contains("Number of errors 0")) {
                // good
            }
            else {
                Console.WriteLine("COMPILATION ERRORS");
                Console.Write(compilerOutput);
            }
        }

        private static string StartTask(string exe,string args,bool wait) {
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