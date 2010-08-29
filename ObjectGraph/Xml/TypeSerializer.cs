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
using System.Linq.Expressions;
using System.Reflection;
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
        readonly Func<T> _createInstance;
        #endregion

        public TypeSerializer(XName name)
            : base(name)
        {
            _simplePropertySerializers = new List<IPropertySerializer<T>>();
            _complexPropertySerializers = new List<IPropertySerializer<T>>();
            _propertySerializersByName = new Dictionary<string, IPropertySerializer<T>>();
            _createInstance = GetConstructor<T>();

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
            var obj = _createInstance();

            bool seenObjectStart = false;

            do
            {
                var nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    if (!seenObjectStart)
                    {
                        if (string.Compare(reader.Name, Name.LocalName, false) != 0 && string.Compare(reader.NamespaceURI, Name.NamespaceName, false) != 0)
                            throw new FormatException("Expected element '{0}' with namespace '{1}'".FormatWith(Name.LocalName,Name.NamespaceName).PrefixXmlLineInfo(reader));

                        if (reader.MoveToFirstAttribute())
                        {
                            do
                            {
                                IPropertySerializer<T> serializer;
                                if (_propertySerializersByName.TryGetValue(reader.Name, out serializer) &&
                                    serializer.Type == PropertySerializerType.Simple)
                                    serializer.ReadProperty(reader, obj);
                            }
                            while (reader.MoveToNextAttribute());
                            reader.MoveToElement();
                        }

                        seenObjectStart = true;
                    }
                    else
                    {
                        IPropertySerializer<T> serializer;
                        if (_propertySerializersByName.TryGetValue(reader.Name, out serializer) &&
                            serializer.Type == PropertySerializerType.Complex)
                        {
                            serializer.ReadProperty(reader, obj);
                        }
                        else
                        {
                            reader.Skip();
                            continue;
                        }
                    }

                    if (!reader.IsEmptyElement)
                        continue;
                    
                    break;
                }

                if (nodeType == XmlNodeType.EndElement)
                    break;

            } while (reader.Read());

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

        static Func<TType> GetConstructor<TType>() where TType : new()
        {
            var constructorInfo = typeof(TType).GetConstructor(new Type[0]);
            return Expression.Lambda<Func<TType>>(Expression.New(constructorInfo)).Compile();
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

        class ReadContext
        {
            public string ElementName { get; set; }
            public string NamespaceName { get; set; }
            public ReadContext ParentContext { get; set; }
            public Dictionary<string, IPropertySerializer<T>> SerializersByName { get; set; }
        }
    }

    internal class TypeSerializer
    {
        #region Fields
        static Dictionary<Tuple<Type,XName>, object> _serializersByTypeAndName;
        static MethodInfo _buildMethodT;
        readonly XName _name;
        #endregion

        static TypeSerializer()
        {
            _serializersByTypeAndName = new Dictionary<Tuple<Type, XName>, object>();
            _buildMethodT = typeof(TypeSerializer).GetMethod("Build",
                                                             BindingFlags.Public | BindingFlags.Static,
                                                             null,
                                                             new[] {typeof(XName)},
                                                             null);
        }

        protected TypeSerializer(XName name)
        {
            _name = name;
        }

        public XName Name { get { return _name; } }

        public static TypeSerializer Build(Type type, XName name)
        {
            return (TypeSerializer)_buildMethodT.MakeGenericMethod(type).Invoke(null, new object[] {name});
        }

        public static TypeSerializer<T> Build<T>()
            where T : new()
        {
            return Build<T>(null);
        }

        public static TypeSerializer<T> Build<T>(XName name)
            where T : new()
        {
            var type = typeof(T);

            lock (_serializersByTypeAndName)
            {
                object serializer;

                var attr = type.GetAttribute<DataContractAttribute>();
                if (attr == null)
                    throw new NotSupportedException("Type {0} must have a [DataContract] attribute to be serializable".FormatWith(type.Name));

                name = name ?? attr.Name ?? type.Name;

                var key = Tuple.Create(typeof(T), name);
                if (_serializersByTypeAndName.TryGetValue(key, out serializer))
                    return (TypeSerializer<T>)serializer;

                TypeSerializer<T> ret;

                _serializersByTypeAndName[key] = ret = new TypeSerializer<T>(name);

                return ret;
            }
        }
    }
}
