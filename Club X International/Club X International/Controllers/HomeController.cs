using Club_X_International.DataConnect;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Controllers
{
    [HandleError(View = "Error")]
    public class HomeController : Controller
    {
        private Repository _repo = new Repository();
        // GET: Home
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                HttpCookie IDCookie = Request.Cookies.Get("gid");
                if (IDCookie != null)
                {
                    var id = Server.HtmlEncode(IDCookie.Value);
                    var user = _repo.FindUserByID(id);
                    if (user != null)
                    {
                        Session["FirstName"] = user.FirstName;
                        Session["UserId"] = user.Id;
                    }
                    else
                    {
                        Session["FirstName"] = "Guest";
                    }
                }
                else
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // Search
        public ActionResult Search(string searchString)
        {
            if (!(string.IsNullOrEmpty(searchString)))
            {
                if (searchString.Contains("blog"))
                {
                    return RedirectToAction("Index", "Blog");
                }
                else if (searchString.Contains("event"))
                {
                    return RedirectToAction("Index", "Events");
                }
            }
            return RedirectToAction("Index");
        }
    }
}