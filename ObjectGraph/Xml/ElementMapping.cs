//
// Copyright (C) 2010-2011 Leon Breedt
// ljb -at- bitserf [dot] org
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
using System.Xml.Linq;
using ObjectGraph.Util;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents a mapping of an XML element to a CLR type.
    /// </summary>
    /// <typeparam name="TTarget">The CLR type that this mapping will be associated with.</typeparam>
    public class ElementMapping<TTarget> : MappingBase, IElementMapping<TTarget>
    {
        #region Fields
        readonly Func<TTarget> _constructor;
        IAttributeMapping[] _attributes;
        IChildElementMapping[] _childElements;
        IDictionary<string, IDictionary<string, IAttributeMapping>> _attributesByNamespaceAndName;
        IDictionary<string, IDictionary<string, IChildElementMapping>> _childElementsByNamespaceAndName;
        #endregion

        public ElementMapping(XName name)
            : this(name, true, typeof(TTarget))
        {
        }

        public ElementMapping(XName name, bool cacheConstructor, Type type)
            : base(type, name)
        {
            if (cacheConstructor)
                _constructor = ReflectionHelper.GetTypedConstructorDelegate<TTarget>();
            _attributes = NoAttributes;
            _childElements = NoChildElements;
        }

        public virtual TTarget CreateInstance()
        {
            if (_constructor == null)
                throw new InvalidOperationException(string.Format("No constructor requested for type {0}", typeof(TTarget)));

            return _constructor();
        }

        public override IAttributeMapping[] Attributes
        {
            get { return _attributes; }
            internal set
            {
                _attributes = value;
                _attributesByNamespaceAndName = BuildMappingLookupTableByNamespaceAndName(_attributes);
            }
        }

        public override IChildElementMapping[] ChildElements
        {
            get { return _childElements; }
            internal set
            {
                _childElements = value;
                _childElementsByNamespaceAndName = BuildMappingLookupTableByNamespaceAndName(_childElements);
            }
        }

        public object CreateInstanceUntyped()
        {
            return CreateInstance();
        }

        public IAttributeMapping TryFindAttributeMapping(string localName)
        {
            return TryFindAttributeMapping("", localName);
        }

        public IAttributeMapping TryFindAttributeMapping(string namespaceUri, string localName)
        {
            if (_attributesByNamespaceAndName == null)
                return null;

            IDictionary<string, IAttributeMapping> propertiesByName;
            if (!_attributesByNamespaceAndName.TryGetValue(namespaceUri, out propertiesByName))
                return null;

            IAttributeMapping attributeMapping;
            if (!propertiesByName.TryGetValue(localName, out attributeMapping))
                return null;

            return attributeMapping;
        }

        public IChildElementMapping TryFindChildElementMapping(string localName)
        {
            return TryFindChildElementMapping("", localName);
        }

        public IChildElementMapping TryFindChildElementMapping(string namespaceUri, string localName)
        {
            if (_childElementsByNamespaceAndName == null)
                return null;

            IDictionary<string, IChildElementMapping> childrenByName;
            if (!_childElementsByNamespaceAndName.TryGetValue(namespaceUri, out childrenByName))
                return null;

            IChildElementMapping childElementMapping;
            if (!childrenByName.TryGetValue(localName, out childElementMapping))
                return null;

            return childElementMapping;
        }

        Dictionary<string, IDictionary<string, T>> BuildMappingLookupTableByNamespaceAndName<T>(IEnumerable<T> mappings)
            where T : IMapping
        {
            var mappingsByNamespace = from mapping in mappings
                                      let ns = mapping.NamespaceUri ?? ""
                                      group mapping by ns into g
                                      select new {Namespace = g.Key, Items = g};

            var mappingsByNamespaceAndName = new Dictionary<string, IDictionary<string, T>>();

            foreach (var mappingGrouping in mappingsByNamespace)
            {
                IDictionary<string, T> mappingsByName;
                if (!mappingsByNamespaceAndName.TryGetValue(mappingGrouping.Namespace, out mappingsByName))
                    mappingsByNamespaceAndName[mappingGrouping.Namespace] = mappingsByName = new Dictionary<string, T>();

                foreach (var groupedDescriptor in mappingGrouping.Items)
                {
                    if (mappingsByName.ContainsKey(groupedDescriptor.LocalName))
                        throw new ArgumentException(string.Format("'{0}' contains multiple mappings with name '{1}'", LocalName, groupedDescriptor.LocalName));
                    mappingsByName[groupedDescriptor.LocalName] = groupedDescriptor;
                }
            }

            return mappingsByNamespaceAndName;
        }

    }
}
