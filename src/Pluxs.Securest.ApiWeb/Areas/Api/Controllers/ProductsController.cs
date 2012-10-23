using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pluxs.Securest.ApiWeb.Areas.Api.Controllers
{
    [ApiAuthorize]
    [Compress]
    public class ProductsController : ApiControllerBase
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult NewArrivals(int count = 25)
        {
            return ApiResult(Enumerable.Range(1, count));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Submit(string productName, decimal price)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ApiParameterMissingException("productName");

            return ApiResult(new { name = productName, price = price, discount = 0.9 });
        }
    }
}