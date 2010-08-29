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
    /// The complex property serializer is responsible for serializing a property that
    /// is a user-defined type. It is serialized as an XML element.
    /// </summary>
    /// <typeparam name="TDeclaringType">The type that declares the property.</typeparam>
    /// <typeparam name="TPropertyType">The type of the property itself.</typeparam>
    internal class ComplexPropertySerializer<TDeclaringType, TPropertyType> : PropertySerializer, IPropertySerializer<TDeclaringType>
        where TPropertyType : new()
    {
        #region Fields
        readonly TypeSerializer<TPropertyType> _serializer;
        #endregion

        public ComplexPropertySerializer(XName name,
                                         Func<TDeclaringType, TPropertyType> getter,
                                         Action<TDeclaringType, TPropertyType> setter,
                                         TypeSerializer<TPropertyType> serializer)
            : base(name)
        {
            Getter = getter;
            Setter = setter;
            _serializer = serializer;
        }

        public PropertySerializerType Type { get { return PropertySerializerType.Complex; } }
        public Func<TDeclaringType, TPropertyType> Getter { get; private set; }
        public Action<TDeclaringType, TPropertyType> Setter { get; private set; }

        public void ReadProperty(XmlReader reader, TDeclaringType target)
        {
            Setter(target, _serializer.ReadObject(reader));
        }

        public void WriteProperty(XmlWriter writer, TDeclaringType source)
        {
            _serializer.WriteObject(writer, Getter(source));
        }
    }
}
