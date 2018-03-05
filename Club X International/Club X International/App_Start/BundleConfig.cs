using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Club_X_International.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundle)
        {
            bundle.Add(new StyleBundle("~/Content/site").Include("~/Content/Site.css"));
            bundle.Add(new StyleBundle("~/Content/font-awesome").Include("~/Content/font-awesome.min.css"));
            bundle.Add(new StyleBundle("~/Content/PageList").Include("~/Content/PagedList.css"));
            bundle.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/Mobirise Template/assets/bootstrap/css/bootstrap.min.css"));
            bundle.Add(new ScriptBundle("~/Scripts/jquery").Include("~/scripts/jquery-{version}.js"));
            bundle.Add(new ScriptBundle("~/Scripts/jqueryval").Include("~/scripts/jquery.validate*"));
            bundle.Add(new ScriptBundle("~/Scripts/bootstrap").Include("~/scripts/bootstrap.min.js"));
            bundle.Add(new ScriptBundle("~/Scripts/site").Include("~/scripts/Site.js"));
            bundle.Add(new ScriptBundle("~/Scripts/modernizr").Include("~/scripts/modernizr-2.6.2.js"));
            bundle.Add(new ScriptBundle("~/Scripts/ckeditor").Include("~/scripts/ckeditor/ckeditor.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}