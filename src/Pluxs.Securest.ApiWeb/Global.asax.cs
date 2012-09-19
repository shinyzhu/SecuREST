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

            if (path.StartsWith("/api"))
            {
                var error = Server.GetLastError();

                Server.ClearError();

                //TODO: LOG ERROR TO STORAGE
                //LogHelper.Error(error);

                //HANDLE RESPONSE
                var routeData = new RouteData();

                routeData.Values.Add("action", "ApiError");
                routeData.Values.Add("error", error);

                Response.Clear();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.TrySkipIisCustomErrors = true;

                IController errorController = new Controllers.ErrorController();
                errorController.Execute(new RequestContext(new HttpContextWrapper(this.Context), routeData));
            }
        }
    }
}