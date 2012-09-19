using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluxs.Securest.ApiWeb
{
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Get request parameter from Header, then QueryString, last Form.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetParameter(this HttpRequestBase request, string key)
        {
            var value = request.Headers.Get(key);

            if (string.IsNullOrEmpty(value))
                value = request.QueryString.Get(key);

            if (string.IsNullOrEmpty(value))
                value = request.Form.Get(key);

            return value;
        }
    }
}