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
        static List<SerializableProperty<T>> _properties;
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
            _properties = new List<SerializableProperty<T>>();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var info = GetNodeInfo(propertyInfo);

                if (info == null)
                    continue;

                _properties.Add(new SerializableProperty<T>(info.Item1 ?? propertyInfo.Name,
                                                            info.Item2,
                                                            info.Item3,
                                                            propertyInfo.PropertyType,
                                                            PropertyGetter(propertyInfo.GetGetMethod()),
                                                            PropertySetter(propertyInfo.GetSetMethod())));
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

        static void WriteProperty(XmlWriter writer, SerializableProperty<T> property, T obj)
        {
            if (property.NodeType == NodeType.Attribute)
            {
                writer.WriteAttributeString(property.Name.LocalName,
                                            property.Name.NamespaceName,
                                            property.GetXmlValue(obj));
            }
            else if (property.NodeType == NodeType.Element)
            {
                writer.WriteElementString(property.Name.LocalName,
                                          property.Name.NamespaceName,
                                          property.GetXmlValue(obj));
            }
        }

        static Tuple<XName, NodeType, bool> GetNodeInfo(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
            {
                var serializableAttribute = memberInfo.GetCustomAttributes(typeof(XSerializableAttribute),
                                                                           false).FirstOrDefault() as XSerializableAttribute;

                if (serializableAttribute != null)
                    return Tuple.Create(serializableAttribute.Name, NodeType.Element, true);
            }
            else
            {
                var requiredAttribute = memberInfo.GetCustomAttributes(typeof(XRequiredAttribute),
                                                                       false).FirstOrDefault() as XRequiredAttribute;
                var optionalAttribute = memberInfo.GetCustomAttributes(typeof(XOptionalAttribute),
                                                                       false).FirstOrDefault() as XOptionalAttribute;

                if (requiredAttribute != null && optionalAttribute != null)
                    throw new NotSupportedException("Only one of [Required] or [Optional] is supported on a particular property.");

                if (requiredAttribute != null)
                    return Tuple.Create(requiredAttribute.Name,
                                        requiredAttribute.Type,
                                        true);
                if (optionalAttribute != null)
                    return Tuple.Create(optionalAttribute.Name,
                                        optionalAttribute.Type,
                                        false);
            }

            return null;
        }

        // See: http://msmvps.com/blogs/jon_skeet/archive/2008/08/09/making-reflection-fly-and-exploring-delegates.aspx

        static Func<T, object> PropertyGetter(MethodInfo method)
        {
            var constructedHelper = PropertyGetterHelperMethod.MakeGenericMethod(typeof(T), method.ReturnType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return (Func<T, object>)ret;
        }

#pragma warning disable 0693
        static Func<T, object> PropertyGetterHelper<T, TReturn>(MethodInfo method)
        {
            var func = (Func<T, TReturn>)Delegate.CreateDelegate(typeof(Func<T, TReturn>), method);
            Func<T, object> ret = target => func(target);
            return ret;
        }
#pragma warning restore 0693

        static Action<T, object> PropertySetter(MethodInfo method)
        {
            var constructedHelper = PropertySetterHelperMethod.MakeGenericMethod(typeof(T), method.GetParameters()[0].ParameterType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return (Action<T, object>)ret;
        }

#pragma warning disable 0693
        static Action<T, object> PropertySetterHelper<T, TParam>(MethodInfo method)
        {
            var action = (Action<T, TParam>)Delegate.CreateDelegate(typeof(Action<T, TParam>), method);
            Action<T, object> ret = (target, value) => action(target, (TParam)value);
            return ret;
        }
#pragma warning restore 0693
    }
}
