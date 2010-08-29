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
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    internal class PropertySerializer
    {
        #region Fields
        static Dictionary<PropertyInfo, PropertySerializer> _serializersByPropertyInfo;
        static MethodInfo _buildMethodT;
        readonly XName _name;
        #endregion

        static PropertySerializer()
        {
            _serializersByPropertyInfo = new Dictionary<PropertyInfo, PropertySerializer>();
            _buildMethodT = typeof(PropertySerializer).GetMethod("Build",
                                                                 BindingFlags.Static | BindingFlags.Public,
                                                                 null,
                                                                 new[] {typeof(PropertyInfo)},
                                                                 null);
        }

        public PropertySerializer(XName name)
        {
            _name = name;
        }

        public XName Name { get { return _name; } }

        public static IPropertySerializer<TDeclaringType> Build<TDeclaringType>(Type propertyType, PropertyInfo info)
        {
            return (IPropertySerializer<TDeclaringType>)_buildMethodT.MakeGenericMethod(typeof(TDeclaringType), propertyType).Invoke(null, new[] { info });
        }

        public static IPropertySerializer<TDeclaringType> Build<TDeclaringType, TPropertyType>(PropertyInfo info)
        {
            lock (_serializersByPropertyInfo)
            {
                PropertySerializer serializer;

                if (_serializersByPropertyInfo.TryGetValue(info, out serializer))
                    return (IPropertySerializer<TDeclaringType>)serializer;

                var attr = info.GetAttribute<DataMemberAttribute>();
                if (attr == null)
                    throw new NotSupportedException("Property {0} must have a [DataMember] attribute to be serializable".FormatWith(info.Name));

                XName name = attr.Name ?? info.Name;

                var propertyType = info.PropertyType;

                var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                var typeCode = Type.GetTypeCode(type);

                var getter = (Func<TDeclaringType, TPropertyType>)Delegate.CreateDelegate(typeof(Func<TDeclaringType, TPropertyType>), info.GetGetMethod());
                var setter = (Action<TDeclaringType, TPropertyType>)Delegate.CreateDelegate(typeof(Action<TDeclaringType, TPropertyType>), info.GetSetMethod());

                Func<string, TPropertyType> xmlReadFunc;
                Func<TPropertyType, string> xmlWriteFunc;

                if (typeCode != TypeCode.Empty &&
                    typeCode != TypeCode.Object &&
                    typeCode != TypeCode.DBNull)
                {
                    if (type.IsEnum)
                    {
                        xmlReadFunc = (Func<string, TPropertyType>)BasicConverter.ForReadingEnum(type);
                        xmlWriteFunc = (Func<TPropertyType, string>)BasicConverter.ForWritingEnum(type);
                    }
                    else
                    {
                        xmlReadFunc = (Func<string, TPropertyType>)BasicConverter.ForReading(typeCode);
                        xmlWriteFunc = (Func<TPropertyType, string>)BasicConverter.ForWriting(typeCode);
                    }
                }
                else
                {
                    if (typeCode == TypeCode.Object)
                    {
                        xmlReadFunc = null;
                        xmlWriteFunc = null;
                    }
                    else
                        throw new InvalidOperationException("Type {0} is not supported".FormatWith(typeCode));
                }

                if (typeCode == TypeCode.Object)
                {
                    var serializerType = typeof(ComplexPropertySerializer<,>).MakeGenericType(typeof(TDeclaringType), propertyType);
                    var propertyValueSerializerType = typeof(TypeSerializer<>).MakeGenericType(typeof(TPropertyType));
                    var propertyValueSerializer = TypeSerializer.Build(typeof(TPropertyType), name);

                    var constructor = serializerType.GetConstructor(new[]
                                                            {
                                                                typeof(XName),
                                                                typeof(Func<TDeclaringType, TPropertyType>),
                                                                typeof(Action<TDeclaringType, TPropertyType>),
                                                                propertyValueSerializerType,
                                                            });

                    serializer = (PropertySerializer)constructor.Invoke(new object[] {name, getter, setter, propertyValueSerializer});
                    _serializersByPropertyInfo[info] = serializer;
                }
                else
                {
                    var serializerType = typeof(SimplePropertySerializer<,>).MakeGenericType(typeof(TDeclaringType), propertyType);


                    var constructor = serializerType.GetConstructor(new[]
                                                            {
                                                                typeof(XName),
                                                                typeof(Func<TDeclaringType, TPropertyType>),
                                                                typeof(Action<TDeclaringType, TPropertyType>),
                                                                typeof(Func<string, TPropertyType>),
                                                                typeof(Func<TPropertyType, string>),
                                                            });

                    serializer = (PropertySerializer)constructor.Invoke(new object[] { name, getter, setter, xmlReadFunc, xmlWriteFunc });
                    _serializersByPropertyInfo[info] = serializer;
                }

                return (IPropertySerializer<TDeclaringType>)serializer;
            }
        }
    }
}
