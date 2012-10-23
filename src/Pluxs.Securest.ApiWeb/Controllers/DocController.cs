using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Pluxs.Securest.ApiWeb.Models;

namespace Pluxs.Securest.ApiWeb.Controllers
{
    public class DocController : Controller
    {
        public ActionResult Index()
        {
            var controllers = GetSubClasses<ApiControllerBase>().Where(c => c.Name != "ErrorController");

            var endpoints = controllers.Select(c => new EndpointInfo
            {
                Name = c.Name.Replace("Controller", string.Empty).ToLower(),
                AuthorizeRequired = c.GetCustomAttributes(typeof(ApiAuthorizeAttribute), false).Select(x => ((ApiAuthorizeAttribute)x).Enabled).FirstOrDefault(),
                Actions = c.GetMethods().Where(m => m.ReturnType == typeof(ActionResult) && m.Name != "ApiResult")
                .Select(m => new EndpointActionInfo
                {
                    Name = m.Name.ToLower(),
                    HttpMethod = m.GetCustomAttributes(typeof(AcceptVerbsAttribute), false).SelectMany(a => ((AcceptVerbsAttribute)a).Verbs).FirstOrDefault(),
                    AuthorizeRequired = m.GetCustomAttributes(typeof(ApiAuthorizeAttribute), false).Select(x => ((ApiAuthorizeAttribute)x).Enabled).FirstOrDefault(),
                    Parameters = m.GetParameters().Select(p => new EndpointActionParameterInfo
                    {
                        Name = p.Name.ToLower(),
                        TypeName = p.ParameterType.Name.ToLower()
                    })
                })
            });

            ViewBag.Endpoints = endpoints;

            return View();
        }

        #region Doc helpers

        /// <summary>
        /// List subclasses of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static IEnumerable<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(T)));
        }

        #endregion

    }
}
