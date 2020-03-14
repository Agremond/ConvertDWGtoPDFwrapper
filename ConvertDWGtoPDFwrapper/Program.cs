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
           

            string to = "sysadm@";
            string from = "fs1@";
            string server = "s1";
            bool completed = false;

            MailMessage message = new MailMessage(from, to);
            message.BodyEncoding = Encoding.UTF8;

            message.Subject = "Convert_DWG_to_PDF";
            message.Body = Convert.ToString(DateTime.Now) + "\n";

            SmtpClient client = new SmtpClient(server);
                      
            int timemask = -125;
            int count = 0;
            DateTime currDate = DateTime.Now;
            FileInfo _file = null;
            FileInfo _tmpfile = null;
            FileInfo _outfile = null;
            
            DirectoryInfo thisDir = null;
            DateTime oldDate = currDate.AddMinutes(timemask);
    
            Process converter = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();


            if (args?.Count() == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                message.Body += "Не задано имя каталога.\n";
                client.Send(message);
                return;
            }
            string path = args[0];

            startInfo.FileName = @"C:\Program Files (x86)\Acme CAD Converter\aconv.exe";

            if (!Directory.Exists(path))
            {
                message.Body += "Directory not exist.\n";
                client.Send(message);
                return;
            }

            try
            {
                bool convert = false;
                List<string> files = Directory.EnumerateFiles(path, "*.dwg*", SearchOption.AllDirectories).ToList<string>();
                message.Body += "Searching files...\n";
                message.Body = Convert.ToString(DateTime.Now) + "\n";
                //            message.Body += "Найдено файлов: " + files.Count + "\n\n";
                foreach (string currentFile in files)
                {
                    convert = false;
                    try
                    {
                        
                        _file = new FileInfo(currentFile);

                        Console.WriteLine("Touch file..." + _file.FullName);
                        if (_file.LastWriteTime > oldDate)
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
                        message.Body += "Can not access to file" + currentFile + "\n";
                    }

                  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (count == 0)
            {
            //    message.Body += "Файлы для пережатия не найдены\n";
                return;
            }

            currDate = DateTime.Now;
            message.Body += "\n" + currDate;

           
                
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
