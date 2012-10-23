using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pluxs.Securest.ApiWeb.Models
{
    public class EndpointInfo
    {
        public string Name { get; set; }
        public bool AuthorizeRequired { get; set; }
        public IEnumerable<EndpointActionInfo> Actions { get; set; }

        public string AuthorizeRequiredMessage
        {
            get { return this.AuthorizeRequired ? "ApiAuthorize" : string.Empty; }
        }
    }

    public class EndpointActionInfo
    {
        public string Name { get; set; }
        public string HttpMethod { get; set; }
        public bool AuthorizeRequired { get; set; }
        public IEnumerable<EndpointActionParameterInfo> Parameters { get; set; }

        public string AuthorizeRequiredMessage
        {
            get { return this.AuthorizeRequired ? "ApiAuthorize" : string.Empty; }
        }

        public string ParametersString
        {
            get { return string.Join(",", this.Parameters.Select(p => p.Name)); }
        }
    }

    public class EndpointActionParameterInfo
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
}