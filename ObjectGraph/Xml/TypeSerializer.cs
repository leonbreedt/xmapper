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
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// A type serializer is responsible for serializing and deserializing a
    /// contract type. It always corresponds to an XML element. It
    /// can contain zero or more properties.
    /// </summary>
    internal class TypeSerializer<T> : TypeSerializer
        where T : new()
    {
        #region Fields
        readonly List<IPropertySerializer<T>> _simplePropertySerializers;
        readonly List<IPropertySerializer<T>> _complexPropertySerializers;
        readonly Dictionary<string, IPropertySerializer<T>> _propertySerializersByName;
        #endregion

        public TypeSerializer(XName name)
            : base(name)
        {
            _simplePropertySerializers = new List<IPropertySerializer<T>>();
            _complexPropertySerializers = new List<IPropertySerializer<T>>();
            _propertySerializersByName = new Dictionary<string, IPropertySerializer<T>>();

            BuildPropertySerializers();
        }

        public IEnumerable<IPropertySerializer<T>> SimplePropertySerializers
        {
            get { return _simplePropertySerializers; }
        }

        public IEnumerable<IPropertySerializer<T>> ComplexPropertySerializers
        {
            get { return _complexPropertySerializers; }
        }

        public virtual T ReadObject(XmlReader reader)
        {
            var obj = new T();

            string elementName = Name.LocalName;
            string namespaceName = Name.NamespaceName;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (string.Compare(reader.Name, elementName, false) != 0 || string.Compare(reader.NamespaceURI, namespaceName, false) != 0)
                        throw new FormatException("Expected element '{0}' with namespace '{1}'".FormatWith(elementName, namespaceName).PrefixXmlLineInfo(reader));

                    if (reader.HasAttributes)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);

                            IPropertySerializer<T> serializer;
                            if (_propertySerializersByName.TryGetValue(reader.Name, out serializer))
                                serializer.ReadProperty(reader, obj);
                        }
                    }

                    break;
                }
                
                continue;
            }

            return obj;
        }

        public virtual void WriteObject(XmlWriter writer, T obj)
        {
            writer.WriteStartElement(Name.LocalName, Name.NamespaceName);

            foreach (var serializer in _simplePropertySerializers)
                serializer.WriteProperty(writer, obj);

            foreach (var serializer in _complexPropertySerializers)
                serializer.WriteProperty(writer, obj);

            writer.WriteEndElement();
        }

        void BuildPropertySerializers()
        {
            var serializablePropertyInfo = from info in typeof(T).GetProperties()
                                           let attr = info.GetAttribute<DataMemberAttribute>()
                                           where attr != null
                                           select info;

            foreach (var info in serializablePropertyInfo)
            {
                var serializer = PropertySerializer.Build<T>(info.PropertyType, info);

                string xmlName = serializer.Name.LocalName;
                if (_propertySerializersByName.ContainsKey(xmlName))
                    throw new ArgumentException("Property '{0}' has name '{1}' already used by another property.".FormatWith(info.Name, xmlName));
                _propertySerializersByName.Add(xmlName, serializer);

                if (serializer.Type == PropertySerializerType.Simple)
                    _simplePropertySerializers.Add(serializer);
                else
                    _complexPropertySerializers.Add(serializer);
            }
        }
    }

    internal class TypeSerializer
    {
        #region Fields
        static Dictionary<Type, object> _serializersByType;
        readonly XName _name;
        #endregion

        static TypeSerializer()
        {
            _serializersByType = new Dictionary<Type, object>();
        }

        protected TypeSerializer(XName name)
        {
            _name = name;
        }

        public XName Name { get { return _name; } }

        public static TypeSerializer<T> Build<T>()
            where T : new()
        {
            var type = typeof(T);

            lock (_serializersByType)
            {
                object serializer;

                if (_serializersByType.TryGetValue(type, out serializer))
                    return (TypeSerializer<T>)serializer;

                var attr = type.GetAttribute<DataContractAttribute>();
                if (attr == null)
                    throw new NotSupportedException("Type {0} must have a [DataContract] attribute to be serializable".FormatWith(type.Name));

                XName name = attr.Name ?? type.Name;

                TypeSerializer<T> ret;

                _serializersByType[type] = ret = new TypeSerializer<T>(name);

                return ret;
            }
        }
    }
}
