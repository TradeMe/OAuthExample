using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using TradeMeExampleOAuthApplication.Models;
using TradeMeExampleOAuthApplication.ViewModel.Home;

namespace TradeMeExampleOAuthApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new ConsumerConfiguration()
            {
                HasValidConsumerKeys = !string.IsNullOrEmpty(OauthHeaderData.ConsumerKey) &&
                                         !string.IsNullOrEmpty(OauthHeaderData.ConsumerSecret)
            });
        }

        [HttpPost]
        public ActionResult SetKeys(string consumerKey, string consumerSecret)
        {
            OauthHeaderData.ConsumerKey = consumerKey;
            OauthHeaderData.ConsumerSecret = consumerSecret;
            return RedirectToAction("Index");
        }
    }
}