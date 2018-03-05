using Club_X_International.App_Start;
using Club_X_International.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Club_X_International
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterGlobalFilters(GlobalFilters.Filters);

        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Error(object sender,EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();
            var route = new RouteData();
            route.Values.Add("controller", "errors");
            if (exception is HttpException)
            {
                var httpException = (HttpException)exception;
                if (httpException != null)
                {
                    switch (httpException.GetHttpCode())
                    {
                        case 400:
                            route.Values.Add("action", "Page400");
                            break;
                        case 401:
                            route.Values.Add("action", "Page401");
                            break;
                        case 403:
                            route.Values.Add("action", "Page403");
                            break;
                        case 404:
                            route.Values.Add("action", "Page404");
                            break;
                        case 408:
                            route.Values.Add("action", "Page408");
                            break;
                        case 500:
                            route.Values.Add("action", "Page500");
                            break;
                        case 501:
                            route.Values.Add("action", "Page501");
                            break;
                        case 502:
                            route.Values.Add("action", "Page502");
                            break;
                        default:
                            route.Values.Add("action", "general");
                            break;
                    }
                    Server.ClearError();
                   Response.TrySkipIisCustomErrors = true;
                }
                //Response.StatusCode = httpException.GetHttpCode();
            }
            IController errorController = new ErrorsController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), route));
        }
    }
}
