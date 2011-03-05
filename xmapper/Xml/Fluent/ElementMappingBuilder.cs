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
using System.Linq.Expressions;
using System.Xml.Linq;

namespace XMapper.Xml.Fluent
{
    internal class ElementMappingBuilder<TElement> : IElementMappingBuilder<TElement>
    {
        #region Fields
        readonly XName _name;
        readonly List<IAttributeMapping> _attrs;
        readonly List<Func<IChildElementMapping>> _elements;
        #endregion

        public ElementMappingBuilder(XName name)
        {
            _name = name;
            _attrs = new List<IAttributeMapping>();
            _elements = new List<Func<IChildElementMapping>>();
        }

        public IElementMappingBuilder<TElement> Attribute<TProperty>(XName name, Expression<Func<TElement, TProperty>> property)
        {
            _attrs.Add(new AttributeMapping<TElement, TProperty>(name, property));
            return this;
        }

        public IElementMappingBuilder<TElement> Attribute<TProperty>(XName name, Expression<Func<TElement, TProperty>> property, Func<string, TProperty> customDeserializer, Func<TProperty, string> customSerializer)
        {
            _attrs.Add(new AttributeMapping<TElement, TProperty>(name, property, customDeserializer, customSerializer));
            return this;
        }

        public IChildElementMappingBuilder<TChildTarget, IElementMappingBuilder<TElement>> Element<TChildTarget>(XName name, Expression<Func<TElement, TChildTarget>> propertyInParent)
        {
            var builder = new ChildElementMappingBuilder<TElement, TChildTarget, IElementMappingBuilder<TElement>>(this, name, propertyInParent);
            _elements.Add(builder.Build);
            return builder;
        }

        public ICollectionChildElementMappingBuilder<TElement, TChildElement, IElementMappingBuilder<TElement>> CollectionElement<TChildElement>(XName name, Expression<Func<TElement, IList<TChildElement>>> propertyInParent)
        {
            var builder = new CollectionChildElementMappingBuilder<TElement, TChildElement, IElementMappingBuilder<TElement>>(this, name, propertyInParent);
            _elements.Add(builder.Build);
            return builder;
        }

        public virtual IElementMapping Build()
        {
            return new ElementMapping<TElement>(_name)
                   {
                       Attributes = _attrs.ToArray(),
                       ChildElements = _elements.Select(f => f()).ToArray()
                   };
        }
    }
}