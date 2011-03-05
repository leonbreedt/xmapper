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
using System.Linq.Expressions;
using System.Xml.Linq;

namespace XMapper.Fluent
{

    /// <summary>
    /// Base interface for collection child element mappings.
    /// </summary>
    public interface ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>
    {
        ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> Attribute<TAttributeProperty>(XName name,
                                                                                                                 Expression<Func<TMember, TAttributeProperty>> property);

        ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder> Attribute<TAttributeProperty>(XName name,
                                                                                                                 Expression<Func<TMember, TAttributeProperty>> property,
                                                                                                                 Func<string, TAttributeProperty> customDeserializer,
                                                                                                                 Func<TAttributeProperty, string> customSerializer);

        IChildElementMappingBuilder<TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> Element<TChildElement>(XName name, Expression<Func<TMember, TChildElement>> propertyInParent);

        ICollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> CollectionElement<TChildElement>(XName name, Expression<Func<TMember, IList<TChildElement>>> propertyInParent);
        ICollectionChildElementMappingBuilder<TMember, TChildElement, ICollectionChildElementMappingBuilder<TContainer, TMember, TParentBuilder>> CollectionElement<TChildElement>(XName name);

        TParentBuilder EndElement();

        ICollectionChildElementMapping Build();
    }
}
