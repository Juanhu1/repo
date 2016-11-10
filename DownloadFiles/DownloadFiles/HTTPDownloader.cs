using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFiles
{
    public class HTTPDownloader:WebClientDownloader
    {
/*        public HTTPDownloader(string URL, string fullPathWhereToSave, int timeout, string Name = null, string Password = null)
            : base(URL, fullPathWhereToSave, timeout, Name, Password)
 */
        public HTTPDownloader(Job job, string fullPathWhereToSave, int timeout)
            : base(job, fullPathWhereToSave, timeout)
        {
        }
    }
}
