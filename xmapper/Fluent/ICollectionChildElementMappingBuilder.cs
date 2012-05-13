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
using System.Xml.Linq;

namespace XMapper.Fluent
{

    /// <summary>
    /// Base interface for collection child element mappings.
    /// </summary>
    public interface ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder>
    {
        ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder> Attribute<TProperty>(XName name,
                                                                                                        Expression<Func<TElement, TProperty>> property);

        ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder> Attribute<TProperty>(XName name,
                                                                                                        Expression<Func<TElement, TProperty>> property,
                                                                                                        Func<string, TProperty> customDeserializer,
                                                                                                        Func<TProperty, string> customSerializer);

        ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder> TextContent<TProperty>(Expression<Func<TElement, TProperty>> property);
        ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder> TextElement<TChildElement>(XName name, Expression<Func<TElement, TChildElement>> property);

        IChildElementMappingBuilder<TChildElement, ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder>> Element<TChildElement>(XName name, Expression<Func<TElement, TChildElement>> propertyInParent);


        ICollectionChildElementMappingBuilder<TElement, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder>> CollectionElement<TChildElement>(XName name, Expression<Func<TElement, IList<TChildElement>>> propertyInParent);
        ICollectionChildElementMappingBuilder<TElement, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TElement, TParentBuilder>> CollectionElement<TChildElement>(XName name);

        TParentBuilder EndElement();

        ICollectionChildElementMapping Build();
    }
}
