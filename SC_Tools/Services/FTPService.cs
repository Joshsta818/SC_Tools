using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace SC_Tools.Services
{
    public static class FTPService
    {
        public static FtpClient client;

        public static void SetupClient()
        {
            //Setup host for client object
            client = new FtpClient(Properties.Resources.Host);

            //Setup Creds for client object
            client.Credentials = new NetworkCredential(Properties.Resources.User, Properties.Resources.Password);
        }

        public static void GetDirectoryList()
        {
            client.Connect();
            foreach(FtpListItem item in client.GetListing("/sctools2020"))
            {
                if(item.Type == FtpFileSystemObjectType.File)
                {
                    long size = client.GetFileSize(item.FullName);

                    FtpHash hash = client.GetChecksum(item.FullName);
                }
                DateTime time = client.GetModifiedTime(item.FullName);
            }
            client.Disconnect();
        }

        public static void DownloadDirectory()
        {
            client.Connect();
            client.DownloadDirectory(@"C:\SCTools2021", @"/sctools2020", FtpFolderSyncMode.Mirror);
            client.Disconnect();
        }
    }
}
