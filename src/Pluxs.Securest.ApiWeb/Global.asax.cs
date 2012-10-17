using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Pluxs.Securest.ApiWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            var path = Request.Url.AbsolutePath.ToLower();

            var error = Server.GetLastError();

            Server.ClearError();

            Response.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.TrySkipIisCustomErrors = true;

            var action = path.StartsWith("/api") ? "ApiError" : "NormalError";

            //HANDLE RESPONSE
            var routeData = new RouteData();

            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", action);
            routeData.Values.Add("error", error);

            IController errorController = new Controllers.ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(this.Context), routeData));
        }
    }
}