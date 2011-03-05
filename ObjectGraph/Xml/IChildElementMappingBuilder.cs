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
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Fluent interface for building an element mapping for an element contained within another element.
    /// Adds an additional method for returning to the "parent" builder scope.
    /// </summary>
    public interface IChildElementMappingBuilder<TTarget, TParentBuilder> : IElementMappingBuilder<TTarget>
    {
        new IChildElementMappingBuilder<TTarget, TParentBuilder> Attribute<TProperty>(XName name,
                                                                                      Expression <Func<TTarget, TProperty>> property);

        new IChildElementMappingBuilder<TTarget, TParentBuilder> Attribute<TProperty>(XName name,
                                                                                      Expression<Func<TTarget, TProperty>> property,
                                                                                      Func<string, TProperty> customDeserializer,
                                                                                      Func<TProperty, string> customSerializer);

        TParentBuilder EndChild();
    }
}
