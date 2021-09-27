using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Google_OAuth_2._0.Models
{
    public class GoogleDriveRepository : IGoogleDriveRepository
    {

        private readonly IHostingEnvironment _env;

        public GoogleDriveRepository(IHostingEnvironment env)
        {
            _env = env;
        }

        
        public DriveService GetDriveService()
        {
            string[] scopes = { DriveService.Scope.Drive };
            string credentials = "credentials.json";
            string appName = "myproject-326921";

            UserCredential credential;

            using (var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, "./credentials/credentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            });



            return service;
        }

        
        public void UploadFile(IFormFile file)
        {
            string path = SaveFileLocally(file);

            var service = GetDriveService();
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = Path.GetFileName(path);
            fileMetadata.MimeType = "image/jpeg";
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id";
                request.Upload();
            }

            var files = request.ResponseBody;

           
        }

        
        public IEnumerable<DriveFile> GetGoogleDriveFiles()
        {
            DriveService service = GetDriveService();

            FilesResource.ListRequest FileListRequest = service.Files.List();

            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";


            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

            ICollection<DriveFile> fileList = new List<DriveFile>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {

                    string size = FindFileSize(file.Size);


                    var driveFile = new DriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        FileSize = size,
                        CreatedOn = file.CreatedTime.Value.ToShortDateString()
                    };
                    fileList.Add(driveFile);
                }
            }
            return fileList;
        }
        public string FindFileSize(long? fileSize)
        {
            string text;

            if (fileSize == null) text = "Not Found";
            else
            {
                text = " Bytes";

                if (fileSize >= 1024)
                {
                    fileSize /= (1024);
                    text = " KB";
                }

                if (fileSize >= 1024)
                {
                    fileSize /= (1024);
                    text = " MB";
                }
            }

            return fileSize + text;
        }
        public string SaveFileLocally(IFormFile file)
        {
            var dir = _env.ContentRootPath;

            var path = Path.Combine(dir, "Uploads/" + file.FileName);

            using (var fileStream = new FileStream(path,FileMode.Create,FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            return path;
        }

        public string DownloadFile(string fileId)
        {
            DriveService service = GetDriveService();

            var dir = _env.ContentRootPath;
            

            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;

            var path = Path.Combine(dir, $"Downloads/{FileName}");

            MemoryStream stream1 = new MemoryStream();

            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                            {
                                stream1.WriteTo(file);
                            }

                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);

            return path;
        }
    }
}
