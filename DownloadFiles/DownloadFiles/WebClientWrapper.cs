using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DownloadFiles
{
    public class WebClientWrapper:WebClient
    {
        public virtual void wrapper_DownloadFileAsync(Uri address, string fileName)
        {
            DownloadFileAsync(address, fileName) ;
        }
    }
}
