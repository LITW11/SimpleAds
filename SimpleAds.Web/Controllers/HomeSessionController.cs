using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SimpleAds.Data;
using SimpleAds.Web.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace SimpleAds.Web.Controllers
{
    public class HomeSessionController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=True";

        public IActionResult Index()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            List<SimpleAd> databaseAds = db.GetAds();
            var ids = HttpContext.Session.Get<List<int>>("ListingIds");

            return View(new HomePageViewModel
            {
                Ads = databaseAds.Select(ad => new AdViewModel
                {
                    Ad = ad,
                    CanDelete = ids != null && ids.Contains(ad.Id)
                }).ToList()
            });
        }

        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(SimpleAd ad)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            db.AddSimpleAd(ad);

            List<int> ids = HttpContext.Session.Get<List<int>>("ListingIds");
            if (ids == null)
            {
                ids = new List<int>();
            }
            ids.Add(ad.Id);
            HttpContext.Session.Set("ListingIds", ids);
            return Redirect("/homesession/index");
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("ListingIds");
            if (ids != null && ids.Contains(id))
            {
                SimpleAdDb db = new SimpleAdDb(_connectionString);
                db.Delete(id);
            }

            return Redirect("/homesession/index");
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}
