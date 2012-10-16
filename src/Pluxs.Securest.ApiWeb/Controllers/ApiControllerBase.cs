using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Pluxs.Securest.ApiWeb
{
    public class ApiControllerBase : Controller
    {
        public static readonly string XMLFormat = "xml";
        public static readonly string JSONFormat = "json";
        public static readonly string JSONPFormat = "jsonp";
        public static readonly string JSONContentType = "application/json";
        public static readonly string JavascriptContentType = "application/x-javascript";
        public static readonly string XMLContentType = "application/xml";
        public static readonly Encoding ContentEncoding = Encoding.UTF8;

        /// <summary>
        /// Indicates the max json string length 4MB.
        /// </summary>
        static readonly int _maxJsonLength = 8388608;//=2097152 * 4;//4MB * 4


        public ActionResult ApiResult(object data)
        {
            if (data == null)
                data = new { };

            var format = Request.GetParameter("format");

            if (string.IsNullOrEmpty(format))
                format = FormatFromAcceptTypes(Request.AcceptTypes);

            var callback = Request.GetParameter("callback");
            if (!string.IsNullOrEmpty(callback))
                format = JSONPFormat;

            if (JSONPFormat == format.ToLower() && string.IsNullOrEmpty(callback))
                callback = string.Format("callback_{0:0}", (DateTime.Now.ToUnixSeconds() * 1000));

            switch (format.ToLower())
            {
                case "xml":
                    return this.XmlResultFromData(data);
                case "json":
                    return this.JsonResultFromData(data);
                case "jsonp":
                    return this.JsonpResultFromData(data, callback);
                default:
                    throw new ApiException("The format '" + format + "' is not supported, use 'json'(Default), 'xml' or 'jsonp'('callback' required) instead", HttpStatusCode.BadRequest);
            }
        }

        protected virtual ActionResult XmlResultFromData(object data)
        {
            return new ContentResult
            {
                Content = data.ToXmlString(),
                ContentType = ApiControllerBase.XMLContentType,
                ContentEncoding = ApiControllerBase.ContentEncoding
            };
        }

        protected virtual ActionResult JsonResultFromData(object data)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = _maxJsonLength;
            serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
            var json = serializer.Serialize(data);

            return new ContentResult
            {
                ContentType = ApiControllerBase.JSONContentType,
                ContentEncoding = ApiControllerBase.ContentEncoding,
                Content = json
            };
        }

        protected virtual ActionResult JsonpResultFromData(object data, string callback)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = _maxJsonLength;
            serializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });
            var json = serializer.Serialize(data);

            return new ContentResult
            {
                ContentType = ApiControllerBase.JavascriptContentType,
                ContentEncoding = ApiControllerBase.ContentEncoding,
                Content = string.Format("{0}({1});", callback, json)
            };
        }

        static string FormatFromAcceptTypes(params string[] acceptTypes)
        {
            if (acceptTypes.Where(t => t.ToLower().Contains(XMLFormat)).Count() > 0)
                return XMLFormat;

            if (acceptTypes.Where(t => t.ToLower().Contains(JSONFormat)).Count() > 0)
                return JSONFormat;

            if (acceptTypes.Where(t => t.ToLower().Contains(JSONPFormat)).Count() > 0)
                return JSONPFormat;

            return XMLFormat;
        }

    }
}
