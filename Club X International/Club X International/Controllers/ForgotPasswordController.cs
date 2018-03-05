using Club_X_International.App_Start;
using Club_X_International.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Controllers
{
    [Authorize]
    public class ForgotPasswordController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signinManager;
        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager {
            get
            {
                return _signinManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signinManager = value;
            }
        }

        public ForgotPasswordController() { }

        public ForgotPasswordController(ApplicationUserManager userManager,ApplicationSignInManager signinManager)
        {
            UserManager = userManager;
            SignInManager = signinManager;
        }

        // GET: ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [NonAction]
        public void SendToken(ApplicationUser user)
        {
            var mailAddressFrom = new MailAddress(MailSettings.smtpCredUN);
            var mailAddressTo = new MailAddress(user.Email);
            var mailMessage = new MailMessage(mailAddressFrom, mailAddressTo);
            mailMessage.Subject = "Password Reset";

            var code = UserManager.GeneratePasswordResetToken(user.Id);
            var callBackUrl = Url.Action("ResetPassword", "ForgotPassword", new { code = code, userId = user.Id }, protocol: Request.Url.Scheme);

            mailMessage.Body = string.Format("<h3>Reset your Password by clicking " +
                "<a href=\"{0}\" title=\"Password Reset\">here</a></h3>", callBackUrl);
            mailMessage.IsBodyHtml = true;
            MailSettings.Mail(mailMessage);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if(user == null || user.EmailConfirmed == false)
                {
                    return View(model);
                }
                SendToken(user);
                return RedirectToAction("ConfirmForgotPassword", new { Email = user.Email });

            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ConfirmForgotPassword(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Index", "SignUp");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                await UserManager.UpdateAsync(user);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                addErrors(result);
                return RedirectToAction("TokenError", "ForgotPassword", new { Email = user.Email });
            }
        }
        [AllowAnonymous]
        public ActionResult TokenError(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public async Task<ActionResult> ResendToken(string Email)
        {
            var user = await UserManager.FindByEmailAsync(Email);
            if (user != null)
            {
                SendToken(user);
                return RedirectToAction("ConfirmForgotPassword", new { Email = user.Email });
            }else
            {
                return RedirectToAction("ForgotPassword");
            }
        }

        #region
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if(_signinManager != null)
                {
                    _signinManager.Dispose();
                    _signinManager = null;
                }
            }
            base.Dispose(disposing);
        }
        
        private void addErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion
    }
}