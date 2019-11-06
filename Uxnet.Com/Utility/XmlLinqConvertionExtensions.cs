﻿using System.Xml;
using System.Xml.Linq;

namespace XmlLib
{
    /// <summary>
    /// Provides extension methods for simple conversion between System.Xml and System.Xml.Linq classes.
    /// </summary>
    /// <remarks>From: http://brianary.blogspot.com/2010/02/converting-between-xdocument-and.html</remarks>
    public static class XmlLinqConversionExtensions
    {
        /// <summary>
        /// Converts an XDocument to an XmlDocument.
        /// </summary>
        /// <param name="xdoc">The XDocument to convert.</param>
        /// <returns>The equivalent XmlDocument.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xdoc)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(xdoc.CreateReader());
            return xmldoc;
        }

        /// <summary>
        /// Convert an XElement to an XmlDocument.
        /// </summary>
        /// <param name="xelement"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this XElement xelement)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(xelement.CreateReader());
            return xmldoc;
        }

        /// <summary>
        /// Converts an XmlDocument to an XDocument.
        /// </summary>
        /// <param name="xmldoc">The XmlDocument to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(this XmlDocument xmldoc)
        {
            return XDocument.Load(xmldoc.CreateNavigator().ReadSubtree());
        }

        /// <summary>
        /// Converts an XmlDocument to an XElement.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static XElement ToXElement(this XmlDocument xmldoc)
        {
            return ToXDocument(xmldoc).Root;
        }

        /// <summary>
        /// Converts an XElement to an XmlElement.
        /// </summary>
        /// <param name="xelement">The XElement to convert.</param>
        /// <returns>The equivalent XmlElement.</returns>
        public static XmlElement ToXmlElement(this XElement xelement)
        {
            return new XmlDocument().ReadNode(xelement.CreateReader()) as XmlElement;
        }

        /// <summary>
        /// Converts an XmlElement to an XElement.
        /// </summary>
        /// <param name="xmlelement">The XmlElement to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static XElement ToXElement(this XmlElement xmlelement)
        {
            return XElement.Load(xmlelement.CreateNavigator().ReadSubtree());
        }
    }
}

