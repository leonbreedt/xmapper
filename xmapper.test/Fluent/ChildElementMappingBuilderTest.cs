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
using Shouldly;
using XMapper.Fluent;
using XMapper.Test.Model;

namespace XMapper.Test
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

            builder.Attribute(Ns + "City", x => x.City, x => x, x => x);
            builder.Element(Ns + "PostCode", x => x.PostCode);
            builder.CollectionElement(Ns + "AreaCode", x => x.AreaCodes);

            var actual = (ChildElementMapping<Person, Address>)builder.Build();

            actual.SetOnContainer(person, address);
            actual.GetFromContainer(person).ShouldBe(address);
            person.Address.ShouldBe(address);
            actual.Attributes.Length.ShouldBe(1);
            actual.ChildElements.Length.ShouldBe(2);
            actual.ChildElements[0].ShouldBeTypeOf(typeof(ChildElementMapping<Address, PostCode>));
            actual.ChildElements[1].ShouldBeTypeOf(typeof(CollectionChildElementMapping<Address, AreaCode>));
        }

        [TestMethod]
        public void EndElement_ShouldReturnParentScope()
        {
            var parentScope = new object();
            var builder = new ChildElementMappingBuilder<Person, Address, object>(parentScope, Ns + "Address", x => x.Address);

            builder.EndElement().ShouldBe(parentScope);
        }
    }
}
