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
using System.Xml;
using System.Xml.Linq;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    public class XSerializer<T> 
        where T : class
    {
        #region Fields
        static readonly MethodInfo PropertyGetterHelperMethod;
        static readonly MethodInfo PropertySetterHelperMethod;
        static List<SerializableProperty> _properties;
        static SerializableClass<T> _class;
        #endregion

        static XSerializer()
        {
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
            _properties = new List<SerializableProperty>();

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

                _properties.Add(serializableProperty);
            }
        }

        public static void Serialize(Stream stream, T obj)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                Debug.Assert(writer != null);

                writer.WriteStartDocument();

                writer.WriteStartElement(_class.Name.LocalName,
                                         _class.Name.NamespaceName);

                foreach (var property in _properties)
                    WriteProperty(writer, property, obj);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
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
