using System;
using System.Linq;

namespace DownloadFiles
{
    class Program
    {
         static void Main(string[] args)
        {
            new Worker(args).Execute();
        }
    }
}
