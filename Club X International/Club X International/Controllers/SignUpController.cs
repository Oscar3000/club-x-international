using Club_X_International.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Club_X_International.Models;
using Microsoft.AspNet.Identity;
using Club_X_International.Models.ViewModels;
using System.Net.Mail;
using System.Net;

namespace Club_X_International.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class SignUpController : Controller
    {
        private ApplicationSignInManager _signManager;
        private ApplicationUserManager _userManager;

        public SignUpController()
        { }

        public SignUpController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set
            {
                _signManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            set
            {
                _userManager = value;
            }
        }

        // GET: SignUp
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        public void SendConfirmationToken(ApplicationUser user)
        {
            var mailAddressFrom = new MailAddress(MailSettings.smtpCredUN);
            var mailAddressTo = new MailAddress(user.Email);
            var mailMessage = new MailMessage(mailAddressFrom, mailAddressTo);
            mailMessage.Subject = "Email Confirmation";
            var code = UserManager.GenerateEmailConfirmationToken(user.Id);
            var callBackUrl = Url.Action("ConfirmEmail", "SignUp", new { code = code, userId = user.Id }, Request.Url.Scheme);
            mailMessage.Body = string.Format("<h3>Thank you for registering with us!</h3>" +
                "<h3>Please click <a href=\"{0}\" title=\"Confirm Registration\">here</a> to confirm your Registration</h3>", callBackUrl);
            mailMessage.IsBodyHtml = true;
            MailSettings.Mail(mailMessage);
        }


        //Post: SignUp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, EmailConfirmed = false };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    SendConfirmationToken(user);
                   return RedirectToAction("Confirm", "SignUp", new { Email = user.Email});
                }
                addErrors(result);
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string code, string userId)
        {
            ApplicationUser user = this.UserManager.FindById(userId);
            if (user == null)
            {
                return View("Error");
            }
            if (userId != null || code != null)
            {
                var result = await UserManager.ConfirmEmailAsync(user.Id, code);
                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    await UserManager.AddToRolesAsync(user.Id, "User");
                    Session["UserId"] = user.Id;
                    HttpCookie IDCookie = Request.Cookies.Get("gid");
                    if (IDCookie == null)
                    {
                        IDCookie = new HttpCookie("gid");
                        IDCookie.Value = user.Id;
                        IDCookie.Expires = DateTime.Now.AddDays(20);
                        Response.Cookies.Add(IDCookie);
                    }
                    return RedirectToAction("Index", "Home", new { ConfirmedEmail = user.Email });

                }
                else
                {
                    addErrors(result);
                    return RedirectToAction("TokenError", "SignUp", new { Email = user.Email });
                }
            }
            else
            {
                return RedirectToAction("Confirm", "SignUp", new { Email = "" });
            }
        }

        //Get: TokenErrror
        public ActionResult TokenError(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public ActionResult ResendToken(string Email)
        {
            var user = UserManager.FindByEmail(Email);
            if (user != null)
            {
                SendConfirmationToken(user);
                return RedirectToAction("Confirm", "SignUp", new { Email = user.Email });
            }
            else
            {
                return RedirectToAction("Confirm", "SignUp", new { Email = "" });
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

                if (_signManager != null)
                {
                    _signManager.Dispose();
                    _signManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region
        private void addErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion


    }
}