using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Renci.SshNet.Common;

namespace DownloadFiles
{
    public class SFTPDownloader : DownloadAbstract
    {
        private const int SLEEPINGTIME = 500 ;
        private readonly string host="", remoteFile="";
        private readonly int port = 22;
        private SftpDownloadAsyncResult DownloadResult { get; set; }
        public SFTPDownloader(Job job, string fullPathWhereToSave, int timeout)
            : base(job, fullPathWhereToSave, timeout)
//        public SFTPDownloader(string URL, string fullPathWhereToSave, int timeout, string Name = null, string Password = null)
 //           : base(URL, fullPathWhereToSave, timeout, Name, Password)
        {
            string URL = job.URI;
            int colpos = URL.IndexOf(':');
            int slashpos = URL.IndexOf('/');
            if (colpos < 0)
            {
                host = URL.Substring(0, slashpos);
                remoteFile = URL.Substring(slashpos + 1);
            } 
            else
            {
                Int32.TryParse(URL.Substring(colpos + 1, slashpos - colpos - 1), out port);
                host = URL.Substring(0, colpos);
                remoteFile = URL.Substring(slashpos + 1);
            }
        }
        public override bool StartDownload()
        {
            char [] Phases = { '|', '/', '-', '\\' }; 
            using (var sftp = new SftpClient(host,22, _job.Name, _job.Password ))
            {
                        
                sftp.Connect();
                bool downloadCompleted = false;
                using (Stream file1 = File.OpenWrite(_fullPathWhereToSave))
                {
                    var downloadWaitHandles = new List<WaitHandle>();
                    DownloadResult = sftp.BeginDownloadFile(remoteFile, file1, 
                        null, null) as SftpDownloadAsyncResult;
                    downloadWaitHandles.Add(DownloadResult.AsyncWaitHandle);
                    int Count=0 ;
                    ulong size=0 ;
                    int Phase = 0;
                    while (!downloadCompleted)
                    {
                        //  Assume download completed
                        downloadCompleted = true;
                        var sftpResult = DownloadResult;
                        
                        if (!DownloadResult.IsCompleted)
                        {
                            downloadCompleted = false;
                            Console.Write("\r     -->    {0}", Phases[Phase++ ]);
                            if (Phase > 3)
                            {
                                Phase = 0;
                            }
                            if (size != DownloadResult.DownloadedBytes)
                            {
                                size = DownloadResult.DownloadedBytes;
                                Count = 0;
                            }
                            if (Count > _timeout)
                            {
                                // timeout, stop downloading
                                sftp.EndDownloadFile(DownloadResult);
                                break;
                            }
                        }                        
                        Thread.Sleep(SLEEPINGTIME);
                        Count += SLEEPINGTIME;
                    }
                    Console.WriteLine();
                }
                sftp.Disconnect();
                if (!downloadCompleted )
                {
                    File.Delete(_fullPathWhereToSave);
                    throw new DownloadFilesException( "Timeout while downloading file " + remoteFile);
                }
                if ( DownloadResult.DownloadedBytes==0)
                {
                    File.Delete(_fullPathWhereToSave);
                    throw new DownloadFilesException("File not found: " + remoteFile);
                }
            }
            return true ;
        }
    }
}
