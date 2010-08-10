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

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectGraph.Test
{
    [TestClass]
    public class ProtoBufTest
    {
        [TestMethod]
        public void ProtoBuf_FlatObject_Serializes_And_Deserializes()
        {
            var expected = new SalesAgent {Id = "jd", FirstName = "John", LastName = "Smith", Role = PersonRole.Sales};
            var stream = new MemoryStream();
            var index = new InMemoryObjectIndex();

            Serialization.Save(expected, stream, SerializationFormat.ProtocolBuffer);

            Assert.AreNotEqual(0, stream.Length);

            stream.Seek(0, SeekOrigin.Begin);

            var actual = Serialization.Load<SalesAgent>(stream, SerializationFormat.ProtocolBuffer, index);

            Assert.AreNotSame(expected, actual);
            Assert.AreEqual(new {expected.Id, actual.FirstName, actual.LastName, Type = actual.Role},
                            new {actual.Id, actual.FirstName, actual.LastName, Type = actual.Role});
            Assert.AreNotSame(expected, actual);

            actual.FirstName = "James";

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void ProtoBuf_EmbeddedCollection_Serializes_And_Deserializes_DoesNotPreserveObjectReferences()
        {
            var superior = new Manager {Id = "jw", FirstName = "James", LastName = "Smith", Role = PersonRole.Manager, CarParkNumber = 1};
            var manager1 = new Manager {Id = "jd", FirstName = "John", LastName = "Smith", Role = PersonRole.Manager, CarParkNumber = 33, Superior = superior};
            var manager2 = new Manager {Id = "pw", FirstName = "Paul", LastName = "Smith", Role = PersonRole.Manager, CarParkNumber = 33, Superior = superior};

            // protobuf does not support a collection as the root object, so wrap it.

            var managers = new List<Manager> {superior, manager1, manager2};
            var expected = new Document {Managers = managers};

            var stream = new MemoryStream();

            Serialization.Save(expected, stream, SerializationFormat.ProtocolBuffer);

            Assert.AreNotEqual(0, stream.Length);

            stream.Seek(0, SeekOrigin.Begin);

            var actual = Serialization.Load<Document>(stream, SerializationFormat.ProtocolBuffer);

            Assert.AreNotSame(expected, actual);
            CollectionAssert.AreEqual(expected.Managers, actual.Managers);
            Assert.AreNotSame(actual.Managers[1].Superior, actual.Managers[2].Superior);
        }
    }
}
