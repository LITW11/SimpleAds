using System;
using SimpleAds.Data;

namespace SimpleAds.Web.Models
{
    public class AdViewModel
    {
        public SimpleAd Ad { get; set; }
        public bool CanDelete { get; set; }
    }
}
