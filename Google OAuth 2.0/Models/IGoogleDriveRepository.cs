using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Google_OAuth_2._0.Models
{
    public interface IGoogleDriveRepository
    {
        void InitializeLogin();
        void UploadFile();
    }
}
