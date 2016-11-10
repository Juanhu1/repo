using System;

namespace DownloadFiles
{
    public class Job
    {
        public enum eProtocols { eHTTP, eFTP, eSFTP };
        private string _URI, _Name, _Password;
        private eProtocols _eProtocol = eProtocols.eHTTP;
        public Job(eProtocols Protocol, string URI, string Name = null, string Password = null)
        {
            if (string.IsNullOrEmpty(URI)) throw new ArgumentNullException("URI");
            _eProtocol = Protocol;
            _URI=URI ;
            _Name = Name==null?null:Name;
            _Password = Password==null?null:Password ;
        }
        public eProtocols Protocol
        {
            get { return _eProtocol; }
        }
        public string URI
        {
            get { return _URI; }
        }
        public string getURL()
        {
            string u = "";
            switch (Protocol) {
                case eProtocols.eHTTP:
                    u = "http";
                    break;
                case eProtocols.eFTP:
                    u = "ftp";
                    break;
                case eProtocols.eSFTP:
                    u = "sftp";
                    break;
            }
            return u + "://" + URI;
        }
        public string Name
        {
            get { return _Name; }
        }
        public string Password
        {
            get { return _Password; }
        }
    }
}
