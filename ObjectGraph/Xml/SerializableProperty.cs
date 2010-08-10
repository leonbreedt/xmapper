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
using System.Xml;
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    internal class SerializableProperty<T> 
        where T : class
    {
        public SerializableProperty(XName name, NodeType nodeType, bool isRequired, Type propertyType, Func<T, object> getter, Action<T, object> setter)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType);

            Name = name;
            NodeType = nodeType;
            IsRequired = isRequired;
            IsNullable = underlyingType != null;
            PropertyType = propertyType;
            PropertyTypeCode = underlyingType != null ? Type.GetTypeCode(underlyingType) : Type.GetTypeCode(propertyType);
            Getter = getter;
            Setter = setter;
        }

        public XName Name { get; set; }
        public NodeType NodeType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsNullable { get; set; }
        public Type PropertyType { get; set; }
        public TypeCode PropertyTypeCode { get; set; }
        public Func<T, object> Getter { get; set; }
        public Action<T, object> Setter { get; set; }

        public string GetXmlValue(T obj)
        {
            var value = Getter(obj);
            if (value == null)
                return null;

            switch (PropertyTypeCode)
            {
                case TypeCode.Boolean:
                    return XmlConvert.ToString((bool)value);
                case TypeCode.Byte:
                    return XmlConvert.ToString((byte)value);
                case TypeCode.Char:
                    return XmlConvert.ToString((char)value);
                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Unspecified);
                case TypeCode.Decimal:
                    return XmlConvert.ToString((decimal)value);
                case TypeCode.Double:
                    return XmlConvert.ToString((double)value);
                case TypeCode.Int16:
                    return XmlConvert.ToString((short)value);
                case TypeCode.Int32:
                    return XmlConvert.ToString((int)value);
                case TypeCode.Int64:
                    return XmlConvert.ToString((long)value);
                case TypeCode.Object:
                    return value.ToString();
                case TypeCode.SByte:
                    return XmlConvert.ToString((sbyte)value);
                case TypeCode.Single:
                    return XmlConvert.ToString((float)value);
                case TypeCode.String:
                    return value as string;
                case TypeCode.UInt16:
                    return XmlConvert.ToString((ushort)value);
                case TypeCode.UInt32:
                    return XmlConvert.ToString((uint)value);
                case TypeCode.UInt64:
                    return XmlConvert.ToString((ulong)value);
                default:
                    throw new NotSupportedException(PropertyTypeCode.ToString());
            }
        }
    }
}
