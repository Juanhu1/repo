using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace DownloadFiles
{
    class FTPDownload
    {
        private readonly string _url;
        private readonly string _fullPathWhereToSave;
        private bool _result = false;
        bool _stillDownloading = true;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public FTPDownload(string url, string fullPathWhereToSave)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");

            this._url = url;
            this._fullPathWhereToSave = fullPathWhereToSave;
        }

        public bool StartDownload(int timeout)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));

                if (File.Exists(_fullPathWhereToSave))
                {
                    File.Delete(_fullPathWhereToSave);
                }
                using (WebClient client = new WebClient())
                {
                    var ur = new Uri(_url);
                    // client.Credentials = new NetworkCredential("username", "password");
                    client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    client.DownloadFileCompleted += WebClientDownloadCompleted;
                    Console.WriteLine(@"Downloading file:");
                    client.DownloadFileAsync(ur, _fullPathWhereToSave);
                    while (_stillDownloading)
                    {
                        _stillDownloading = false;
                        _semaphore.Wait(timeout);
                    }
                    return _result && File.Exists(_fullPathWhereToSave);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Was not able to download file!");
                Console.Write(e);
                return false;
            }
            finally
            {
                this._semaphore.Dispose();
            }
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _stillDownloading = true;
            Console.Write("\r     -->    {0}%.", e.ProgressPercentage);
        }

        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            _result = !args.Cancelled;
            if (!_result)
            {
                Console.Write(args.Error.ToString());
            }
            Console.WriteLine(Environment.NewLine + "Download finished!");
            _semaphore.Release();
        }

        public static bool DownloadFile(string url, string fullPathWhereToSave, int timeoutInMilliSec)
        {

            return new FTPDownload(url, fullPathWhereToSave).StartDownload(timeoutInMilliSec);
        }
    }
}
