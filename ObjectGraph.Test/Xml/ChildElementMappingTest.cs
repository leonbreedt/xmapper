//
// Copyright (C) 2010-2011 Leon Breedt
// ljb -at- bitserf [dot] org
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectGraph.Test.Xml.Model;
using ObjectGraph.Xml;
using Shouldly;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class ChildElementMappingTest : TestBase
    {
        [TestMethod]
        public void GetFromContainer_ShouldReadFromContainer()
        {
            var person = new Person {Address = new Address {StreetName = "231 Queen Street", City = "Auckland"}};
            var mapping = new ChildElementMapping<Person, Address>(Ns + "Address", x => x.Address);

            var actual = mapping.GetFromContainer(person);

            actual.ShouldBe(person.Address);
        }

        [TestMethod]
        public void SetOnContainer_ShouldWriteToContainer()
        {
            var person = new Person();
            var mapping = new ChildElementMapping<Person, Address>(Ns + "Address", x => x.Address);
            var expected = new Address {StreetName = "231 Queen Street", City = "Auckland"};

            mapping.SetOnContainer(person, expected);

            person.Address.ShouldBe(expected);
        }
    }
}
