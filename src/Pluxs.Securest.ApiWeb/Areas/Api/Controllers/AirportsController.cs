using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace Pluxs.Securest.ApiWeb.Areas.Api.Controllers
{
    /// <summary>
    /// airports service
    /// </summary>
    [ApiAuthorize]
    [Compress]
    public class AirportsController : ApiControllerBase
    {
        [AcceptVerbs("GET")]
        public System.Web.Mvc.ActionResult Geo()
        {
            var xmlFile = Server.MapPath("~/App_Data/airports.xml");

            var airports = XElement.Load(xmlFile).Elements("airport")
                .Select(x =>
                    new
                    {
                        code = x.Element("code").Value,
                        name = x.Element("name").Value,
                        pinyin = x.Element("pinyin").Value,
                        py = x.Element("py").Value,
                        lat = double.Parse(x.Element("lat").Value),
                        lng = double.Parse(x.Element("lng").Value)
                    });

            return ApiResult(airports);
        }
    }
}