# Securest #
**A simple request signature based security REST service with ASP.NET MVC**

## Features ##

- Serve all requests as api request or custom prefix
- Support JSON/XML/JSONP data formats as response
- GZIP/Deflate compress support
- Auto documentation
- OAuth 1.0 like client authorization
- single user authorization not support currently

## Start ##

**Endpoint sample**


    [ApiAuthorize]
    [Compress]
    public class ProductsController : ApiControllerBase
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult NewArrivals()
        {
            var newArrivals = ProductsDataContext.NewArrivals;

            return ApiResult(newArrivals);
        }
    }



**Client sample**

`curl http://your_api_server/api/products/newarrivals`

## Thank you ##

**Any comments and feedbacks are welcome**