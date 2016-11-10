using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFiles
{
    public class DownloaderFactory
    {
        private readonly string _pathToSave ;
        private readonly int _timeout;
        public DownloaderFactory(string pathToSave, int timeout)
        {
            _pathToSave=pathToSave ;
            _timeout = timeout;
        }
        public DownloadInterface Create(Job job)
        {
            switch (job.Protocol)
            {
                case Job.eProtocols.eHTTP:
                    return new HTTPDownloader(job, _pathToSave, _timeout );
                case Job.eProtocols.eFTP:
                    return new FTPDownloader(job, _pathToSave, _timeout);
                case Job.eProtocols.eSFTP:
                    return new SFTPDownloader(job, _pathToSave, _timeout);
            }
            return null;
        }

    }
}
