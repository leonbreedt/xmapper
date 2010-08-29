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
        readonly List<PropertySerializer> _propertySerializers;
        readonly Dictionary<string, PropertySerializer> _propertySerializersByName;
        #endregion

        public TypeSerializer(XName name)
            : base(name)
        {
            _propertySerializers = new List<PropertySerializer>();
            _propertySerializersByName = new Dictionary<string, PropertySerializer>();

            BuildPropertySerializers();
        }

        public IEnumerable<PropertySerializer> Properties
        {
            get { return _propertySerializers; }
        }

        public virtual T ReadObject(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteObject(XmlWriter writer, T obj)
        {
            throw new NotImplementedException();
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

                _propertySerializers.Add(serializer);
                _propertySerializersByName.Add(serializer.Name.LocalName, serializer);
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
