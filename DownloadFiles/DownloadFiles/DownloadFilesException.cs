using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFiles 
{
    public class DownloadFilesException : Exception
    {
        public DownloadFilesException(string message) : base(message)
        {
        }
    }
}
