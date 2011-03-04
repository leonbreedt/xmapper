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

using System.Linq;
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    internal class MemberElementMappingBuilder<TCustomMemberTarget, TParentBuilder> : ChildElementMappingBuilder<TCustomMemberTarget, TCustomMemberTarget, TParentBuilder>
    {
        public MemberElementMappingBuilder(TParentBuilder parentBuilderScope, XName name)
            : base(parentBuilderScope, name, null)
        {
        }

        public override IMapping Build()
        {
            return new ElementMapping<TCustomMemberTarget>(Name)
                   {
                       Children = Attributes.Union(Elements.Select(f => f())).ToArray()
                   };
        }
    }
}