using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DownloadFiles;
using System.IO;

namespace DownloadFiles
{
    [TestClass]
    public class UnitTest1
    {

        const string testFilePath = @"C:\temp";

        [TestMethod]
        public void TestExceptions()
        {
            try
            {
                DownloaderFactory df = new DownloaderFactory(@"C:\temp", 5000);
                Job job = new Job(Job.eProtocols.eHTTP, "http://index.hu/123.jpg", null, null);
                bool result = df.Create(job).StartDownload();
                Assert.Fail();
            }
            catch (DownloadFilesException de)
            {
            }
            Jobs jobs = new Jobs("Notexist.json");
            try
            {
                jobs.LoadJson();
                Assert.Fail();
            } 
            catch (DownloadFilesException de)
            {

            }
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
            catch (DownloadFilesException de)
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
        public void TestHttpDownload()
        {            
            bool result =  FileDownload.DownloadFile("http://www.jatekfutar.hu/molto/molto-15419.jpg", testFilePath, 5000, null,null);
            Assert.IsTrue(result, "Result is false" );
            Assert.IsTrue(File.Exists(testFilePath+ "\\molto-15419.jpg" ), "File not exist" );
            long length = new FileInfo(testFilePath + "\\molto-15419.jpg").Length;
            Assert.AreEqual(length, 313261);
        }
        [TestMethod]
        public void TestFtpDownload()
        {
            bool result = FileDownload.DownloadFile("ftp://speedtest.tele2.net/1MB.zip", testFilePath, 5000,null, null);
            Assert.IsTrue(result, "Result is false");
            Assert.IsTrue(File.Exists(testFilePath + "\\1MB.zip"), "File not exist");
            FileAttributes fa=File.GetAttributes(testFilePath + "\\1MB.zip");
            long length = new FileInfo(testFilePath + "\\1MB.zip").Length;
            Assert.AreEqual(length, 1048576);
        }
    }
}
