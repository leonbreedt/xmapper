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
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectGraph
{
    /// <summary>
    /// Utility class for binding to type members using expressions to avoid
    /// slow reflection lookups and hard-coding member name strings.
    /// </summary>
    public static class MemberBinding
    {
        public static PropertyInfo Property<TClass, TMember>(Expression<Func<TClass, TMember>> expression)
        {
            return (PropertyInfo)((MemberExpression)expression.Body).Member;
        }

        public static FieldInfo Field<TClass, TMember>(Expression<Func<TClass, TMember>> expression)
        {
            return (FieldInfo)((MemberExpression)expression.Body).Member;
        }
    }
}
