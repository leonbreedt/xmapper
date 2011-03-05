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

namespace ObjectGraph.Xml
{
    internal class ElementMappingBuilder<TTarget> : IElementMappingBuilder<TTarget>
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

        public IElementMappingBuilder<TTarget> Attribute<TProperty>(XName name, Expression<Func<TTarget, TProperty>> property)
        {
            _attrs.Add(new AttributeMapping<TTarget, TProperty>(name, property));
            return this;
        }

        public IElementMappingBuilder<TTarget> Attribute<TProperty>(XName name, Expression<Func<TTarget, TProperty>> property, Func<string, TProperty> customDeserializer, Func<TProperty, string> customSerializer)
        {
            _attrs.Add(new AttributeMapping<TTarget, TProperty>(name, property, customDeserializer, customSerializer));
            return this;
        }

        public IContainerElementMappingBuilder<TMemberTarget, IElementMappingBuilder<TTarget>> ContainerElement<TMemberTarget>(XName name, Expression<Func<TTarget, IList<TMemberTarget>>> propertyInTarget)
        {
            var builder = new ContainerElementMappingBuilder<TTarget, TMemberTarget, IElementMappingBuilder<TTarget>>(this, name, propertyInTarget);
            _elements.Add(builder.Build);
            return builder;
        }

        public IChildElementMappingBuilder<TChildTarget, IElementMappingBuilder<TTarget>> Element<TChildTarget>(XName name, Expression<Func<TTarget, TChildTarget>> propertyInParent)
        {
            var builder = new ChildElementMappingBuilder<TTarget, TChildTarget, IElementMappingBuilder<TTarget>>(this, name, propertyInParent);
            _elements.Add(builder.Build);
            return builder;
        }

        public virtual IElementMapping Build()
        {
            return new ElementMapping<TTarget>(_name)
                   {
                       Attributes = Attributes.ToArray(),
                       ChildElements = Elements.Select(f => f()).ToArray()
                   };
        }

        protected XName Name { get { return _name; } }
        protected List<IAttributeMapping> Attributes { get { return _attrs; } }
        protected List<Func<IChildElementMapping>> Elements { get { return _elements; } }
    }
}