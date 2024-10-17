using Microsoft.AspNetCore.Mvc;
using QRCodeGenerator.App.Interfaces;
using QRCodeGenerator.App.Models;
using QRCodeGenerator.App.ViewModels;
using System.Diagnostics;

namespace QRCodeGenerator.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly IMessageService _messageService;

        public HomeController(ILogger<HomeController> logger, IMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string text)
        {
            if(string.IsNullOrEmpty(text))
               return BadRequest();
            byte[] qrCodeAsBytes = _messageService.SendMessage(text).Result;
            string qrCodeAsBase64 = $"data:image/png;base64,{ Convert.ToBase64String(qrCodeAsBytes)}";

            GenerateQRCodeViewModel model = new();
            model.QRCodeImage = qrCodeAsBase64;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
