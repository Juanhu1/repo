using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace DownloadFiles
{
    public class WebClientDownloader : DownloadAbstract 
    {
        private bool _result = false;
        private bool _stillDownloading = true;
        private bool _completed = false;
        private Exception _error = null;
        private WebClientWrapper _client = null;
        public WebClientDownloader(Job job, string fullPathWhereToSave, int timeout)
            : base(job, fullPathWhereToSave, timeout)
        {
        }
        public bool StartDownload(WebClientWrapper client)
        {
            _client = client;
            return StartDownload();
        }
        public override bool StartDownload()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));
                _error = null;
                if (File.Exists(_fullPathWhereToSave))
                {
                    File.Delete(_fullPathWhereToSave);
                }
                if (_client == null)
                {
                    _client = new WebClientWrapper();
                }
                using (_client)
                {
                    _completed = false;
                    var ur = new Uri( _job.getURL());
                    _client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    _client.DownloadFileCompleted += WebClientDownloadCompleted;
                    Console.WriteLine(@"Downloading file: " + _job.getURL());
                    _client.wrapper_DownloadFileAsync(ur, _fullPathWhereToSave);
                    while (_stillDownloading)
                    {
                        _stillDownloading = false;
                        _semaphore.Wait(_timeout);
                    }
                    if (_error != null || !_completed)
                    {
                        File.Delete(_fullPathWhereToSave);                        
                        throw new DownloadFilesException(_error==null?"download error":_error.Message);
                    }
                    return _result && File.Exists(_fullPathWhereToSave);
                }
            }
            catch (Exception e)
            {
                File.Delete(_fullPathWhereToSave);
                throw new DownloadFilesException("Was not able to download file: " + e.Message);
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
            _completed = true;
            _result = !args.Cancelled;
            _error = args.Error;
            if (_result && args.Error == null)
            {
                Console.WriteLine(Environment.NewLine + "Download finished!");
            }
            _stillDownloading = false;
            _semaphore.Release();
        }
    }
}
