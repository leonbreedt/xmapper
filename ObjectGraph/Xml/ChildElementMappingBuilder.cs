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
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    internal class ChildElementMappingBuilder<TContainingTarget, TTarget, TParentBuilder> : ElementMappingBuilder<TTarget>, IChildElementMappingBuilder<TTarget, TParentBuilder>
    {
        #region Fields
        readonly TParentBuilder _parentBuilderScope;
        readonly Expression<Func<TContainingTarget, TTarget>> _propertyInParent;
        #endregion

        public ChildElementMappingBuilder(TParentBuilder parentBuilderScope, XName name, Expression<Func<TContainingTarget, TTarget>> propertyInParent)
            : base(name)
        {
            _parentBuilderScope = parentBuilderScope;
            _propertyInParent = propertyInParent;
        }

        public override IMapping Build()
        {
            return new ChildElementMapping<TContainingTarget, TTarget>(Name, _propertyInParent)
                   {
                       Children = Attributes.Union(Elements.Select(f => f())).ToArray()
                   };
        }

        public new IChildElementMappingBuilder<TTarget, TParentBuilder> Attribute<TProperty>(XName name, Expression<Func<TTarget, TProperty>> property)
        {
            base.Attribute(name, property);
            return this;
        }

        public new IChildElementMappingBuilder<TTarget, TParentBuilder> Attribute<TProperty>(XName name, Expression<Func<TTarget, TProperty>> property, Func<string, TProperty> customDeserializer, Func<TProperty, string> customSerializer)
        {
            base.Attribute(name, property, customDeserializer, customSerializer);
            return this;
        }

        public TParentBuilder EndChild()
        {
            return _parentBuilderScope;
        }
    }
}