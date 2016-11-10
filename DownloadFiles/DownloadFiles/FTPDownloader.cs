using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFiles
{
    public class FTPDownloader:WebClientDownloader
    {
        public FTPDownloader(Job job, string fullPathWhereToSave, int timeout)
            : base(job, fullPathWhereToSave, timeout)
        {
        }
    }
}
