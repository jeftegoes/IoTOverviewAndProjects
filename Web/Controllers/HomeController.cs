using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Messaging.Interfaces.Commands;
using Messaging.Interfaces.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;

        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel orderViewModel)
        {
            var memoryStream = new MemoryStream();
            using (var uploadedFile = orderViewModel.File.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
            }

            orderViewModel.ImageData = memoryStream.ToArray();
            orderViewModel.ImageUrl = orderViewModel.File.FileName;
            orderViewModel.OrderId = Guid.NewGuid();

            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMqUri }" + $"{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");

            var endPoint = await _busControl.GetSendEndpoint(sendToUri);

            await endPoint.Send<IRegisterOrderCommand>(new {
                orderViewModel.OrderId,
                orderViewModel.UserEmail,
                orderViewModel.ImageData,
                orderViewModel.ImageUrl
            });

            ViewData["OrderId"] = orderViewModel.OrderId;
            return View("Thanks");
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
