using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace DownloadFiles
{

    public abstract class DownloadAbstract:DownloadInterface
    {
        protected readonly Job _job;
        protected readonly string _fullPathWhereToSave;
        protected readonly int _timeout;
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        public DownloadAbstract(Job job, string fullPathWhereToSave, int timeout )
        {
            //if (string.IsNullOrEmpty(URL)) throw new ArgumentNullException("URL");
            if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");
            this._job = job;
            this._timeout = timeout;
            int substract = fullPathWhereToSave[fullPathWhereToSave.Length - 1] == '/' ? 1 : 0;
            string fileName = job.getURL().Substring(job.getURL().LastIndexOf('/') - substract);
            this._fullPathWhereToSave = fullPathWhereToSave + fileName;
        }
        public virtual bool StartDownload()
        {
            return false;
        }
    }
}
