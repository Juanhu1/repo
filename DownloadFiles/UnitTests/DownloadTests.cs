using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DownloadFiles;
using System.IO;
using System.Net;

namespace DownloadFiles
{
    class WebClientMock : WebClientWrapper 
    {
        public override void wrapper_DownloadFileAsync(Uri address, string fileName)
        {
            string text = "Some text for partially downloaded data";
            // WriteAllText creates a file, writes the specified string to the file,
            // and then closes the file.    You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllText(fileName, text);
        }
    }
    [TestClass]
    public class DownloadTests
    {

        const string testFilePath = @"C:\temp";
        static DownloaderFactory downloader;
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            downloader = new DownloaderFactory(@"C:\temp", 5000);
        }

        [TestMethod]
        [ExpectedException(typeof(DownloadFilesException))]
        public void TestInvalidFileExceptions()
        {
            Job job = new Job(Job.eProtocols.eHTTP, "http://index.hu/123.jpg", null, null);
            bool result = downloader.Create(job).StartDownload();
            Assert.Fail();
        }
        [TestMethod]
        [ExpectedException(typeof(DownloadFilesException))]
        public void TestJobFileExceptions()
        {
            Jobs jobs = new Jobs("Notexist.json");
            jobs.LoadJson();
            Assert.Fail();
        }
        [TestMethod]
        public void TestParse()
        {
            Jobs jobs = new Jobs();
            try
            {
                jobs.ParseJsonstringToList("");
                Assert.Fail();
            }
            catch (DownloadFilesException)
            {

            }
            jobs.ParseJsonstringToList("[]");
            Assert.AreEqual(jobs.getJobsCount(), 0);
            jobs.ParseJsonstringToList("[{\"protocol\": \"http\", \"uri\": \"www.jatekfutar.hu/molto/molto-15419.jpg\" }]");
            Assert.AreEqual(jobs.getJobsCount(), 1);
        }
        [TestMethod]
        public void TestJobFile()
        {
            Jobs jobs = new Jobs();
            try
            {
                jobs.LoadJson();
            }
            catch (DownloadFilesException )
            {
                Assert.Fail();
            }
            int count = 0;
            foreach (Job job in jobs.Next())
            {
                count++;                
            }
            Assert.AreEqual(count, 2);
        }
        [TestMethod]
        public void TestPartiallyDownload()
        {
            WebClientMock client = new WebClientMock();
            Job job = new Job(Job.eProtocols.eHTTP, "www.jatekfutar.hu/molto/molto-15419.jpg", null, null);
            try
            {
                bool result = ((WebClientDownloader)downloader.Create(job)).StartDownload(client);
            }
            catch (DownloadFilesException)
            {

            }
            Assert.IsFalse(File.Exists(testFilePath + "\\molto-15419.jpg"), "File IS exist");
        }
        [TestMethod]
        public void TestHttpDownload()
        {
            Job job = new Job(Job.eProtocols.eHTTP, "www.jatekfutar.hu/molto/molto-15419.jpg", null, null);
            bool result = downloader.Create(job).StartDownload();
            Assert.IsTrue(result, "Result is false" );
            Assert.IsTrue(File.Exists(testFilePath+ "\\molto-15419.jpg" ), "File not exist" );
            long length = new FileInfo(testFilePath + "\\molto-15419.jpg").Length;
            Assert.AreEqual(length, 313261);
        }
        [TestMethod]
        public void TestFtpDownload()
        {
            Job job = new Job(Job.eProtocols.eFTP, "speedtest.tele2.net/1MB.zip", null, null);
            bool result = downloader.Create(job).StartDownload();
            Assert.IsTrue(result, "Result is false");
            Assert.IsTrue(File.Exists(testFilePath + "\\1MB.zip"), "File not exist");
            long length = new FileInfo(testFilePath + "\\1MB.zip").Length;
            Assert.AreEqual(length, 1048576);
        }
        [TestMethod]
        public void TestSFtpDownload()
        {
            Job job = new Job(Job.eProtocols.eSFTP, "test.rebex.net:22/pub/example/WinFormClient.png", "demo", "password");
            bool result = downloader.Create(job).StartDownload();
            Assert.IsTrue(result, "Result is false");
            Assert.IsTrue(File.Exists(testFilePath + "\\WinFormClient.png"), "File not exist");
            long length = new FileInfo(testFilePath + "\\WinFormClient.png").Length;
            Assert.AreEqual(length, 80000);
        }
    }
}
