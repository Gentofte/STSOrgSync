using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace STSOrgSyncV2.Controllers
{
    // ================================================================================
    [Authorize]
    public class HomeController : Controller
    {
        // -----------------------------------------------------------------------------
        public IActionResult Index()
        {
            return View();
        }
    }
}
