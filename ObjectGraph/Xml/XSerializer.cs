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

                // FIXME: Generate delegate for reading XML value without boxing, and writing XML value without boxing, pass to serializableproperty
                // FIXME: Maybe pre-generate delegates for primitive types to avoid too much reflection here?

                var serializableType = typeof(SerializableProperty<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
                var serializableTypeConstructor = serializableType.GetConstructor(new[]
                                                                              {
                                                                                  typeof(XName),
                                                                                  typeof(NodeType),
                                                                                  typeof(bool),
                                                                                  typeof(Type),
                                                                                  getter.GetType(),
                                                                                  setter.GetType(),
                                                                                  null, // FIXME
                                                                                  null // FIXME
                                                                              });

                var serializableProperty =
                        (SerializableProperty)serializableTypeConstructor.Invoke(new[]
                                                                                 {
                                                                                     info.Item1 ?? propertyInfo.Name,
                                                                                     info.Item2,
                                                                                     info.Item3,
                                                                                     propertyInfo.PropertyType,
                                                                                     getter,
                                                                                     setter,
                                                                                     null, // FIXME
                                                                                     null // FIXME
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

        static object PropertyGetter(MethodInfo method)
        {
            var constructedHelper = PropertyGetterHelperMethod.MakeGenericMethod(typeof(T), method.ReturnType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return ret;
        }

#pragma warning disable 0693
        static Func<T, TProperty> PropertyGetterHelper<T, TProperty>(MethodInfo method)
        {
            var func = (Func<T, TProperty>)Delegate.CreateDelegate(typeof(Func<T, TProperty>), method);
            Func<T, TProperty> ret = target => func(target);
            return ret;
        }
#pragma warning restore 0693

        static object PropertySetter(MethodInfo method)
        {
            var constructedHelper = PropertySetterHelperMethod.MakeGenericMethod(typeof(T), method.GetParameters()[0].ParameterType);
            var ret = constructedHelper.Invoke(null, new object[] { method });
            return ret;
        }

#pragma warning disable 0693
        static Action<T, TProperty> PropertySetterHelper<T, TProperty>(MethodInfo method)
        {
            var action = (Action<T, TProperty>)Delegate.CreateDelegate(typeof(Action<T, TProperty>), method);
            Action<T, TProperty> ret = (target, value) => action(target, value);
            return ret;
        }
#pragma warning restore 0693
    }
}
