using System;
using System.Xml.Serialization;

namespace FindXMLDifference
{
    [XmlRoot(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class element
    {

        [XmlElement(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public complexType complexType { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlIgnore]
        public int minOccurs { get; set; }

        [XmlIgnore]
        public string maxOccurs { get; set; }

        [XmlAttribute(AttributeName = "type", Namespace = "")]
        public string type { get; set; }

        [XmlAttribute(AttributeName = "nillable", Namespace = "")]
        public bool nillable { get; set; }
    }

    [XmlRoot(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class sequence
    {

        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<element> element { get; set; }
    }

    [XmlRoot(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class complexType
    {

        [XmlElement(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public sequence sequence { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlElement(ElementName = "complexContent", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public complexContent complexContent { get; set; }

        [XmlAttribute(AttributeName = "abstract", Namespace = "")]
        public bool @abstract { get; set; }
    }

    [XmlRoot(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class enumeration
    {

        [XmlAttribute(AttributeName = "value", Namespace = "")]
        public string value { get; set; }
    }

    [XmlRoot(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class restriction
    {

        [XmlElement(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<enumeration> enumeration { get; set; }

        [XmlAttribute(AttributeName = "base", Namespace = "")]
        public string @base { get; set; }
    }

    [XmlRoot(ElementName = "simpleType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class simpleType
    {

        [XmlElement(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public restriction restriction { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }
    }

    [XmlRoot(ElementName = "extension", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class extension
    {

        [XmlElement(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public sequence sequence { get; set; }

        [XmlAttribute(AttributeName = "base", Namespace = "")]
        public string @base { get; set; }
    }

    [XmlRoot(ElementName = "complexContent", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class complexContent
    {

        [XmlElement(ElementName = "extension", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public extension extension { get; set; }

        [XmlAttribute(AttributeName = "mixed", Namespace = "")]
        public bool mixed { get; set; }
    }

    [XmlRoot(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class schema
    {

        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<element> element { get; set; }

        [XmlElement(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<complexType> complexType { get; set; }

        [XmlElement(ElementName = "simpleType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public simpleType simpleType { get; set; }

        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string xs { get; set; }

        [XmlAttribute(AttributeName = "elementFormDefault", Namespace = "")]
        public string elementFormDefault { get; set; }

        [XmlAttribute(AttributeName = "targetNamespace", Namespace = "")]
        public string targetNamespace { get; set; }
    }

    [XmlRoot(ElementName = "types", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class types
    {

        [XmlElement(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public schema schema { get; set; }
    }

    [XmlRoot(ElementName = "part", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class part
    {

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "element", Namespace = "")]
        public string element { get; set; }
    }

    [XmlRoot(ElementName = "message", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class message
    {

        [XmlElement(ElementName = "part", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public part part { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }
    }

    [XmlRoot(ElementName = "input", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class input
    {

        [XmlAttribute(AttributeName = "Action", Namespace = "http://www.w3.org/2006/05/addressing/wsdl")]
        public string Action { get; set; }

        [XmlAttribute(AttributeName = "message", Namespace = "")]
        public string message { get; set; }

        [XmlElement(ElementName = "body", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
        public body body { get; set; }
    }

    [XmlRoot(ElementName = "output", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class output
    {

        [XmlAttribute(AttributeName = "Action", Namespace = "http://www.w3.org/2006/05/addressing/wsdl")]
        public string Action { get; set; }

        [XmlAttribute(AttributeName = "message", Namespace = "")]
        public string message { get; set; }

        [XmlElement(ElementName = "body", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
        public body body { get; set; }
    }

    [XmlRoot(ElementName = "operation", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class operation
    {

        [XmlElement(ElementName = "input", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public input input { get; set; }

        [XmlElement(ElementName = "output", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public output output { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "soapAction", Namespace = "")]
        public string soapAction { get; set; }

        [XmlAttribute(AttributeName = "style", Namespace = "")]
        public string style { get; set; }

        [XmlElement(ElementName = "operation", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
        public operation Operation { get; set; }
    }

    [XmlRoot(ElementName = "portType", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class portType
    {

        [XmlElement(ElementName = "operation", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public List<operation> operation { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }
    }

    [XmlRoot(ElementName = "binding", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
    public class binding
    {

        [XmlAttribute(AttributeName = "transport", Namespace = "")]
        public string transport { get; set; }

        [XmlElement(ElementName = "binding", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
        public binding Binding { get; set; }

        [XmlElement(ElementName = "operation", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public List<operation> operation { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "type", Namespace = "")]
        public string type { get; set; }
    }

    [XmlRoot(ElementName = "body", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
    public class body
    {

        [XmlAttribute(AttributeName = "use", Namespace = "")]
        public string use { get; set; }
    }

    [XmlRoot(ElementName = "address", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
    public class address
    {

        [XmlAttribute(AttributeName = "location", Namespace = "")]
        public string location { get; set; }
    }

    [XmlRoot(ElementName = "port", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class port
    {

        [XmlElement(ElementName = "address", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
        public address address { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "binding", Namespace = "")]
        public string binding { get; set; }
    }

    [XmlRoot(ElementName = "service", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class service
    {

        [XmlElement(ElementName = "port", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public port port { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }
    }

    [XmlRoot(ElementName = "definitions", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class definitions
    {

        [XmlElement(ElementName = "types", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public types types { get; set; }

        [XmlElement(ElementName = "message", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public List<message> message { get; set; }

        [XmlElement(ElementName = "portType", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public portType portType { get; set; }

        [XmlElement(ElementName = "binding", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public binding binding { get; set; }

        [XmlElement(ElementName = "service", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public service service { get; set; }

        [XmlAttribute(AttributeName = "wsdl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsdl { get; set; }

        [XmlAttribute(AttributeName = "wsam", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsam { get; set; }

        [XmlAttribute(AttributeName = "wsx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsx { get; set; }

        [XmlAttribute(AttributeName = "wsap", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsap { get; set; }

        [XmlAttribute(AttributeName = "msc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string msc { get; set; }

        [XmlAttribute(AttributeName = "wsp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsp { get; set; }

        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string xsd { get; set; }

        [XmlAttribute(AttributeName = "soap", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string soap { get; set; }

        [XmlAttribute(AttributeName = "wsu", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsu { get; set; }

        [XmlAttribute(AttributeName = "soap12", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string soap12 { get; set; }

        [XmlAttribute(AttributeName = "soapenc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string soapenc { get; set; }

        [XmlAttribute(AttributeName = "tns", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string tns { get; set; }

        [XmlAttribute(AttributeName = "wsa10", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsa10 { get; set; }

        [XmlAttribute(AttributeName = "wsaw", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsaw { get; set; }

        [XmlAttribute(AttributeName = "wsa", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string wsa { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "targetNamespace", Namespace = "")]
        public string targetNamespace { get; set; }
    }



}
