﻿//
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
using ObjectGraph.Util;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents a mapping of an XML element to a CLR type.
    /// </summary>
    /// <typeparam name="TTarget">The CLR type that this mapping will be associated with.</typeparam>
    public class ElementMapping<TTarget> : MappingBase, IElementMapping<TTarget>
    {
        /// <summary>
        /// Pre-cached empty list of children.
        /// </summary>
        static readonly IMapping[] NoChildren = new IMapping[0];

        #region Fields
        readonly Func<TTarget> _constructor;
        IMapping[] _children;
        #endregion

        public ElementMapping(XName name)
            : this(name, true)
        {
        }

        public ElementMapping(XName name, bool cacheConstructor)
            : base(typeof(TTarget), name)
        {
            if (cacheConstructor)
                _constructor = ReflectionHelper.GetTypedConstructorDelegate<TTarget>();
            _children = NoChildren;
        }

        public virtual TTarget CreateInstance()
        {
            return _constructor();
        }

        public override IMapping[] Children
        {
            get { return _children; } 
            internal set { _children = value; }
        }
    }
}