using Microsoft.AspNetCore.Mvc;
using Shares.Models;
using System.Diagnostics;
using Shares.Controllers;

//https://stackify.com/restsharp/
//Add Another Class to use the API below to refresh the Market Values and the total value in the Database then refresh the view
//when clicking the refresh button on the home page.
//https://rapidapi.com/alphavantage/api/alpha-vantage/


namespace Shares.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration Configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            Configuration = _configuration;
        }
        public IActionResult Index()
        {
            var db = new Model.Shares_DBContext(Configuration);
            var query = from b in db.Shares orderby b.Count select b;
            ViewData["Message"] = query;
            return View();    
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