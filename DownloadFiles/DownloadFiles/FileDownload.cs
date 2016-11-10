using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace DownloadFiles
{
    public class FileDownload
    {
        private readonly string _url;
        private readonly string _fullPathWhereToSave;
        private bool _result = false;
        private bool _stillDownloading = true;
        private Exception _error = null;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public FileDownload(string url, string fullPathWhereToSave)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");

            this._url = url;
            int substract = fullPathWhereToSave[fullPathWhereToSave.Length - 1] == '/' ? 1 : 0;
            string fileName = url.Substring(url.LastIndexOf('/')-substract );
            this._fullPathWhereToSave = fullPathWhereToSave+fileName;
        }

        public bool StartDownload(int timeout, string Name=null, string Password=null)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));
                _error = null;
                if (File.Exists(_fullPathWhereToSave))
                {
                    File.Delete(_fullPathWhereToSave);
                }
                using (WebClient client = new WebClient())
                {
                    var ur = new Uri(_url);
                    if (Name != null)
                    {
                        client.Credentials = new NetworkCredential(Name,Password);
                    }
                    client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    client.DownloadFileCompleted += WebClientDownloadCompleted;
                    Console.WriteLine(@"Downloading file: "+_url);
                    client.DownloadFileAsync(ur, _fullPathWhereToSave);
                    while (_stillDownloading)
                    {
                        _stillDownloading = false;
                        _semaphore.Wait(timeout);
                    }
                    if (_error!=null)
                    {
                        File.Delete(_fullPathWhereToSave);
                        throw new DownloadFilesException(_error.Message);
                    }
                    return _result && File.Exists(_fullPathWhereToSave);
                }
            }
            catch (Exception e)
            {
                File.Delete(_fullPathWhereToSave);
                throw new DownloadFilesException("Was not able to download file: " + e.Message );
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
            _result = !args.Cancelled ;
            _error = args.Error;
            if (_result && args.Error==null)
            {
                Console.WriteLine(Environment.NewLine + "Download finished!");
            }
            _stillDownloading = false;
            _semaphore.Release();
        }

        public static bool DownloadFile(string url, string fullPathWhereToSave, int timeoutInMilliSec, string Name, string Password)
        {

            return new FileDownload(url, fullPathWhereToSave).StartDownload(timeoutInMilliSec, Name, Password);
        }
    }
}
