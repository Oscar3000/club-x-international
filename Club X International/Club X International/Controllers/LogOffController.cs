using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Controllers
{
    [HandleError(View = "Error")]
    public class LogOffController : Controller
    {
        // GET: LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index()
        {
            HttpCookie IDCookie = Request.Cookies.Get("gid");
            if (IDCookie != null)
            {
                IDCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(IDCookie);
            }
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}