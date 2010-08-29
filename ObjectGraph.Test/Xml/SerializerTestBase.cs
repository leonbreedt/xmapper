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
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using ObjectGraph.Xml;

namespace ObjectGraph.Test.Xml
{
    public abstract class SerializerTestBase
    {
        internal static XmlWriter BuildFragmentWriter(Stream stream)
        {
            return XmlWriter.Create(stream, new XmlWriterSettings {ConformanceLevel = ConformanceLevel.Fragment, Encoding = new UTF8Encoding(false)});
        }

        internal static XmlReader BuildFragmentReader(Stream stream)
        {
            return XmlReader.Create(stream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });
        }

        internal static PropertySerializer BuildPropertySerializerFor<TContainingType, TPropertyType>(Expression<Func<TContainingType, TPropertyType>> expr)
        {
            return PropertySerializer.Build<TContainingType, TPropertyType>((PropertyInfo)((MemberExpression)expr.Body).Member);
        }
    }
}
