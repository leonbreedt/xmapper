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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectGraph.Test
{
    [TestClass]
    public class MemberEqualityComparerTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_IndexedProperty_ThrowsException()
        {
            new MemberEqualityComparer<WithIndexer>(typeof(WithIndexer).GetProperties());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_NonPropertyOrFieldMember_ThrowsException()
        {
            new MemberEqualityComparer<WithIndexer>(typeof(object).GetMembers());
        }

        [TestMethod]
        public void ValidMembersSpecified_ComparesOnlyThoseMembers()
        {
            var comparer = new MemberEqualityComparer<WithMember>(MemberBinding.Field<WithMember, string>(x => x.Name),
                                                                  MemberBinding.Property<WithMember, int>(x => x.Age));

            var obj1 = new WithMember {Name = "John", Age = 15, IgnoredValue = 30};
            var obj2 = new WithMember {Name = "John", Age = 15, IgnoredValue = 40};

            Assert.IsTrue(comparer.Equals(obj1, obj2));
            Assert.AreEqual(comparer.GetHashCode(obj1), comparer.GetHashCode(obj2));
        }

        class WithMember : Item<WithMember>
        {
            public string Name;
            public int Age { get; set; }
            public int IgnoredValue;
        }

        class WithIndexer
        {
            public object this[int index]
            {
                get { return null; } set { }
            }
        }
    }
}
