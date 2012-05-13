//
// Copyright (C) 2010-2012 Leon Breedt
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

namespace XMapper.Fluent
{
    internal class CollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> : ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>
    {
        #region Fields
        readonly XName _name;
        readonly TParentBuilder _parentBuilderScope;
        readonly Expression<Func<TContainer, IList<TMember>>> _propertyInParent;
        readonly List<IAttributeMapping> _attrs;
        readonly List<Func<IChildElementMapping>> _elements;
        ITextContentMapping _textContent;
        readonly List<ITextContentMapping> _childTextElements;
        #endregion

        public CollectionChildElementMappingBuilder(TParentBuilder parentBuilderScope, XName name, Expression<Func<TContainer, IList<TMember>>> propertyInParent)
        {
            _parentBuilderScope = parentBuilderScope;
            _name = name;
            _propertyInParent = propertyInParent;
            _attrs = new List<IAttributeMapping>();
            _elements = new List<Func<IChildElementMapping>>();
            _childTextElements = new List<ITextContentMapping>();
        }

        public CollectionChildElementMappingBuilder(TParentBuilder parentBuilderScope, XName name)
            : this(parentBuilderScope, name, null)
        {
        }

        public ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> Attribute<TAttributeProperty>(XName name, Expression<Func<TMember, TAttributeProperty>> property)
        {
            _attrs.Add(new AttributeMapping<TMember, TAttributeProperty>(name, property));
            return this;
        }

        public ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> Attribute<TAttributeProperty>(XName name, Expression<Func<TMember, TAttributeProperty>> property, Func<string, TAttributeProperty> customDeserializer, Func<TAttributeProperty, string> customSerializer)
        {
            _attrs.Add(new AttributeMapping<TMember, TAttributeProperty>(name, property, customDeserializer, customSerializer));
            return this;
        }

        public ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> TextContent<TProperty>(Expression<Func<TMember, TProperty>> property)
        {
            if (_textContent != null)
                throw new ArgumentException("Only one TextContent() is allowed for a particular element.");
            _textContent = new TextContentMapping<TMember, TProperty>(property);
            return this;
        }

        public ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> TextElement<TChildElement>(XName name, Expression<Func<TMember, TChildElement>> property)
        {
            _childTextElements.Add(new TextContentMapping<TMember, TChildElement>(name, property));
            return this;
        }

        public IChildElementMappingBuilder<TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> Element<TChildElement>(XName name, Expression<Func<TMember, TChildElement>> propertyInParent)
        {
            var builder = new ChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>>(this, name, propertyInParent);
            _elements.Add(builder.Build);
            return builder;
        }

        public ICollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> CollectionElement<TChildElement>(XName name, Expression<Func<TMember, IList<TChildElement>>> propertyInParent)
        {
            var builder = new CollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>>(this, name, propertyInParent);
            _elements.Add(builder.Build);
            return builder;
        }

        public ICollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> CollectionElement<TChildElement>(XName name)
        {
            var builder = new CollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>>(this, name);
            _elements.Add(builder.Build);
            return builder;
        }

        public TParentBuilder EndElement()
        {
            return _parentBuilderScope;
        }

        public ICollectionChildElementMapping Build()
        {
            return new CollectionChildElementMapping<TContainer, TMember>(_name, _propertyInParent)
                       {
                           Attributes = _attrs.ToArray(),
                           ChildElements = _elements.Select(f => f()).ToArray(),
                           TextContent = _textContent,
                           ChildTextElements = _childTextElements.ToArray(),
                       };
        }
    }
}