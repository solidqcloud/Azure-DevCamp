using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DevCamp.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            ViewBag.Url = Request.Url;
            StringBuilder qString = new StringBuilder();
            foreach(string s in Request.QueryString.AllKeys)
            {
                qString.Append($"Param={s};Value={HttpUtility.UrlDecode(Request.QueryString[s])}");
            }
            ViewBag.QueryString = qString.ToString();
            ViewBag.Error = "An error occurred";
            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            
            ViewBag.Error = $"Invalid Action Name::[{actionName}]";
       }
    }
}