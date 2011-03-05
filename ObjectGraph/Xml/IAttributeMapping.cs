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

namespace ObjectGraph.Xml
{
    /// <summary>
    /// Base interface for attribute mappings.
    /// </summary>

    public interface IAttributeMapping : IMapping
    {
        /// <summary>
        /// Reads a representation of the property value that can be safely written as an XML attribute.
        /// </summary>
        /// <param name="target">The target object to read the property value from.</param>
        /// <returns>Returns an XML-safe representation of the value.</returns>
        string GetValueInXmlForm(object target);

        /// <summary>
        /// Converts the XML attribute value into the CLR type of the property, and then sets the property value to
        /// this converted value.
        /// </summary>
        /// <param name="target">The target object to set the property value in.</param>
        /// <param name="value">The XML representation of the property.</param>
        void SetValueFromXmlForm(object target, string value);
    }

    /// <summary>
    /// Represents a mapping of an XML attribute to an object property.
    /// </summary>
    /// <typeparam name="TContainer">The CLR type that contains the property this mapping is associated with.</typeparam>
    /// <typeparam name="TProperty">The type of the property in the CLR type.</typeparam>
    public interface IAttributeMapping<TContainer, TProperty> : IAttributeMapping
    {
        /// <summary>
        /// Reads the value of this attribute from the property it is associated with on the target object.
        /// </summary>
        /// <param name="target">The target object to read the property from.</param>
        TProperty GetValue(TContainer target);

        /// <summary>
        /// Reads a representation of the property value that can be safely written as an XML attribute.
        /// </summary>
        /// <param name="target">The target object to read the property value from.</param>
        /// <returns>Returns an XML-safe representation of the value.</returns>
        string GetValueInXmlForm(TContainer target);

        /// <summary>
        /// Converts the XML attribute value into the CLR type of the property, and then sets the property value to
        /// this converted value.
        /// </summary>
        /// <param name="target">The target object to set the property value in.</param>
        /// <param name="value">The XML representation of the property.</param>
        void SetValueFromXmlForm(TContainer target, string value);
    }
}
