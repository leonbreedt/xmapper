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

namespace XMapper.Xml
{
    /// <summary>
    /// Base interface for child element mappings.
    /// </summary>
    public interface IChildElementMapping : IElementMapping
    {
        /// <summary>
        /// Reads the object from the property on the target that contains this object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Returns the CLR object.</returns>
        object GetFromContainer(object target);

        /// <summary>
        /// Sets the property on the target to the specified CLR object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="item">The CLR object.</param>
        void SetOnContainer(object target, object item);
    }
}
