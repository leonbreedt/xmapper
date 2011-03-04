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

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Represents a mapping from an XML construct to a model element such as a class or class property.
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// Gets the CLR type associated with this mapping.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the XML namespace URI associated with the XML construct being mapped from.
        /// </summary>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the XML local name associated with the XML construct being mapped from.
        /// </summary>
        string LocalName { get; }

        /// <summary>
        /// Gets the child mappings contained within this mapping, if any.
        /// </summary>
        IMapping[] Children { get; }
    }
}