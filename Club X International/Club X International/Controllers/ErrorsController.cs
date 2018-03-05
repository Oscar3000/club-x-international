using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Page400()
        {
            return View();
        }

        public ActionResult Page401()
        {
            return View();
        }

        public ActionResult Page403()
        {
            return View();
        }

        public ActionResult Page404()
        {
            return View();
        }

        public ActionResult Page408()
        {
            return View();
        }

        public ActionResult Page500()
        {
            return View();
        }

        public ActionResult Page501()
        {
            return View();
        }

        public ActionResult Page502()
        {
            return View();
        }

        public ActionResult general()
        {
            return View();
        }
    }
}