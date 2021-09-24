using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Google_OAuth_2._0.Models
{
    public class GoogleDriveRepository : IGoogleDriveRepository
    {
        [Obsolete]
        public DriveService InitializeLogin()
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

        public void UploadFile()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public IEnumerable<DriveFile> GetGoogleDriveFiles()
        {
            DriveService service = InitializeLogin();

            FilesResource.ListRequest FileListRequest = service.Files.List();

            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";


            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

            ICollection<DriveFile> fileList = new List<DriveFile>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var driveFile = new DriveFile
                    {
                        Id = file.Id,
                        Name = file.Name
                    };
                    fileList.Add(driveFile);
                }
            }
            return fileList;
        }

    }
}
