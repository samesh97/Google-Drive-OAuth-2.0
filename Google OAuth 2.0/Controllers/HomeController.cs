using Google_OAuth_2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Google_OAuth_2._0.Controllers
{
    public class HomeController : Controller
    {
        private IGoogleDriveRepository _googleDriveRepository;

        public HomeController(IGoogleDriveRepository googleDriveRepository)
        {
            _googleDriveRepository = googleDriveRepository;
        }

        public IActionResult Index()
        {
            var list = _googleDriveRepository.GetGoogleDriveFiles();
            return View(list);
        }

        public IActionResult Login()
        {
            _googleDriveRepository.InitializeLogin();
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
