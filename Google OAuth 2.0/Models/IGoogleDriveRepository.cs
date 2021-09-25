using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Google_OAuth_2._0.Models
{
    public interface IGoogleDriveRepository
    {
        DriveService GetDriveService();
        void UploadFile(IFormFile imageFile);
        IEnumerable<DriveFile> GetGoogleDriveFiles();

    }
}
