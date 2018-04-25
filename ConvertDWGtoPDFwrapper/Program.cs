using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConvertDWGtoPDFwrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            int timemask = 0;
            DateTime currDate = DateTime.Now;
            FileInfo _file = null;
            FileInfo _tmpfile = null;
            FileInfo _outfile = null;
            
            DirectoryInfo thisDir = null;
            DateTime oldDate = currDate.AddMinutes(timemask);

            string path = @"c:\test";
         
            Process converter = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();


            startInfo.FileName = @"C:\Program Files (x86)\Acme CAD Converter\aconv.exe";

            
            

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Directory not exist.");
                return;
            }

            try
            {
                bool convert = false;
                List<string> files = Directory.EnumerateFiles(@"c:\test", "*.dwg*", SearchOption.AllDirectories).ToList<string>();
                Console.WriteLine("Searching files...");
                foreach (string currentFile in files)
                {
                    convert = false;
                    try
                    {
                        
                        _file = new FileInfo(currentFile);

                        Console.WriteLine("Touch file..." + _file.FullName);
                        if (_file.LastWriteTime < oldDate)
                        {
                            Console.WriteLine("Found new file: " + _file.FullName);

                            thisDir = new DirectoryInfo(_file.DirectoryName);
                            thisDir = thisDir.CreateSubdirectory("pdf");
                            _outfile = new FileInfo(Path.ChangeExtension(thisDir.FullName + "\\" +_file.Name, ".pdf"));

                            Console.WriteLine("Check if PDF exist: " + _outfile.FullName);
                            if (_outfile.Exists)
                            {
                                Console.WriteLine("PDF exist. Return: " + _outfile.FullName);
                                if (_outfile.Length != 0)
                                    continue;
                                convert = true;
                            }
                            else
                                convert = true;

                            if(convert)
                            {
                                Console.WriteLine("Trying converting to " + _outfile.FullName);
                                
                                try
                                {
                                    
                                    startInfo.Arguments = @"/r /e /ls /ad /i /a -2 /layer /f 104  /layerop ""C:\Program Files (x86)\Acme CAD Converter\layers.ini"" " +  '"' + _file.FullName + '"';
                                    startInfo.CreateNoWindow = true;
                                    startInfo.UseShellExecute = false;
                                    startInfo.RedirectStandardOutput = true;
                                    startInfo.RedirectStandardError = true;
                                    converter.StartInfo = startInfo;
                                   

                                    converter.Start();
                                    string output = converter.StandardError.ReadToEnd();
                                    Console.WriteLine(startInfo.Arguments);
                                    Console.WriteLine(output);
                                    converter.WaitForExit(300000);

                                    _tmpfile = new FileInfo(Path.ChangeExtension(_file.FullName, ".pdf"));
                                    File.Move(_tmpfile.FullName, _outfile.FullName);
                                    _outfile.Refresh();
                                    if (_outfile.Exists && _outfile.Length != 0)
                                    {
                                        Console.WriteLine("DWG converted: " + _outfile.FullName + " size: " + _outfile.Length);
                                    }
                                    else
                                        Console.WriteLine("DWG not converted: " + _outfile.FullName);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                               
                                
                               // Console.WriteLine(startInfo.Arguments);
                            }

                        }
                        
                    }
                    catch (IOException)
                    {
                        Console.Write("Can not access to file" + currentFile);
                    }

                  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            Console.ReadKey();

        }



        
    }
}
