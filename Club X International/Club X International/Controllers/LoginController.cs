using Club_X_International.App_Start;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Club_X_International.Models;
using Club_X_International.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace Club_X_International.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class LoginController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LoginController() { }

        public LoginController(ApplicationSignInManager signInManager,ApplicationUserManager userManager)
        {

        }

        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Login
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        //Post: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserManager.FindByEmail(model.Email);
            if (user != null)
            {

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        Session["UserId"] = user.Id;
                        HttpCookie IDCookie = Request.Cookies.Get("gid");
                        if (IDCookie == null)
                        {
                            IDCookie = new HttpCookie("gid");
                            IDCookie.Value = user.Id;
                            IDCookie.Expires = DateTime.Now.AddDays(20);
                            Response.Cookies.Add(IDCookie);
                        }
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        //Post: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Index_Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }
            var user = UserManager.FindByEmail(model.Email);
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            if (user != null)
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        Session["UserId"] = user.Id;
                        HttpCookie IDCookie = Request.Cookies.Get("gid");
                        if (IDCookie == null)
                        {
                            IDCookie = new HttpCookie("gid");
                            IDCookie.Value = user.Id;
                            IDCookie.Expires = DateTime.Now.AddDays(20);
                            Response.Cookies.Add(IDCookie);
                        }
                        Response.Redirect("~/Home/index");
                        return null;
                    //case SignInStatus.LockedOut:
                    //    return View("Lockout");
                    //case SignInStatus.RequiresVerification:
                    //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return PartialView(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return PartialView(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}