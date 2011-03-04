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
    internal class ContainerElementMappingBuilder<TContainingTarget, TMemberTarget, TParentBuilder> : IContainerElementMappingBuilder<TMemberTarget, TParentBuilder>
    {
        #region Fields
        readonly TParentBuilder _parentBuilderScope;
        readonly XName _name;
        readonly Expression<Func<TContainingTarget, ItemCollection<TMemberTarget>>> _propertyInParent;
        readonly List<Func<IMapping>> _memberElements;
        #endregion

        public ContainerElementMappingBuilder(TParentBuilder parentBuilderScope, XName name, Expression<Func<TContainingTarget, ItemCollection<TMemberTarget>>> propertyInParent)
        {
            _parentBuilderScope = parentBuilderScope;
            _name = name;
            _propertyInParent = propertyInParent;
            _memberElements = new List<Func<IMapping>>();
        }

        public IMapping Build()
        {
            return new ContainerElementMapping<TContainingTarget, TMemberTarget>(_name, _propertyInParent)
                   {
                       Children = _memberElements.Select(f => f()).ToArray()
                   };
        }

        public IChildElementMappingBuilder<TMemberTarget, IContainerElementMappingBuilder<TMemberTarget, TParentBuilder>> MemberElement(XName name)
        {
            var builder = new MemberElementMappingBuilder<TMemberTarget, IContainerElementMappingBuilder<TMemberTarget, TParentBuilder>>(this, name);
            _memberElements.Add(builder.Build);
            return builder;
        }

        public IChildElementMappingBuilder<TCustomMemberTarget, IContainerElementMappingBuilder<TMemberTarget, TParentBuilder>> MemberElement<TCustomMemberTarget>(XName name)
        {
            var builder = new MemberElementMappingBuilder<TCustomMemberTarget, IContainerElementMappingBuilder<TMemberTarget, TParentBuilder>>(this, name);
            _memberElements.Add(builder.Build);
            return builder;
        }

        public TParentBuilder EndContainer()
        {
            return _parentBuilderScope;
        }
    }
}