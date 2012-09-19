using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Pluxs.Securest.ApiWeb
{
    /// <summary>
    /// OAuth like authorization for api requests
    /// </summary>
    public class ApiAuthorizeAttribute : ActionFilterAttribute
    {
        static readonly double TimeSpanDifferInMinute = 5;

        static readonly string XAuthApiKeyKey = "x_auth_apikey";
        static readonly string XAuthNonceKey = "x_auth_nonce";
        static readonly string XAuthTimeStampKey = "x_auth_timestamp";
        static readonly string XAuthSignatureKey = "x_auth_signature";

        static List<string> NonceCache = new List<string>();

        /// <summary>
        /// Gets or sets a bool value indicates the authorize is enabled, true enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Create an instance of ApiAuthorizeAttrubite and enabled.
        /// </summary>
        public ApiAuthorizeAttribute()
        {
            this.Enabled = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if !DEBUG
            if (this.Enabled)
            {
                ValidateRequest(filterContext.HttpContext.Request);
            }
#endif

            base.OnActionExecuting(filterContext);
        }

        static void ValidateRequest(HttpRequestBase request)
        {
            var apiKey = request.GetParameter(XAuthApiKeyKey);

            if (string.IsNullOrEmpty(apiKey))
                throw new ApiAuthorizeException("api key is not found");

            var app = Models.AppInfo.Apps.Where(x => x.ApiKey == apiKey).FirstOrDefault();

            if (null == app)
                throw new ApiAuthorizeException("api key is invalid");

            if (Models.AppInfo.AppStatus.Normal != app.Status)
                throw new ApiAuthorizeException("app is invalid");

            //ts
            var timestamp = request.GetParameter(XAuthTimeStampKey);

            if (string.IsNullOrEmpty(timestamp))
                throw new ApiAuthorizeException("timestamp is not found");

            double ts;
            if (!double.TryParse(timestamp, out ts))
                throw new ApiAuthorizeException("timestamp is invalid");

            //timespan 5 min expire
            var time = DateTimeExtensions.Date1970.AddMilliseconds(ts);
            var now = DateTime.UtcNow;
            TimeSpan span = now - time;

            if (Math.Abs(span.TotalMinutes) > TimeSpanDifferInMinute)
                throw new ApiAuthorizeException("request is timeout");

            //nonce
            var nonce = request.GetParameter(XAuthNonceKey);

            if (string.IsNullOrEmpty(nonce))
                throw new ApiAuthorizeException("request nonce is not found");

            if (NonceCache.Contains(nonce))
                throw new ApiAuthorizeException("duplicated request");
            else
                NonceCache.Add(nonce);

            //signature
            var signature = request.GetParameter(XAuthSignatureKey);

            if (string.IsNullOrEmpty(signature))
                throw new ApiAuthorizeException("signature is not found");

            //compute signature
            var method = request.HttpMethod.ToUpper();

            var url = request.Url.AbsoluteUri;
            if (!string.IsNullOrEmpty(request.Url.Query))
                url = url.Replace(request.Url.Query, string.Empty);

            var paramters = new Dictionary<string, string>();
            foreach (var k in request.Form.AllKeys)
            {
                paramters.Add(k, request.Form.Get(k));
            }
            foreach (var k in request.QueryString.AllKeys)
            {
                paramters.Add(k, request.QueryString.Get(k));
            }

            var paramString = string.Join("&", paramters.OrderBy(d => d.Key).Select(d => string.Format("{0}={1}", (d.Key), (d.Value))).ToArray());

            //METHOD&url&paramString&nonce&timestamp&secret
            var source = UrlEncode(method) + "&" +
                UrlEncode(url) + "&" +
                UrlEncode(paramString) + "&" +
                UrlEncode(nonce) + "&" +
                timestamp + "&" + app.ApiSecret;

            //LogHelper.Info(source);

            var hash = Hashing.ComputeMD5(source);

            if (hash != signature)
                throw new ApiAuthorizeException("signature is invalid");

            //valid request
            //LogHelper.Info(app.Name + "'s request #" + nonce);
        }

        static readonly string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        static string UrlEncode(string source)
        {
            if (null == source)
                return null;

            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var buffer = new StringBuilder();

            foreach (var b in Encoding.UTF8.GetBytes(source))
            {
                if (b < 128 && unreservedChars.IndexOf((char)b) != -1)
                {
                    buffer.Append((char)b);
                }
                else
                {
                    buffer.AppendFormat("%{0:X2}", b);
                }
            }

            return buffer.ToString();
        }
    }
}