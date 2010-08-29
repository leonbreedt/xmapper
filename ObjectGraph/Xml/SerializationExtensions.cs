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

        public static string PrefixXmlLineInfo(this string s, XmlReader reader)
        {
            var info = reader as IXmlLineInfo;
            if (info != null)
                return "({0},{1}): {2}".FormatWith(info.LineNumber, info.LinePosition, s);
            return s;
        }
    }
}
