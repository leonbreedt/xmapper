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
    public class ContainerElementMappingBuilderTest : TestBase
    {
        [TestMethod]
        public void Build_ShouldCreateValidMapping()
        {
            var person = new Person();
            var contactMethods = new ItemCollection<ContactMethod>();
            var builder = new ContainerElementMappingBuilder<Person, ContactMethod, object>(null, Ns + "ContactMethods", x => x.ContactMethods);

            var actual = (ContainerElementMapping<Person, ContactMethod>)builder.Build();

            actual.SetCollectionOnTarget(person, contactMethods);
            actual.GetCollectionFromTarget(person).ShouldBe(contactMethods);
        }

        [TestMethod]
        public void MemberElement_ShouldAddMemberElementMapping()
        {
            var builder = new ContainerElementMappingBuilder<Person, ContactMethod, object>(null, Ns + "ContactMethods", x => x.ContactMethods);

            builder.MemberElement(Ns + "ContactMethod")
                       .Attribute(Ns + "Type", x => x.Type)
                       .Attribute(Ns + "Value", x => x.Value)
                   .EndElement()
                   .MemberElement<AddressContactMethod>(Ns + "Address")
                       .Attribute(Ns + "Type", x => x.Type)
                       .Attribute(Ns + "Value", x => x.Value)
                       .Attribute(Ns + "StreetName", x => x.StreetName)
                   .EndElement();

            var actual = builder.Build();

            actual.ChildElements.Length.ShouldBe(2);
            actual.ChildElements[0].ShouldBeTypeOf(typeof(ElementMapping<ContactMethod>));
            actual.ChildElements[1].ShouldBeTypeOf(typeof(ElementMapping<AddressContactMethod>));
        }

        [TestMethod]
        public void EndContainerElement_ShouldReturnParentScope()
        {
            var parentScope = new object();
            var builder = new ContainerElementMappingBuilder<Person, ContactMethod, object>(parentScope, Ns + "ContactMethods", x => x.ContactMethods);

            builder.EndContainerElement().ShouldBe(parentScope);
        }
    }
}
