//
// Copyright (C) 2010 Leon Breedt
// bitserf -at- gmail [dot] com
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
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XRequiredAttribute : Attribute
    {
        public XRequiredAttribute()
            : this(NodeType.Attribute)
        {
        }

        public XRequiredAttribute(NodeType type)
        {
            Type = type;
        }

        public XName Name { get; set; }
        public NodeType Type { get; set; }
    }
}
