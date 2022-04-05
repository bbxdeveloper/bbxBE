using bbxBE.Common.NAV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace bbxBE.Common
{
    public static class XMLUtil
    {


 

        /// <summary>
        /// Egy XmlNode-ban lévő elérési út értékével tér vissza. Ez lehet XmlAttribute és XmlElement is
        /// </summary>
        /// <param name="xmlNode">Megadott elem</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="defaultValue">Ha nem létezik az elérési út végén elem, akkor ezzel a megadott default értékkel tér vissza</param>
        /// <returns>Az elérési út végén lévő attribute vagy elem értéke számként</returns>
        public static int GetNodeValue(this XmlNode xmlNode, string xPath, int defaultValue)
        {
            string sValue = xmlNode.GetNodeValue(xPath, string.Empty);

            if (string.IsNullOrEmpty(sValue)) return defaultValue;

            int value;

            if (int.TryParse(sValue, out value)) return value;

            return defaultValue;
        }

        /// <summary>
        /// Egy XmlNode-ban lévő elérési út értékével tér vissza. Ez lehet XmlAttribute és XmlElement is
        /// </summary>
        /// <param name="xmlNode">Megadott elem</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="defaultValue">Ha nem létezik az elérési út végén elem, akkor ezzel a megadott default értékkel tér vissza</param>
        /// <returns>Az elérési út végén lévő attribute vagy elem értéke decimálisan</returns>
        public static decimal GetNodeValue(this XmlNode xmlNode, string xPath, decimal defaultValue)
        {
            string sValue = xmlNode.GetNodeValue(xPath, string.Empty);

            if (string.IsNullOrEmpty(sValue)) return defaultValue;

            decimal value;

            if (decimal.TryParse(sValue.Replace(".", ","), out value)) return value;

            return defaultValue;
        }

        /// <summary>
        /// Egy XmlNode-ban lévő elérési út értékével tér vissza. Ez lehet XmlAttribute és XmlElement is
        /// </summary>
        /// <param name="xmlNode">Megadott elem</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="defaultValue">Ha nem létezik az elérési út végén elem, akkor ezzel a megadott default értékkel tér vissza</param>
        /// <returns>Az elérési út végén lévő attribute vagy elem értéke bool formában</returns>
        public static bool GetNodeValue(this XmlNode xmlNode, string xPath, bool defaultValue)
        {
            string sValue = xmlNode.GetNodeValue(xPath, string.Empty);

            if (string.IsNullOrEmpty(sValue)) return defaultValue;

            if (sValue == "0") return true;
            if (sValue == "1") return false;

            bool value;

            if (bool.TryParse(sValue, out value)) return value;

            return defaultValue;
        }

        /// <summary>
        /// Egy XmlNode-ban lévő elérési út értékével tér vissza. Ez lehet XmlAttribute és XmlElement is
        /// </summary>
        /// <param name="xmlNode">Megadott elem</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="defaultValue">Ha nem létezik az elérési út végén elem, akkor ezzel a megadott default értékkel tér vissza</param>
        /// <returns>Az elérési út végén lévő attribute vagy elem értéke szövegesen</returns>
        public static string GetNodeValue(this XmlNode xmlNode, string xPath, string defaultValue)
        {
            XmlNode node = xmlNode.SelectSingleNode(xPath);

            if (node == null) return defaultValue;

            return node.InnerText;
        }

        /// <summary>
        /// Egy XmlNode-ban lévő elérési út értékével tér vissza. Ez lehet XmlAttribute és XmlElement is
        /// </summary>
        /// <param name="xmlNode">Megadott elem</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="defaultValue">Ha nem létezik az elérési út végén elem, akkor ezzel a megadott default értékkel tér vissza</param>
        /// <returns>Az elérési út végén lévő attribute vagy elem értéke dátum formában</returns>
        public static DateTime GetNodeValue(this XmlNode xmlNode, string xPath, DateTime defaultValue)
        {
            string sValue = xmlNode.GetNodeValue(xPath, string.Empty);

            if (string.IsNullOrEmpty(sValue)) return defaultValue;

            DateTime value;

            if (DateTime.TryParse(sValue, out value)) return value;

            return defaultValue;
        }

        /// <summary>
        /// Egy XmlNode tag megadott útvonalon lévő értékét beállítja
        /// </summary>
        /// <param name="node">A megadott XmlNode</param>
        /// <param name="xPath">Elérési út</param>
        /// <param name="value">Új érték</param>
        /// <returns>Visszatér az elérési út végén lévő XmlNode tag-el</returns>
        public static XmlNode SetNodeValue(this XmlNode node, string xPath, object value)
        {
            foreach (string path in xPath.Trim('/').Split('/'))
            {
                XmlNode pathNode = node.SelectSingleNode(path);

                if (pathNode == null)
                {
                    if (path.StartsWith("@"))
                    {
                        pathNode = node.OwnerDocument.CreateAttribute(path.TrimStart('@'));
                        node.Attributes.Append(pathNode as XmlAttribute);
                    }
                    else
                    {
                        pathNode = node.OwnerDocument.CreateElement(path);
                        node.AppendChild(pathNode);
                    }
                }

                node = pathNode;
            }

            if (value != null)
                node.InnerText = value.ToString();

            return node;
        }

        /// <summary>
        /// Egy XmlNodeList enumerálható listájával tér vissza
        /// </summary>
        public static IEnumerable<XmlNode> ToEnumerable(this XmlNodeList xmlNodeList)
        {
            if (xmlNodeList != null)
            {
                foreach (XmlNode xmlNode in xmlNodeList)
                    yield return xmlNode;
            }
        }

    }
}
