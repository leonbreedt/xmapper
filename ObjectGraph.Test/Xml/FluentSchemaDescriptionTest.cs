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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectGraph.Test.Xml.Model;
using ObjectGraph.Xml;
using Shouldly;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class FluentSchemaDescriptionTest : TestBase
    {
        [TestMethod]
        public void Build_ShouldBuildFullDescription()
        {
            var builder = new FluentSchemaDescription();

            builder.Element<Person>(Ns + "Person")
                    .Attribute(Ns + "Id", x => x.Id)
                    .Attribute(Ns + "FirstName", x => x.FirstName)
                    .Attribute(Ns + "LastName", x => x.LastName)
                    .Attribute(Ns + "IsEnabled", x => x.IsEnabled)
                    .Element(Ns + "Address", x => x.Address)
                        .Attribute(Ns + "StreetName", x => x.StreetName)
                        .Attribute(Ns + "City", x => x.City)
                    .EndElement()
                    .ContainerElement(Ns + "ContactMethods", x => x.ContactMethods)
                        .MemberElement(Ns + "ContactMethod")
                            .Attribute(Ns + "Type", x => x.Type)
                            .Attribute(Ns + "Value", x => x.Value)
                        .EndElement()
                        .MemberElement<AddressContactMethod>(Ns + "AddressContactMethod")
                            .Attribute(Ns + "Type", x => x.Type)
                            .Attribute(Ns + "Value", x => x.Value)
                            .Attribute(Ns + "StreetName", x => x.StreetName)
                        .EndElement()
                    .EndContainerElement();

            var schema = builder.Build();

            schema.Mappings.Count().ShouldBe(5);
            schema.TryFindMappingForType<Person>().ShouldBeTypeOf(typeof(ElementMapping<Person>));
            schema.TryFindMappingForType<Address>().ShouldBeTypeOf(typeof(ChildElementMapping<Person, Address>));
            schema.TryFindMappingForType<ItemCollection<ContactMethod>>().ShouldBeTypeOf(typeof(ContainerElementMapping<Person, ContactMethod>));
            schema.TryFindMappingForType<ContactMethod>().ShouldBeTypeOf(typeof(ElementMapping<ContactMethod>));
            schema.TryFindMappingForType<AddressContactMethod>().ShouldBeTypeOf(typeof(ElementMapping<AddressContactMethod>));
        }
    }
}
