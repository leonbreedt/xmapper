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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using ProtoBuf;

namespace ObjectGraph
{
    public abstract class Item<TItem> where TItem : class
    {
        public static TItem Load(Stream stream, SerializationFormat format)
        {
            if (format == SerializationFormat.ProtocolBuffer)
                return Serializer.DeserializeWithLengthPrefix<TItem>(stream, PrefixStyle.Fixed32);

            throw new NotImplementedException();
        }

        public void Save(Stream stream, SerializationFormat format)
        {
            if (format == SerializationFormat.ProtocolBuffer)
                Serializer.SerializeWithLengthPrefix(stream, this as TItem, PrefixStyle.Fixed32);
            else
                throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var other = (obj as TItem);
            if (other == null)
                return false;

            return DefaultEqualityComparer.Equals(this as TItem, other);
        }

        public override int GetHashCode()
        {
            return DefaultEqualityComparer.GetHashCode(this as TItem);
        }

        protected virtual IEnumerable<MemberInfo> GetEquatableMembers()
        {
            var type = typeof(TItem);

            var members = new List<MemberInfo>();

            members.AddRange(type.GetProperties().Where(p => p.GetCustomAttributes(typeof(DataMemberAttribute), false).Length == 1));
            members.AddRange(type.GetFields().Where(p => p.GetCustomAttributes(typeof(DataMemberAttribute), false).Length == 1));

            return members;
        }

        protected IEqualityComparer<TItem> DefaultEqualityComparer
        {
            get { return new MemberEqualityComparer<TItem>(GetEquatableMembers()); }
        }

        protected static MemberInfo Property<TProperty>(Expression<Func<TItem, TProperty>> expr)
        {
            return MemberBinding.Property(expr);
        }

        protected static MemberInfo Field<TProperty>(Expression<Func<TItem, TProperty>> expr)
        {
            return MemberBinding.Field(expr);
        }
    }
}
