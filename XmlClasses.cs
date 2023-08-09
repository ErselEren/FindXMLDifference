using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlTypes;
using System.Xml.Linq;
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            element other = (element)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (name == other.name) && (type == other.type) && (nillable == other.nillable);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( complexType, name, type, nillable);
        }
    }

    [XmlRoot(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class sequence
    {

        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<element> element { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            sequence other = (sequence)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (element == other.element);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(element);
        }
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            complexType other = (complexType)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( name == other.name) && (@abstract == other.@abstract) && (sequence == other.sequence) && (complexContent == other.complexContent);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(name, @abstract, sequence, complexContent);   
        }
    }

    [XmlRoot(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class enumeration
    {

        [XmlAttribute(AttributeName = "value", Namespace = "")]
        public string value { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            enumeration other = (enumeration)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (value == other.value);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(value);
        }
    }

    [XmlRoot(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class restriction
    {

        [XmlElement(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<enumeration> enumeration { get; set; }

        [XmlAttribute(AttributeName = "base", Namespace = "")]
        public string @base { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            restriction other = (restriction)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (enumeration == other.enumeration) && (@base == other.@base);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(enumeration,@base);
        }
    }

    [XmlRoot(ElementName = "simpleType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class simpleType
    {

        [XmlElement(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public restriction restriction { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            simpleType other = (simpleType)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (restriction == other.restriction) && (name == other.name);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(restriction, name);
        }
    }

    [XmlRoot(ElementName = "extension", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class extension
    {

        [XmlElement(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public sequence sequence { get; set; }

        [XmlAttribute(AttributeName = "base", Namespace = "")]
        public string @base { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            extension other = (extension)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (sequence == other.sequence) && (@base == other.@base);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(sequence, @base);
        }
    }

    [XmlRoot(ElementName = "complexContent", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class complexContent
    {

        [XmlElement(ElementName = "extension", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public extension extension { get; set; }

        [XmlAttribute(AttributeName = "mixed", Namespace = "")]
        public bool mixed { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            complexContent other = (complexContent)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (extension == other.extension) && (mixed == other.mixed);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(extension, mixed);
        }
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            schema other = (schema)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (element == other.element) && (complexType == other.complexType) && (simpleType == other.simpleType) && (xs == other.xs) && (elementFormDefault == other.elementFormDefault) && (targetNamespace == other.targetNamespace);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( element, complexType, simpleType, xs, elementFormDefault, targetNamespace);
        }
    }

    [XmlRoot(ElementName = "types", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class types
    {

        [XmlElement(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public schema schema { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            types other = (types)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (schema == other.schema);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(schema);
        }
    }

    [XmlRoot(ElementName = "part", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class part
    {

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        [XmlAttribute(AttributeName = "element", Namespace = "")]
        public string element { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            part other = (part)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (name == other.name) && (element == other.element);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(name, element);
        }
    }

    [XmlRoot(ElementName = "message", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class message
    {

        [XmlElement(ElementName = "part", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public part part { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            message other = (message)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return (part == other.part) && (name == other.name);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(part, name);
        }
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


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            input other = (input)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( Action == other.Action) && (message == other.message) && (body == other.body);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( Action, message, body);
        }
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


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            output other = (output)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( Action == other.Action) && (message == other.message) && (body == other.body);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( Action, message, body);    
        }
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            operation other = (operation)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( input == other.input) && (output == other.output) && (name == other.name) && (soapAction == other.soapAction) && (style == other.style) && (Operation == other.Operation);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(input, output, name, soapAction, style, Operation); 
        }
    }

    [XmlRoot(ElementName = "portType", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class portType
    {

        [XmlElement(ElementName = "operation", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public List<operation> operation { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            portType other = (portType)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( operation == other.operation) && (name == other.name);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( operation, name);
        }
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            binding other = (binding)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( transport == other.transport) && (Binding == other.Binding) && (operation == other.operation) && (name == other.name) && (type == other.type);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( transport, Binding, operation, name, type);
        }
    }

    [XmlRoot(ElementName = "body", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
    public class body
    {

        [XmlAttribute(AttributeName = "use", Namespace = "")]
        public string use { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            body other = (body)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( use == other.use);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(use);
        }
    }

    [XmlRoot(ElementName = "address", Namespace = "http://schemas.xmlsoap.org/wsdl/soap/")]
    public class address
    {

        [XmlAttribute(AttributeName = "location", Namespace = "")]
        public string location { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            address other = (address)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( location == other.location);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine(location);
        }
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            port other = (port)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( address == other.address) && (name == other.name) && (binding == other.binding);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( address, name, binding);
        }
    }

    [XmlRoot(ElementName = "service", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
    public class service
    {

        [XmlElement(ElementName = "port", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
        public port port { get; set; }

        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            service other = (service)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( port == other.port) && (name == other.name);
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the properties used in the Equals method
            return HashCode.Combine( port , name);
        }
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


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            definitions other = (definitions)obj; // Cast the object to your class type

            // Compare the properties that define equality
            return ( types == other.types) && (message == other.message) && (portType == other.portType) && (binding == other.binding) && (service == other.service) && (wsdl == other.wsdl) && (wsam == other.wsam) && (wsx == other.wsx) && (wsap == other.wsap) && (msc == other.msc) && (wsp == other.wsp) && (xsd == other.xsd) && (soap == other.soap) && (wsu == other.wsu) && (soap12 == other.soap12) && (soapenc == other.soapenc) && (tns == other.tns) && (wsa10 == other.wsa10) && (wsaw == other.wsaw) && (wsa == other.wsa) && (name == other.name) && (targetNamespace == other.targetNamespace);
        }

    }



}
