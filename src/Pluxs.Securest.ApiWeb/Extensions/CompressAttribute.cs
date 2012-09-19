using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pluxs.Securest.ApiWeb
{
    /// <summary>
    /// Proides response compress with GZIP or DEFLATE.
    /// </summary>
    public class CompressAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets a bool value indicats the compress is enabled.
        /// </summary>
        public bool Enable { get; set; }

        public CompressAttribute()
        {
            this.Enable = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.Enable)
                return;

            var request = filterContext.RequestContext.HttpContext.Request;
            var acceptEncoding = request.Headers.Get("Accept-Encoding");

            if (string.IsNullOrEmpty(acceptEncoding))
                return;

            acceptEncoding = acceptEncoding.ToUpper();
            var response = filterContext.RequestContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-Encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-Encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}