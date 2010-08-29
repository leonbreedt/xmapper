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
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectGraph.Extensions;
using ObjectGraph.Xml;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class TypeSerializerTest : SerializerTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void NoDataContractAttributeThrowsException()
        {
            TypeSerializer.Build<MissingDataContract>();
        }

        [TestMethod]
        public void NameDefaultsToTypeName()
        {
            var serializer = TypeSerializer.Build<WithDataContract>();

            Assert.AreEqual("WithDataContract", serializer.Name);
        }

        [TestMethod]
        public void OverriddenNameIsUsed()
        {
            var serializer = TypeSerializer.Build<WithName>();

            Assert.AreEqual("NewName", serializer.Name);
        }

        [TestMethod]
        public void SerializerIsReusedForSameType()
        {
            var serializer1 = TypeSerializer.Build<WithDataContract>();
            var serializer2 = TypeSerializer.Build<WithDataContract>();
            var serializer3 = TypeSerializer.Build<WithDataContract>();
            var serializer4 = TypeSerializer.Build<WithName>();

            Assert.AreSame(serializer1, serializer2);
            Assert.AreSame(serializer1, serializer3);
            Assert.AreNotSame(serializer1, serializer4);
        }

        [TestMethod]
        public void PropertySerializersAreBuiltForAnnotatedPublicProperties()
        {
            var serializer = TypeSerializer.Build<WithProperties>();

            Assert.AreEqual(3, serializer.SimplePropertySerializers.Count());
        }

        [TestMethod]
        public void TypeWithSimpleProperties_Serializes()
        {
            var stream = new MemoryStream();
            var serializer = TypeSerializer.Build<WithProperties>();
            var target = new WithProperties {FirstName = "John", LastName = "Smith", Age = 35, IsMarried = true};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteObject(writer, target);

            Assert.AreEqual("<WithProperties FirstName=\"John\" LastName=\"Smith\" IsMarried=\"true\" />", stream.ToUtf8String());
        }

        [TestMethod]
        public void TypeWithSimpleProperties_Deserializes()
        {
            var serializer = TypeSerializer.Build<WithProperties>();
            WithProperties target;

            using (var reader = BuildFragmentReader("<WithProperties FirstName=\"John\" LastName=\"Smith\" IsMarried=\"true\" />".ToStream()))
                target = serializer.ReadObject(reader);

            Assert.AreEqual(new {FirstName="John", LastName="Smith", IsMarried=true}, 
                            new {target.FirstName, target.LastName, target.IsMarried});
        }

        [TestMethod]
        public void TypeWithUserTypeProperties_Serializes()
        {
            var stream = new MemoryStream();
            var serializer = TypeSerializer.Build<WithUserProperties>();
            var target = new WithUserProperties
                         {
                             FirstName = "John",
                             LastName = "Smith",
                             HomeAddress = new Address {StreetNumber = "37", StreetName = "Queen St."}
                         };

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteObject(writer, target);

            Assert.AreEqual("<WithUserProperties FirstName=\"John\" LastName=\"Smith\"><HomeAddress StreetNumber=\"37\" StreetName=\"Queen St.\" /></WithUserProperties>", stream.ToUtf8String());
        }

        [TestMethod]
        public void TypeWithUserTypeProperties_Deserializes()
        {
            var serializer = TypeSerializer.Build<WithUserProperties>();
            WithUserProperties target;

            using (var reader = BuildFragmentReader("<WithUserProperties FirstName=\"John\" LastName=\"Smith\"><HomeAddress StreetNumber=\"37\" StreetName=\"Queen St.\" /></WithUserProperties>".ToStream()))
                target = serializer.ReadObject(reader);

            Assert.AreEqual(new {FirstName = "John", LastName = "Smith"},
                            new {target.FirstName, target.LastName});
            Assert.AreEqual(new {StreetNumber = "37", StreetName= "Queen St."},
                            new {target.HomeAddress.StreetNumber, target.HomeAddress.StreetName});
        }

        #region Helpers
        class MissingDataContract
        {
        }

        [DataContract]
        class WithDataContract
        {
        }

        [DataContract(Name="NewName")]
        class WithName
        {
        }

        [DataContract]
        class WithProperties
        {
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }
            [DataMember]
            public bool IsMarried { get; set; }

            public int Age { get; set; }
            bool IsPrivate { get; set; }
        }

        [DataContract]
        class WithUserProperties
        {
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }
            [DataMember]
            public Address HomeAddress { get; set; }
        }

        [DataContract]
        class Address
        {
            [DataMember]
            public string StreetNumber { get; set; }
            [DataMember]
            public string StreetName { get; set; }
        }
        #endregion
    }
}
