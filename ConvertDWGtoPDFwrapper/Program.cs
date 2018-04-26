using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;

namespace ConvertDWGtoPDFwrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args?.Count() == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Не задано имя каталога.");
                return;
            }
            string path = args[0];

            string to = "asobolev@tdfkm.ru";
            string from = "fs1@tdfkm.ru";
            string server = "s1.kifato.net";
            bool completed = false;

            MailMessage message = new MailMessage(from, to);
            message.BodyEncoding = Encoding.UTF8;

            message.Subject = "Convert_DWG_to_PDF";
            message.Body = Convert.ToString(DateTime.Now) + "\n";
            

            SmtpClient client = new SmtpClient(server);
                      
            int timemask = 0;
            int count = 0;
            DateTime currDate = DateTime.Now;
            FileInfo _file = null;
            FileInfo _tmpfile = null;
            FileInfo _outfile = null;
            
            DirectoryInfo thisDir = null;
            DateTime oldDate = currDate.AddMinutes(timemask);

                  
            Process converter = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();


            startInfo.FileName = @"C:\Program Files (x86)\Acme CAD Converter\aconv.exe";

            
            

            if (!Directory.Exists(path))
            {
                message.Body += "Directory not exist.\n";
                return;
            }

            try
            {
                bool convert = false;
                List<string> files = Directory.EnumerateFiles(@"c:\test", "*.dwg*", SearchOption.AllDirectories).ToList<string>();
                message.Body += "Searching files...\n\n";
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
                            if (_outfile.Exists && _outfile.Length != 0)
                            {
                                Console.WriteLine("PDF exist. Return: " + _outfile.FullName);
                                continue;
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
                                    completed = converter.WaitForExit(300000);
                                    if(!completed)
                                    {
                                        message.Body += count + ". " + _outfile.FullName + "convrter not responding";

                                        continue;
                                    }
                                    _tmpfile = new FileInfo(Path.ChangeExtension(_file.FullName, ".pdf"));
                                    File.Move(_tmpfile.FullName, _outfile.FullName);
                                    _outfile.Refresh();
                                    if (_outfile.Exists && _outfile.Length != 0)
                                    {
                                        message.Body += count + ". " + _outfile.FullName + " ";
                                        message.Body += _outfile.Length + "\n";
                                        Console.WriteLine("DWG converted: " + _outfile.FullName + " size: " + _outfile.Length);
                                    }
                                    else
                                        Console.WriteLine("DWG not converted: " + _outfile.FullName);

                                    count++;
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
            currDate = DateTime.Now;
            message.Body += "\n" + currDate;

            if (count == 0)
                return;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}", ex.ToString());
            }

      //      Console.ReadKey();

        }



        
    }
}
