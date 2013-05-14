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
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace XMapper.Fluent
{
    /// <summary>
    /// Fluent interface for building an element mapping.
    /// </summary>
    public interface IElementMappingBuilder<TElement>
    {
        IElementMappingBuilder<TElement> Attribute<TProperty>(XName name,
                                                             Expression<Func<TElement, TProperty>> property);

        IElementMappingBuilder<TElement> AnyAttribute(
            Expression<Func<TElement, IList<XAttribute>>> customAttributesProperty);
        
        IElementMappingBuilder<TElement> AnyAttribute<TAttribute>(
            Expression<Func<TElement, IList<TAttribute>>> attributeProperty,
            Func<XmlReader, TAttribute> customDeserializer,
            Action<XmlWriter, TAttribute> customSerializer);

        IElementMappingBuilder<TElement> Attribute<TProperty>(XName name,
                                                             Expression<Func<TElement, TProperty>> property,
                                                             Func<string, TProperty> customDeserializer,
                                                             Func<TProperty, string> customSerializer);

        IElementMappingBuilder<TElement> TextContent<TProperty>(Expression<Func<TElement, TProperty>> property);
        IElementMappingBuilder<TElement> TextElement<TChildElement>(XName name, Expression<Func<TElement, TChildElement>> property);

        IChildElementMappingBuilder<TChildElement, IElementMappingBuilder<TElement>> Element<TChildElement>(XName name, Expression<Func<TElement, TChildElement>> propertyInParent);
        IElementMappingBuilder<TElement> AnyElement(Expression<Func<TElement, IList<XElement>>> customElementsProperty);
        IElementMappingBuilder<TElement> AnyElement<TCustomElement>(
            Expression<Func<TElement, IList<TCustomElement>>> customElementsProperty,
            Func<XmlReader, TCustomElement> customDeserializer,
            Action<XmlWriter, TCustomElement> customSerializer);

        ICollectionChildElementMappingBuilder<TElement, TChildElement, IElementMappingBuilder<TElement>> CollectionElement<TChildElement>(XName name, Expression<Func<TElement, IList<TChildElement>>> propertyInParent);
        
        IElementMapping Build();
    }
}
