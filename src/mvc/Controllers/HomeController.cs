using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using mvc.Models;

using RestSharp;
using RestSharp.Authenticators;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoggedOut()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> WeatherAsync()
        {

            var token = await HttpContext.GetTokenAsync("access_token");
            var client = new RestClient("https://localhost:44385");
            client.Authenticator = new JwtAuthenticator(token);
            var req = new RestRequest("/WeatherForecast", Method.GET);
            var res = await client.ExecuteAsync<List<WeatherForecast>>(req);


            return View(res.Data);
        }


        public async Task Logout()
        {
            await HttpContext.SignOutAsync("cookie");
            await HttpContext.SignOutAsync("oidc");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
