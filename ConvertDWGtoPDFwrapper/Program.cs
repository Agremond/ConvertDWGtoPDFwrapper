using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConvertDWGtoPDFwrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            int timemask = 0;
            DateTime currDate = DateTime.Now;

            DateTime oldDate = currDate.AddMinutes(timemask);

            string path = @"c:\test";
            if(!Directory.Exists(path))
            {
                Console.Write("Directory not exist.");
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
                        
                        FileInfo _file = new FileInfo(currentFile);
                        
                     
                        if (_file.LastWriteTime < oldDate)
                        {
                            Console.WriteLine("File is found: " + _file.FullName);
                            FileInfo _outfile = new FileInfo(Path.ChangeExtension(_file.FullName, ".pdf"));
                            DirectoryInfo thisDir = new DirectoryInfo(_file.DirectoryName);
                            thisDir = thisDir.CreateSubdirectory("pdf");

                            if (_outfile.Exists)
                            {
                                if (_outfile.Length != 0)
                                    continue;
                                convert = true;
                            }
                            else
                                convert = true;

                            if(convert)
                            {
                                ;
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
