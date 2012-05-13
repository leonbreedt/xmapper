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
using System.Xml.Linq;

namespace XMapper
{
    /// <summary>
    /// Base class for XML mappings.
    /// </summary>
    public abstract class MappingBase : IMapping
    {
        /// <summary>
        /// Pre-cached empty list of attributes.
        /// </summary>
        protected static readonly IAttributeMapping[] NoAttributes = new IAttributeMapping[0];

        /// <summary>
        /// Pre-cached empty list of child elements.
        /// </summary>
        protected static readonly IChildElementMapping[] NoChildElements = new IChildElementMapping[0];

        /// <summary>
        /// Pre-cached empty list of child text elements.
        /// </summary>
        protected static readonly ITextContentMapping[] NoChildTextElements = new ITextContentMapping[0];

        #region Fields
        readonly Type _type;
        readonly string _namespaceUri;
        readonly string _localName;
        #endregion

        /// <summary>
        /// Creates a new XML mapping.
        /// </summary>
        /// <param name="type">The type this mapping is associated with.</param>
        /// <param name="name">The XML name of the XML construct being mapped from.</param>
        protected MappingBase(Type type, XName name)
        {
            _type = type;
            if (name != null)
            {
                _namespaceUri = name.NamespaceName;
                _localName = name.LocalName;
            }
        }

        public Type Type { get { return _type; } }
        public string NamespaceUri { get { return _namespaceUri; } }
        public string LocalName { get { return _localName; } }
        public abstract IAttributeMapping[] Attributes { get; internal set; }
        public abstract IChildElementMapping[] ChildElements { get; internal set; }
        public abstract ITextContentMapping TextContent { get; internal set; }
        public abstract ITextContentMapping[] ChildTextElements { get; internal set; }
    }
}
