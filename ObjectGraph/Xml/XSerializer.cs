//
// Copyright (C) 2010 Leon Breedt
// bitserf -at- gmail [dot] com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.using System;
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    public class XSerializer<T> 
        where T : class, new()
    {
        #region Fields
        static readonly XmlWriterSettings WriterSettings;
        static readonly MethodInfo PropertyGetterHelperMethod;
        static readonly MethodInfo PropertySetterHelperMethod;
        static Dictionary<string, SerializableProperty> _propertiesByName;
        static SerializableClass<T> _class;
        #endregion

        static XSerializer()
        {
            WriterSettings = new XmlWriterSettings {Encoding = new UTF8Encoding(false), OmitXmlDeclaration = true};
            PropertyGetterHelperMethod = typeof(XSerializer<T>).GetMethod("PropertyGetterHelper", BindingFlags.Static | BindingFlags.NonPublic);
            PropertySetterHelperMethod = typeof(XSerializer<T>).GetMethod("PropertySetterHelper", BindingFlags.Static | BindingFlags.NonPublic);

            BuildClass();
            BuildProperties();
        }

        static void BuildClass()
        {
            var type = typeof(T);
            var info = GetNodeInfo(type);

            if (info == null)
                throw new ArgumentException("Class {0} must have the [XSerializable] attribute in order to be serialized to XML.".FormatWith(type.Name));

            _class = new SerializableClass<T>(info.Item1 ?? type.Name);
        }

        static void BuildProperties()
        {
            _propertiesByName = new Dictionary<string, SerializableProperty>();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var info = GetNodeInfo(propertyInfo);

                if (info == null)
                    continue;

                var getter = PropertyGetter(propertyInfo.GetGetMethod());
                var setter = PropertySetter(propertyInfo.GetSetMethod());

                object xmlReader;
                object xmlWriter;

                var propertyType = propertyInfo.PropertyType;
                var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                var typeCode = Type.GetTypeCode(type);

                if (typeCode != TypeCode.Empty && 
                    typeCode != TypeCode.Object && 
                    typeCode != TypeCode.DBNull)
                {
                    if (type.IsEnum)
                    {
                        xmlReader = XBasicConverter.ForReadingEnum(type);
                        xmlWriter = XBasicConverter.ForWritingEnum(type);
                    }
                    else
                    {
                        xmlReader = XBasicConverter.ForReading(typeCode);
                        xmlWriter = XBasicConverter.ForWriting(typeCode);
                    }
                }
                else
                    throw new NotSupportedException("Non-primitive types are not yet supported");

                var serializableType = typeof(SerializableProperty<,>).MakeGenericType(typeof(T), propertyType);
                var serializableTypeConstructor = serializableType.GetConstructor(new[]
                                                                                      {
                                                                                          typeof(XName),
                                                                                          typeof(NodeType),
                                                                                          typeof(bool),
                                                                                          typeof(Type),
                                                                                          getter.GetType(),
                                                                                          setter.GetType(),
                                                                                          xmlReader.GetType(),
                                                                                          xmlWriter.GetType()
                                                                                      });

                var serializableProperty =
                        (SerializableProperty)serializableTypeConstructor.Invoke(new[]
                                                                                 {
                                                                                     info.Item1 ?? propertyInfo.Name,
                                                                                     info.Item2,
                                                                                     info.Item3,
                                                                                     propertyType,
                                                                                     getter,
                                                                                     setter,
                                                                                     xmlReader,
                                                                                     xmlWriter
                                                                                 });

                _propertiesByName.Add(serializableProperty.Name.LocalName, serializableProperty);
            }
        }

        public static void Serialize(Stream stream, T obj)
        {
            using (var writer = XmlWriter.Create(stream, WriterSettings))
            {
                Debug.Assert(writer != null);

                writer.WriteStartDocument();

                writer.WriteStartElement(_class.Name.LocalName,
                                         _class.Name.NamespaceName);

                foreach (var property in _propertiesByName.Values)
                    WriteProperty(writer, property, obj);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }

        public static T Deserialize(Stream stream)
        {
            using (var reader = XmlReader.Create(stream))
            {
                Debug.Assert(reader != null);

                return ReadObjectFromElement(reader);
            }
        }

        static T ReadObjectFromElement(XmlReader reader)
        {
            var obj = new T();

            var elementName = _class.Name.LocalName;
            var namespaceName = _class.Name.NamespaceName;

            while (reader.NodeType != XmlNodeType.Element && reader.Read())
                ;

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (string.Compare(reader.Name, elementName, false) != 0 || string.Compare(reader.NamespaceURI, namespaceName, false) != 0)
                    ThrowXmlFormatException(reader, "Expected element '{0}', namespace '{1}'".FormatWith(elementName, namespaceName));

                if (reader.HasAttributes)
                {
                    for (int i = 0; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToAttribute(i);
                        var property = TryGetSerializableProperty(reader.Name, reader.NamespaceURI);
                        if (property != null)
                            property.SetValueFromXml(obj, reader.Value);
                    }
                }
            }
            else
                ThrowXmlFormatException(reader, "Expected element '{0}', namespace '{1}'".FormatWith(elementName, namespaceName));

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                }
            }

            return obj;
        }

        static void ThrowXmlFormatException(XmlReader reader, string message)
        {
            var lineInfo = reader.GetLineInfo();
            throw new FormatException("({0}:{1}) {2}".FormatWith(lineInfo.Item1, lineInfo.Item2, message));
        }

        static SerializableProperty TryGetSerializableProperty(string name, string namespaceUri)
        {
            SerializableProperty property;

            if (!_propertiesByName.TryGetValue(name, out property))
                return null;

            if (!string.IsNullOrWhiteSpace(namespaceUri))
            {
                if (string.Compare(namespaceUri, property.Name.NamespaceName, false) != 0)
                    return null;
            }

            return property;
        }

        static void WriteProperty(XmlWriter writer, SerializableProperty property, T obj)
        {
            if (property.NodeType == NodeType.Attribute)
            {
                writer.WriteAttributeString(property.Name.LocalName,
                                            property.Name.NamespaceName,
                                            property.GetValueForXml(obj));
            }
            else if (property.NodeType == NodeType.Element)
            {
                writer.WriteElementString(property.Name.LocalName,
                                          property.Name.NamespaceName,
                                          property.GetValueForXml(obj));
            }
        }

        static Tuple<XName, NodeType, bool> GetNodeInfo(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
            {
                var infoAttribute = memberInfo.GetCustomAttributes(typeof(XInfoAttribute),
                                                   false).FirstOrDefault() as XInfoAttribute;

                var contractAttribute = memberInfo.GetCustomAttributes(typeof(DataContractAttribute),
                                                                           false).FirstOrDefault() as DataContractAttribute;

                if (contractAttribute != null)
                    return Tuple.Create(infoAttribute != null ? infoAttribute.Name : contractAttribute.Name,
                                        NodeType.Element,
                                        true);
            }
            else
            {
                var infoAttribute = memberInfo.GetCustomAttributes(typeof(XInfoAttribute),
                                                                   false).FirstOrDefault() as XInfoAttribute;

                var memberAttribute = memberInfo.GetCustomAttributes(typeof(DataMemberAttribute),
                                                                       false).FirstOrDefault() as DataMemberAttribute;

                if (memberAttribute != null)
                {
                    return Tuple.Create(infoAttribute != null ? infoAttribute.Name : memberAttribute.Name,
                                        infoAttribute != null ? infoAttribute.Type : NodeType.Attribute,
                                        infoAttribute != null ? infoAttribute.IsRequired : false);
                }
            }

            return null;
        }

        // See: http://msmvps.com/blogs/jon_skeet/archive/2008/08/09/making-reflection-fly-and-exploring-delegates.aspx

        static object PropertyGetter(MethodInfo method)
        {
            var constructedHelper = PropertyGetterHelperMethod.MakeGenericMethod(typeof(T), method.ReturnType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return ret;
        }

#pragma warning disable 0693
// ReSharper disable UnusedMember.Local
        static Func<T, TProperty> PropertyGetterHelper<T, TProperty>(MethodInfo method)
// ReSharper restore UnusedMember.Local
        {
            return (Func<T, TProperty>)Delegate.CreateDelegate(typeof(Func<T, TProperty>), method);
        }
#pragma warning restore 0693

        static object PropertySetter(MethodInfo method)
        {
            var constructedHelper = PropertySetterHelperMethod.MakeGenericMethod(typeof(T), method.GetParameters()[0].ParameterType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return ret;
        }

#pragma warning disable 0693
// ReSharper disable UnusedMember.Local
        static Action<T, TProperty> PropertySetterHelper<T, TProperty>(MethodInfo method)
// ReSharper restore UnusedMember.Local
        {
            return (Action<T, TProperty>)Delegate.CreateDelegate(typeof(Action<T, TProperty>), method);
        }
#pragma warning restore 0693
    }
}
