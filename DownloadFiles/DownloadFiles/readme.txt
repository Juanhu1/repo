Hi Laszlo,
I went over your solution. The architecture is pretty good, but there were a few things that could have 
been done better
1.       There is too much business logic in your main method. There should be a downloader factory that 
will create the right downloader given a job, or url, or etc. Your current logic makes it difficult to 
add new downloaders, or to change the downloader for a specific URL
2.       The way you handle the protocols could be improved. Currently you start with a string passed to 
the Job, where instead you could pass the enum directly (therefore avoiding the switch statements). 
Also, a protocol factory would create a protocol given a URL and all error handling would be there.
3.       Unit tests are lacking. You have no tests for cases when the files only download partially 
(in which case the partial file must be deleted).
Levon



USAGE:
DownloadFiles Path=c:\temp timeout=5000 jobfile=DownloadJobs.json

- all command line parameters are optional. The default values visible in above.

Where:
Path=c:\temp is the location where files will be stored 
timeout=5000 is the timeout in millisec. It means every 5 seconds a package should arrive (not a whole file, only a package!)
jobfile=DownloadJobs.json is the definition of the jobs. Example below.

If the file is exist it will be overwritten!
In the case of error the program exit (for example if one file is missing from the joblist - wrong url - then 
exit but not deleting the files are already correctly downloaded)


Future development ideas:
- Better error location when error in job file
- More information when error in command line
- simultenous file downloads with threads
- unit tests for the functionality in main (more unit tests at all)
- SFTP download should shows the percent as the others

constraints:
- keys in the jobfile should be lowercased

example of job file (this file exists in the bin/Debug folder):
------------------------------------
[
	{
		"protocol": "http",
		"uri": "www.jatekfutar.hu/molto/molto-15419.jpg"
	}, 
	{
		"protocol": "ftp",
		"uri": "speedtest.tele2.net/1MB.zip"
	},
	{
		"protocol": "sftp",
		"uri": "test.rebex.net:22/pub/example/ConsoleClient.png",
		"name" : "demo",
		"password": "password"
	}
]
-----------------------------------

Program structure.

DownloadInterface.cs - Basic interface definition for the different protocols
DownloadAbstract.cs - Absract class based on the interface for the different protocols. It is define a common constructor.
WebClientDownloader.cs - Common class HTTP and FTP download. Same functionality covered.
HTTPDownloader.cs - Almost empty class based on WebClientDownloader. We don't need any special handling.
FTPDownloader.cs - Almost empty class based on WebClientDownloader. We don't need any special handling.
SFTPDownloader.cs - Based on DownloadAbstract. SFTP should be handling a different way with different library (ssh.net)

Job.cs - define one job (on file)
Jobs.cs - handling and storing the jobs
Program.cs - main program, processing the command line arguments and downloading the files. Would be better separate them.

DownloadTests.cs - contains 6 tests, 3 for downloading, 3 for normal/not normal funcionality
