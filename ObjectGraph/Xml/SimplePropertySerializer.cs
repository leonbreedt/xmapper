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
    /// <summary>
    /// The simple property serializer is responsible for serializing a simple property that
    /// is not a user-defined type or collection. It is serialized as an XML element attribute.
    /// </summary>
    /// <typeparam name="TDeclaringType">The type that declares the property.</typeparam>
    /// <typeparam name="TPropertyType">The type of the property itself.</typeparam>
    internal class SimplePropertySerializer<TDeclaringType, TPropertyType> : PropertySerializer, IPropertySerializer<TDeclaringType>
        where TDeclaringType : new()
    {
        public SimplePropertySerializer(XName name,
                                        Func<TDeclaringType, TPropertyType> getter,
                                        Action<TDeclaringType, TPropertyType> setter,
                                        Func<string, TPropertyType> fromXmlValue,
                                        Func<TPropertyType, string> toXmlValue)
            : base(name)
        {
            Getter = getter;
            Setter = setter;
            FromXmlValue = fromXmlValue;
            ToXmlValue = toXmlValue;
        }

        public Func<TDeclaringType, TPropertyType> Getter { get; private set; }
        public Action<TDeclaringType, TPropertyType> Setter { get; private set; }
        public Func<string, TPropertyType> FromXmlValue { get; private set; }
        public Func<TPropertyType, string> ToXmlValue { get; private set; }

        public PropertySerializerType Type { get { return PropertySerializerType.Simple; } }

        public void ReadProperty(XmlReader reader, TDeclaringType obj)
        {
            Setter(obj, FromXmlValue(reader.Value));
        }

        public void WriteProperty(XmlWriter writer, TDeclaringType obj)
        {
            writer.WriteAttributeString(Name.LocalName, Name.NamespaceName, ToXmlValue(Getter(obj)));
        }
    }
}
