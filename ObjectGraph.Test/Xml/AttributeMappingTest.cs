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
    public class AttributeMappingTest : TestBase
    {
        [TestMethod]
        public void GetValue_ShouldGetPropertyValue()
        {
            var person = new Person {Id = 1};
            var mapping = new AttributeMapping<Person, long?>(Ns + "Person", x => x.Id);

            mapping.GetValue(person).ShouldBe(1);
        }

        [TestMethod]
        public void GetValueInXmlForm_ShouldReturnXmlRepresentation()
        {
            var person = new Person {IsEnabled = true};
            var mapping = new AttributeMapping<Person, bool>(Ns + "Person", x => x.IsEnabled);

            mapping.GetValueInXmlForm(person).ShouldBe("true");
        }

        [TestMethod]
        public void SetValue_ShouldSetPropertyValue()
        {
            var person = new Person();
            var mapping = new AttributeMapping<Person, string>(Ns + "Person", x => x.FirstName);

            mapping.SetValue(person, "James");

            person.FirstName.ShouldBe("James");
        }

        [TestMethod]
        public void SetValueFromXmlForm_ShouldSetPropertyValue()
        {
            var person = new Person();
            var mapping = new AttributeMapping<Person, bool>(Ns + "Person", x => x.IsEnabled);

            mapping.SetValueFromXmlForm(person, "true");

            person.IsEnabled.ShouldBe(true);
        }

        [TestMethod]
        public void CustomDeserializer_ShouldDeserializeCustomValue()
        {
            var person = new Person();
            var mapping = new AttributeMapping<Person, Address>(Ns + "Person", x => x.Address, UnpackAddressFromAttribute, PackAddressForAttribute);

            mapping.SetValueFromXmlForm(person, "231 Queen Street;Auckland");

            person.Address.StreetName.ShouldBe("231 Queen Street");
            person.Address.City.ShouldBe("Auckland");
        }

        [TestMethod]
        public void CustomSerializer_ShouldSerializeCustomValue()
        {
            var person = new Person {Address = new Address {StreetName = "231 Queen Street", City = "Auckland"}};
            var mapping = new AttributeMapping<Person, Address>(Ns + "Person", x => x.Address, UnpackAddressFromAttribute, PackAddressForAttribute);

            var actual = mapping.GetValueInXmlForm(person);

            actual.ShouldBe("231 Queen Street;Auckland");
        }

        static Address UnpackAddressFromAttribute(string packedAddress)
        {
            var parts = packedAddress.Split(';');
            return new Address {StreetName = parts[0], City = parts[1]};
        }

        static string PackAddressForAttribute(Address address)
        {
            return address.StreetName + ";" + address.City;
        }
    }
}
