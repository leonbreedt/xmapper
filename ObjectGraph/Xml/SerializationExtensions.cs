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
using System.Reflection;
using System.Xml;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    internal static class SerializationExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo info)
            where TAttribute : Attribute
        {
            var attrs = info.GetCustomAttributes(typeof(TAttribute), false);
            if (attrs.Length == 0)
                return null;
            return attrs[0] as TAttribute;
        }

        internal static Tuple<int, int> GetXmlLineInfo(this XmlReader reader)
        {
            var info = reader as IXmlLineInfo;
            if (info != null)
                return Tuple.Create(info.LineNumber, info.LinePosition);
            return Tuple.Create(0, 0);
        }

        public static void EnsurePositionedOnNodeOfType(this XmlReader reader, XmlNodeType type)
        {
            if (reader.NodeType != type)
            {
                var line = reader.GetXmlLineInfo();
                throw new FormatException(
                    "({0},{1}): Expected XML element of type {2}, but was {3}".FormatWith(line.Item1,
                                                                                          line.Item2,
                                                                                          type,
                                                                                          reader.NodeType));
            }
        }
    }
}
