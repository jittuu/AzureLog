using System.Web.Mvc;

namespace AzureLog.Web.Controllers
{
    public class HomeController : AsyncController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "StorageAccounts");
        }
    }
}