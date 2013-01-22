using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Pluxs.Securest.ApiWeb
{
    public static class XmlExtensions
    {
        static readonly Type[] writableTypes = new[] 
        {
            typeof(string), 
            typeof(DateTime), 
            typeof(DateTime?),
            typeof(Enum), 
            typeof(decimal), 
            typeof(Guid)
        };

        static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || writableTypes.Contains(type);
        }

        static bool IsEnumerable(this Type type)
        {
            return (type.GetInterface("IEnumerable", true) != null);
        }

        static XElement ExpandoToXML(dynamic node, String nodeName)
        {
            XElement xmlNode = new XElement(nodeName);

            foreach (var property in (IDictionary<String, Object>)node)
            {
                if (property.Value.GetType() == typeof(ExpandoObject))
                    xmlNode.Add(ExpandoToXML(property.Value, property.Key));

                else
                    if (property.Value.GetType() == typeof(List<dynamic>))
                        foreach (var element in (List<dynamic>)property.Value)
                            xmlNode.Add(ExpandoToXML(element, property.Key));
                    else
                        xmlNode.Add(new XElement(property.Key, property.Value));
            }

            return xmlNode;
        }
        /// <summary>
        /// 将一个对象转换成 XML 字符串。
        /// </summary>
        /// <param name="data">要转换的对象。</param>
        /// <returns></returns>
        public static string ToXmlString(this object data)
        {
            return ToXmlString(data, null);
        }

        /// <summary>
        /// 将一个对象转换成 XML 字符串。
        /// </summary>
        /// <param name="data">要转换的对象。</param>
        /// <param name="root">XML 根元素名称。</param>
        /// <returns></returns>
        public static string ToXmlString(this object data, string root)
        {
            return ToXElement(data, root).ToString();
        }

        /// <summary>
        /// 将一个对象转换成 XElement 对象。
        /// </summary>
        /// <param name="data">要转换的对象。</param>
        /// <returns></returns>
        public static XElement ToXElement(this object data)
        {
            return ToXElement(data, null);
        }

        /// <summary>
        /// 将一个对象转换成 XElement 对象。
        /// </summary>
        /// <param name="data">要转换的对象。</param>
        /// <param name="root">XML 根元素名称。</param>
        /// <returns></returns>
        public static XElement ToXElement(this object data, string root)
        {
            if (data == null)
                return null;

            if (string.IsNullOrEmpty(root))
                root = "xml";

            var type = data.GetType();
            var props = type.GetProperties();

            root = XmlConvert.EncodeName(root);
            var xml = new XElement(root);

            if (typeof(ExpandoObject) == type)
            {
                var node = ExpandoToXML(data, root);
                xml = node;
            }
            else if (!type.IsSimpleType() && type.IsEnumerable())
            {
                // singularize the element name
                if (root.EndsWith("ies"))
                    root = root.Substring(0, root.Length - 3) + "y";
                else if (root.EndsWith("s") && root.Length > 1)
                    root = root.Substring(0, root.Length - 1);
                else
                    root = "item";

                foreach (object item in data as IEnumerable)
                {
                    xml.Add(item.GetType().IsSimpleType()
                        ? new XElement(root, item)
                        : item.ToXElement(root));
                }
            }
            else
            {
                var elements = from prop in props
                               let name = XmlConvert.EncodeName(prop.Name)
                               let val = prop.GetValue(data, null)
                               let value = prop.PropertyType.IsSimpleType()
                                       ? new XElement(name, val)
                                       : val.ToXElement(name)
                               where value != null
                               select value;

                xml.Add(elements);
            }

            return xml;
        }
    }
}