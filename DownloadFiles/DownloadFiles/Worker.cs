using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFiles
{
    public class Worker
    {
        private const int defaultTimeout = 5000; //in millisec
        private const string defaultJobfile = "DownloadJobs.json";
        private const string defaultPathToSave = @"C:\temp";

        private string _jobfile = defaultJobfile;
        private string _pathToSave = defaultPathToSave;
        private int _timeout = defaultTimeout;
        private readonly string[] _args;
        public Worker(string [] args ) 
        {
            _args = args;
        }
        static void PrintUsage(bool readLine)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("Path=path to store the downloaded files");
            Console.WriteLine("Timeout=xxx, default timeout is 5 seconds if there is no receiving packet");
            Console.WriteLine("Jobfile=job json file name. Default is DownloadJobs.json");
            if (readLine)
            {
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(Environment.NewLine);
            }
        }
        public void ArgumentProcessor()
        {
            var parsedArgs = _args
                .Select(s => s.Split(new[] { '=' }))
                .ToDictionary(s => s[0].ToLower(), s => s[1]);
            if (parsedArgs.ContainsKey("path"))
            {
                _pathToSave = parsedArgs["path"];
            }
            if (parsedArgs.ContainsKey("timeout"))
            {
                if (!Int32.TryParse(parsedArgs["timeout"], out _timeout))
                {
                    Console.WriteLine("Invalid timeout value");
                    PrintUsage(true);
                    return;
                }
            }
            if (parsedArgs.ContainsKey("jobfile"))
            {
                _jobfile = parsedArgs["jobfile"];
            }
            PrintUsage(false);
        }
        public void Execute()
        {
            try
            {
                ArgumentProcessor();
                Jobs jobs = new Jobs(_jobfile);
                jobs.LoadJson();
                DownloaderFactory downloaderFactory = new DownloaderFactory(_pathToSave, _timeout);
                foreach (Job job in jobs.Next())
                {
                    bool success = downloaderFactory.Create(job).StartDownload();
                    if (!success)
                    {
                        Console.WriteLine("Missing file: " + job.URI);
                        Console.WriteLine("Press Enter to exit.");
                        Console.ReadLine();
                    }
                }
                Console.WriteLine("All files has been processed. Press Enter to exit.");
                Console.ReadLine();
            }
            catch (DownloadFilesException de)
            {
                Console.WriteLine("Exception: " + de.Message);
            }
        }
    }
}
