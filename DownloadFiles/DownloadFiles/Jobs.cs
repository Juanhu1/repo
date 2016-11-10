using System;
using System.Collections.Generic;
using System.IO ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DownloadFiles
{
    public class Jobs
    {
        Dictionary<string, Job.eProtocols> protocolsDictionary = new Dictionary<string, Job.eProtocols>()
                {
                    { "http", Job.eProtocols.eHTTP },
                    { "ftp",  Job.eProtocols.eFTP  },
                    { "sftp", Job.eProtocols.eSFTP }
                };
        string jobFileName = "DownloadJobs.json";
        List<Job> jobList = new List<Job>();
        public Jobs(string fileName = "")
        {
            if (fileName != "" && fileName!=null)
            {
                jobFileName = fileName;
            }
        }
        public int getJobsCount()
        {
            return jobList.Count;
        }
        public System.Collections.Generic.IEnumerable<Job> Next()
        {
            for (int i = 0; i < jobList.Count; i++)
            {                
                yield return jobList[i];
            }
        }
        private Job.eProtocols getProtocol(string protocol)
        {
            switch (protocol.ToLower())
            {
                case "http": return Job.eProtocols.eHTTP;
                case "ftp" : return Job.eProtocols.eFTP;
                case "sftp": return Job.eProtocols.eSFTP;
                default:
                    throw new DownloadFilesException("Not supported protocol: " + protocol);
            }
        }
        public void ParseJsonstringToList(string json)
        {
            try
            {
                JArray ojArray = JArray.Parse(json);
                foreach (JObject record in ojArray)
                {
                    JToken token = record.GetValue("protocol");
                    if (token==null)
                    {
                        throw new DownloadFilesException("Missing protocol in job file");
                    }
                    string Protocol = record.GetValue("protocol").ToString();
                    token = record.GetValue("uri");
                    if (token == null)
                    {
                        throw new DownloadFilesException("Missing URL in job file");
                    }
                    string URI = record.GetValue("uri").ToString();
                    JToken nameToken = record.GetValue("name");
                    JToken passwordToken = record.GetValue("password");
                    jobList.Add(new Job(getProtocol(Protocol), URI,
                                        nameToken == null ? null : nameToken.ToString(),
                                        passwordToken == null ? null : passwordToken.ToString()));
                }
            }
            catch (JsonReaderException e)
            {
                throw new DownloadFilesException("JSon file error: "+e.Message);
            }
        }
        public void LoadJson()
        {
            string json = "";
            if (!File.Exists(jobFileName))
            {
                throw new DownloadFilesException("Job file not found!");
            }
            using (System.IO.StreamReader input = new StreamReader(jobFileName))
            {
                json = input.ReadToEnd();
            }
            ParseJsonstringToList(json);
        }
    }
}
