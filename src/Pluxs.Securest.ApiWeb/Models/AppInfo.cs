using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Pluxs.Securest.ApiWeb.Models
{
    public class AppInfo
    {
        public enum AppStatus
        {
            Closed = 0,
            Normal = 1
        }

        public string Name { get; private set; }
        public string ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        public AppStatus Status { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, this.ApiKey);
        }

        static readonly string _appStoragePath = "~/App_Data/apps.xml";
        static IEnumerable<AppInfo> _apps;
        public static IEnumerable<AppInfo> Apps
        {
            get
            {
                if (null == _apps)
                {
                    _apps = XElement.Load(HttpContext.Current.Server.MapPath(_appStoragePath))
                        .Elements("app").Select(x => new AppInfo
                        {
                            Name = x.Element("name").Value,
                            ApiKey = x.Element("appKey").Value,
                            ApiSecret = x.Element("appSecret").Value,
                            Status = (AppStatus)Enum.Parse(typeof(AppStatus), x.Element("status").Value, true)
                        });
                }

                return _apps;
            }
        }
    }
}