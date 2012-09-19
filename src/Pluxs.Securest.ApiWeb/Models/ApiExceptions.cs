using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluxs.Securest.ApiWeb
{
    public class ApiException : Exception
    {
        public int HttpStatusCode { get; private set; }

        public ApiException(string message, System.Net.HttpStatusCode statusCode)
            : base(message)
        {
            this.HttpStatusCode = (int)statusCode;
        }
    }

    public class ApiMethodNotAllowedException : ApiException
    {
        public ApiMethodNotAllowedException()
            : base("Http method is not allowed", System.Net.HttpStatusCode.MethodNotAllowed)
        {
        }
    }

    public class ApiParameterMissingException : ApiException
    {
        public ApiParameterMissingException(string paramName)
            : base("Parameter '" + paramName + "' is required and its value can not be empty or null", System.Net.HttpStatusCode.BadRequest)
        {
        }
    }

    public class ApiParameterBadFormatException : ApiException
    {
        public ApiParameterBadFormatException(string paramName, string paramValue, string expected)
            : base("The value of '" + paramName + "' is '" + paramValue + "' but expected " + expected, System.Net.HttpStatusCode.BadRequest)
        {
        }
    }

    public class ApiAuthorizeException : ApiException
    {
        public ApiAuthorizeException(string message)
            : base("Authorization failed: " + message, System.Net.HttpStatusCode.BadRequest)
        { }
    }
}