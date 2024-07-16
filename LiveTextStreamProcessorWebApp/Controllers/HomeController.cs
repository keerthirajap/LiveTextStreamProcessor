namespace LiveTextStreamProcessorWebApp.Controllers
{
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? HttpContext.TraceIdentifier;

            // Check if there's an activity and set the requestId accordingly
            if (Activity.Current != null)
            {
                requestId = Activity.Current.Id;
            }

            // Create ErrorViewModel and return View
            var viewModel = new ErrorViewModel { RequestId = requestId };
            return View(viewModel);
        }
    }
}