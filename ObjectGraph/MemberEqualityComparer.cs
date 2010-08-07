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
using System.Collections.Generic;
using System.Reflection;
using ObjectGraph.Extensions;

namespace ObjectGraph
{
    public class MemberEqualityComparer<TItem> : IEqualityComparer<TItem>
    {
        #region Fields
        readonly List<FieldInfo> _fields;
        readonly List<PropertyInfo> _properties;
        #endregion

        public MemberEqualityComparer(MemberInfo arg0)
            : this(new[] { arg0 })
        {
        }

        public MemberEqualityComparer(params MemberInfo[] members)
            : this((IEnumerable<MemberInfo>)members)
        {
        }

        public MemberEqualityComparer(IEnumerable<MemberInfo> members)
        {
            foreach (var member in members)
            {
                if (member is FieldInfo)
                {
                    if (_fields == null)
                        _fields = new List<FieldInfo>();
                    _fields.Add((FieldInfo)member);
                }
                else if (member is PropertyInfo)
                {
                    if (_properties == null)
                        _properties = new List<PropertyInfo>();

                    var propertyInfo = (PropertyInfo)member;
                    if (propertyInfo.GetIndexParameters().Length > 0)
                        throw new ArgumentException("Member '{0}' is an indexed property; indexed properties are not supported by MemberEqualityComparer".FormatWith(member.Name), "members");

                    _properties.Add(propertyInfo);
                }
                else
                    throw new ArgumentException("Member '{0}' is not a field or property; only fields and properties are supported by MemberEqualityComparer".FormatWith(member.Name), "members");
            }
        }

        public bool Equals(TItem x, TItem y)
        {
            var allMembersEqual = true;

            object xValue;
            object yValue;

            if (_fields != null)
            {
                foreach (var fieldInfo in _fields)
                {
                    xValue = fieldInfo.GetValue(x);
                    yValue = fieldInfo.GetValue(y);

                    if (xValue == null)
                        allMembersEqual &= (yValue == null);
                    else
                        allMembersEqual &= (yValue != null && xValue.Equals(yValue));
                    if (!allMembersEqual)
                        return false;
                }
            }

            if (_properties != null)
            {
                foreach (var propertyInfo in _properties)
                {
                    xValue = propertyInfo.GetValue(x, null);
                    yValue = propertyInfo.GetValue(y, null);

                    if (xValue == null)
                        allMembersEqual &= (yValue == null);
                    else
                        allMembersEqual &= (yValue != null && xValue.Equals(yValue));
                    if (!allMembersEqual)
                        return false;
                }
            }

            return true;
        }

        public int GetHashCode(TItem obj)
        {
            int hashCode = 0;

            object value;

            if (_fields != null)
            {
                foreach (var fieldInfo in _fields)
                {
                    value = fieldInfo.GetValue(obj);
                    hashCode ^= (value != null ? value.GetHashCode() : 0);
                }
            }

            if (_properties != null)
            {
                foreach (var propertyInfo in _properties)
                {
                    value = propertyInfo.GetValue(obj, null);
                    hashCode ^= (value != null ? value.GetHashCode() : 0);
                }
            }

            return hashCode;
        }
    }
}
