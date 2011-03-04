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
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Base class for XML mappings.
    /// </summary>
    public abstract class MappingBase : IMapping
    {
        #region Fields
        readonly Type _type;
        readonly string _namespaceUri;
        readonly string _localName;
        #endregion

        /// <summary>
        /// Creates a new XML mapping.
        /// </summary>
        /// <param name="type">The CLR type this mapping is associated with.</param>
        /// <param name="name">The XML name of the XML construct being mapped from.</param>
        protected MappingBase(Type type, XName name)
        {
            _type = type;
            _namespaceUri = name.NamespaceName;
            _localName = name.LocalName;
        }

        public Type Type { get { return _type; } }
        public string NamespaceUri { get { return _namespaceUri; } }
        public string LocalName { get { return _localName; } }
        public abstract IMapping[] Children { get; internal set; }
    }
}
