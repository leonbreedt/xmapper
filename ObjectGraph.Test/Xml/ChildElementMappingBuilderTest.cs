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
    public class ChildElementMappingBuilderTest : TestBase
    {
        [TestMethod]
        public void Build_ShouldCreateValidMapping()
        {
            var person = new Person();
            var address = new Address {StreetName = "231 Queen Street", City = "Auckland"};
            var builder = new ChildElementMappingBuilder<Person, Address, object>(null, Ns + "Address", x => x.Address);

            var actual = (ChildElementMapping<Person,Address>)builder.Build();

            actual.SetOnContainer(person, address);

            actual.GetFromContainer(person).ShouldBe(address);
            actual.NamespaceUri.ShouldBe(Ns.NamespaceName);
            actual.LocalName.ShouldBe("Address");
        }

        [TestMethod]
        public void EndChild_ShouldReturnParentScope()
        {
            var parentScope = new object();
            var builder = new ChildElementMappingBuilder<Person, Address, object>(parentScope, Ns + "Address", x => x.Address);

            builder.EndChild().ShouldBe(parentScope);
        }
    }
}
