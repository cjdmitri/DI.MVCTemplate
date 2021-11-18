using DI.MVCTemplate.Data;
using DI.MVCTemplate.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DI.MVCTemplate.Controllers
{
    public class HomeController : _MyTemplateController
    {
        //private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IWebHostEnvironment _env, ApplicationDbContext _db) : base(userManager, roleManager, signInManager, _env, _db)
        {

        }

        public async Task<IActionResult> Index()
        {
            await RoleInitializer.InitializeAsync(_userManager, _roleManager);
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

        public IActionResult ErrorStatus(int statusCode)
        {
            if (statusCode == 404)
            {

            }
            return View(statusCode);
        }
    }
}
