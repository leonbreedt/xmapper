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
            const string document = @"<Document xmlns='http://test.com'>
                                        <Person Id='123' FirstName='James' LastName='Jefferson' IsEnabled='true'>
                                          <Address StreetName='231 Queen Street' City='Auckland' />
                                          <ContactMethods>
                                              <ContactMethod Type='Email' Value='james@jefferson.com' />
                                              <AddressContactMethod Type='Address' Value='Auckland City' StreetName='232 Queen Street' />
                                              <ContactMethod Type='HomePhone' Value='555-1234' />
                                          </ContactMethods>
                                        </Person>
                                        <Person Id='124' FirstName='Paul' LastName='Jefferson' IsEnabled='false'>
                                          <Address StreetName='500 Dominion Road' City='Auckland' />
                                        </Person>
                                      </Document>";
            var serializer = new Serializer(FullSchema());

            var actual = serializer.Deserialize<Document>(document.ToStream());

            actual.Persons.Count.ShouldBe(2);

            var person1 = actual.Persons[0];
            var person2 = actual.Persons[1];

            person1.Id.ShouldBe(123);
            person1.Address.StreetName.ShouldBe("231 Queen Street");
            person1.Address.City.ShouldBe("Auckland");
            person1.ContactMethods.Count.ShouldBe(3);
            person1.ContactMethods[0].Type.ShouldBe(ContactMethodType.Email);
            person1.ContactMethods[0].Value.ShouldBe("james@jefferson.com");
            person1.ContactMethods[1].Type.ShouldBe(ContactMethodType.Address);
            person1.ContactMethods[1].Value.ShouldBe("Auckland City");
            person1.ContactMethods[1].ShouldBeTypeOf(typeof(AddressContactMethod));
            person1.ContactMethods[1].As<AddressContactMethod>().StreetName.ShouldBe("232 Queen Street");
            person1.ContactMethods[2].Type.ShouldBe(ContactMethodType.HomePhone);
            person1.ContactMethods[2].Value.ShouldBe("555-1234");

            person2.Id.ShouldBe(124);
            person2.FirstName.ShouldBe("Paul");
            person2.LastName.ShouldBe("Jefferson");
            person2.IsEnabled.ShouldBe(false);
            person2.Address.StreetName.ShouldBe("500 Dominion Road");
            person2.Address.City.ShouldBe("Auckland");
        }

        static SchemaDescription FullSchema()
        {
            var description = new FluentSchemaDescription();

            description.Element<Document>(Ns + "Document")
                           .CollectionElement(Ns + "Person", x => x.Persons)
                               .Attribute("Id", x => x.Id)
                               .Attribute("FirstName", x => x.FirstName)
                               .Attribute("LastName", x => x.LastName)
                               .Attribute("IsEnabled", x => x.IsEnabled)
                               .Element(Ns + "Address", x => x.Address)
                                   .Attribute("StreetName", x => x.StreetName)
                                   .Attribute("City", x => x.City)
                               .EndElement()
                               .Element(Ns + "ContactMethods", x => x.ContactMethods)
                                   .CollectionElement<ContactMethod>(Ns + "ContactMethod")
                                       .Attribute("Type", x => x.Type)
                                       .Attribute("Value", x => x.Value)
                                   .EndElement()
                                   .CollectionElement<AddressContactMethod>(Ns + "AddressContactMethod")
                                       .Attribute("Type", x => x.Type)
                                       .Attribute("Value", x => x.Value)
                                       .Attribute("StreetName", x => x.StreetName)
                                   .EndElement()
                               .EndElement()
                           .EndElement();

            return description.Build();
        }
    }
}
