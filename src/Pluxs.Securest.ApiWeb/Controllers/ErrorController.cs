using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pluxs.Securest.ApiWeb.Controllers
{
    public class ErrorController : ApiControllerBase
    {
        public ActionResult NormalError()
        {
            Response.StatusCode = 400;

            return View();
        }

        public ActionResult ApiError(Exception error)
        {
            var message = error.Message;

            //API EXCEPTION
            var apiError = error as ApiException;

            //HTTP EXCEPTION
            var httpError = error as HttpException;

            //OR OTHER EXCEPTION

            if (apiError != null)
            {
                Response.StatusCode = apiError.HttpStatusCode;
            }
            else if (httpError != null)
            {
                Response.StatusCode = httpError.GetHttpCode();

                switch (httpError.GetHttpCode())
                {
                    case 404:
                        message = "Requested path can not be found";
                        break;
                    default:
                        message = "Server Fault";
                        break;
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                message = error.Message;
            }

            try
            {
                return this.ApiResult(new
                {
                    error = message,
                    url = Request.Url.AbsolutePath,
                    method = Request.HttpMethod,
#if DEBUG
                    type = error.GetType().FullName,
                    stack_trace = error.StackTrace,
#endif
                });
            }
            catch (ApiException ex)
            {
                //LogHelper.Error(error);

                return new ContentResult
                {
                    ContentEncoding = ApiControllerBase.ContentEncoding,
                    ContentType = "text/plain",
                    Content = ex.Message
                };
            }
        }
    }
}