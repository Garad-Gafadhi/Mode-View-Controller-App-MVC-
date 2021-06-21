using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VOD.Common.Entities;
using VOD.Common.Extensions;
using VOD.UI.Models;

namespace VOD.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<VODUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<VODUser> singInMgr)
        {
            _logger = logger;
            _signInManager = singInMgr;
        }

        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
                return  RedirectToPage("/Account/Login",
                    new { Area = "Identity" });
            return  RedirectToAction("Dashboard", "Membership");
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

        #region Methods

        //public async Task<IActionResult> Index()
        //{
        //    #region Method testing

        //    //var result1 = await _db.SingleAsync<Download>(
        //    //    s => s.Id.Equals(2));

        //    //var result2 = await _db.GetAsync<Download>();

        //    //var result3 = await _db.GetAsync<Download>(
        //    //    e => e.ModuleId.Equals(1));

        //    //var result4 = await _db.AnyAsync<Download>(
        //    //    x => x.ModuleId.Equals(1));

        //    //var videos = new List<Video>();
        //    //var convertedList = videos.ToSelectList("Id", "Title");

        //    //_db.Include<Download>();
        //    //var result5 = await _db.SingleAsync<Download>(d => d.Id.Equals(3));

        //    //_db.Include<Download>();
        //    //_db.Include<Module, Course>();
        //    //var result6 = await _db.SingleAsync<Download>(d => d.Id.Equals(3));

        //    #endregion Method testing

        //    var user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

        //    if (user != null)
        //    {
        //        var courses = await _db.GetCoursesAsync(user.Id);
        //    }

        //    if (!_signInManager.IsSignedIn(User))
        //        return RedirectToPage("/Account/Login",
        //            new { Area = "Identity" });
        //    return View();
        //}

        #endregion Methods
    }
}