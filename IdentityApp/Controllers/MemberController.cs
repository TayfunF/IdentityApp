using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
