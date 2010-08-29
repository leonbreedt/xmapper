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
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class PropertySerializerTest : SerializerTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void NoDataMemberAttributeThrowsException()
        {
            BuildPropertySerializerFor<MissingDataMember, string>(x => x.FirstName);
        }

        [TestMethod]
        public void NameDefaultsToPropertyName()
        {
            var serializer = BuildPropertySerializerFor<WithDataMember, string>(x => x.FirstName);

            Assert.AreEqual("FirstName", serializer.Name);
        }

        [TestMethod]
        public void OverriddenNameIsUsed()
        {
            var serializer = BuildPropertySerializerFor<WithName, string>(x => x.FirstName);

            Assert.AreEqual("Name", serializer.Name);
        }

        [TestMethod]
        public void SerializerIsReusedForSameProperty()
        {
            var serializer1 = BuildPropertySerializerFor<WithProperties, string>(x => x.FirstName);
            var serializer2 = BuildPropertySerializerFor<WithProperties, string>(x => x.FirstName);
            var serializer3 = BuildPropertySerializerFor<WithProperties, string>(x => x.LastName);
            var serializer4 = BuildPropertySerializerFor<WithName, string>(x => x.FirstName);

            Assert.AreSame(serializer1, serializer2);
            Assert.AreNotSame(serializer1, serializer3);
            Assert.AreNotSame(serializer1, serializer4);
        }

        #region Helpers
        class MissingDataMember
        {
            public string FirstName { get; set; }
        }

        [DataContract]
        class WithDataMember
        {
            [DataMember]
            public string FirstName { get; set; }
        }

        [DataContract]
        class WithName
        {
            [DataMember(Name="Name")]
            public string FirstName { get; set; }
        }

        [DataContract]
        class WithProperties
        {
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }

            public int Age { get; set; }
            bool IsPrivate { get; set; }
        }
        #endregion
    }
}
