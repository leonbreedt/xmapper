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
    public class SerializerTest : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(XmlFormatException))]
        public void DeserializeFragment_InvalidLocalName_ShouldFail()
        {
            const string fragment = @"<Invalid StreetName='231 Queen Street' City='Auckland' />";
            var serializer = new Serializer(FullSchema());

            serializer.Deserialize<Address>(fragment.ToStream());
        }

        [TestMethod]
        [ExpectedException(typeof(XmlFormatException))]
        public void DeserializeFragment_InvalidNamespace_ShouldFail()
        {
            const string fragment = @"<Address StreetName='231 Queen Street' City='Auckland' />";
            var serializer = new Serializer(FullSchema());

            serializer.Deserialize<Address>(fragment.ToStream());
        }

        [TestMethod]
        public void DeserializeFragment_ShouldSucceed()
        {
            const string fragment = @"<Address xmlns='http://test.com' StreetName='231 Queen Street' City='Auckland' />";
            var serializer = new Serializer(FullSchema());

            var actual = serializer.Deserialize<Address>(fragment.ToStream());

            actual.StreetName.ShouldBe("231 Queen Street");
            actual.City.ShouldBe("Auckland");
        }

        [TestMethod]
        public void DeserializeDocument_ShouldSucceed()
        {
            const string document = @"<Person Id='123' FirstName='James' LastName='Jefferson' IsEnabled='true' xmlns='http://test.com'>
                                          <Address StreetName='231 Queen Street' City='Auckland' />
                                          <ContactMethods>
                                              <ContactMethod Type='Email' Value='james@jefferson.com' />
                                              <AddressContactMethod Type='Address' Value='Auckland City' StreetName='232 Queen Street' />
                                              <ContactMethod Type='HomePhone' Value='555-1234' />
                                          </ContactMethods>
                                      </Person>";
            var serializer = new Serializer(FullSchema());

            var actual = serializer.Deserialize<Person>(document.ToStream());

            actual.Id.ShouldBe(123);
            actual.Address.StreetName.ShouldBe("231 Queen Street");
            actual.Address.City.ShouldBe("Auckland");
            actual.ContactMethods.Count.ShouldBe(3);
            actual.ContactMethods[0].Type.ShouldBe(ContactMethodType.Email);
            actual.ContactMethods[0].Value.ShouldBe("james@jefferson.com");
            actual.ContactMethods[1].Type.ShouldBe(ContactMethodType.Address);
            actual.ContactMethods[1].Value.ShouldBe("Auckland City");
            actual.ContactMethods[1].ShouldBeTypeOf(typeof(AddressContactMethod));
            actual.ContactMethods[1].As<AddressContactMethod>().StreetName.ShouldBe("232 Queen Street");
            actual.ContactMethods[2].Type.ShouldBe(ContactMethodType.HomePhone);
            actual.ContactMethods[2].Value.ShouldBe("555-1234");
        }

        static SchemaDescription FullSchema()
        {
            var description = new FluentSchemaDescription();

            description.Element<Person>(Ns + "Person")
                       .Attribute("Id", x => x.Id)
                       .Attribute("FirstName", x => x.FirstName)
                       .Attribute("LastName", x => x.LastName)
                       .Attribute("IsEnabled", x => x.IsEnabled)
                       .Element(Ns + "Address", x => x.Address)
                           .Attribute("StreetName", x => x.StreetName)
                           .Attribute("City", x => x.City)
                       .EndElement()
                       .ContainerElement(Ns + "ContactMethods", x => x.ContactMethods)
                           .MemberElement(Ns + "ContactMethod")
                               .Attribute("Type", x => x.Type)
                               .Attribute("Value", x => x.Value)
                           .EndElement()
                           .MemberElement<AddressContactMethod>(Ns + "AddressContactMethod")
                               .Attribute("Type", x => x.Type)
                               .Attribute("Value", x => x.Value)
                               .Attribute("StreetName", x => x.StreetName)
                           .EndElement()
                       .EndContainerElement();

            return description.Build();
        }
    }
}
