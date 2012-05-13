//
// Copyright (C) 2010-2012 Leon Breedt
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using XMapper.Test.Model;

namespace XMapper.Test
{
    [TestClass]
    public class AttributeMappingTest : TestBase
    {
        [TestMethod]
        public void GetValueInXmlForm_ShouldReturnXmlRepresentation()
        {
            var person = new Person {IsEnabled = true};
            var mapping = new AttributeMapping<Person, bool>(Ns + "Person", x => x.IsEnabled);

            mapping.GetValueInXmlForm(person).ShouldBe("true");
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnsupportedGetterDataType_ThrowsException()
        {
            new AttributeMapping<Test, UnsupportedDataType>("Test", x => x.Unsupported);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnsupportedSetterDataType_ThrowsException()
        {
            new AttributeMapping<Test, UnsupportedDataType>("Test", x => x.Unsupported, x => new UnsupportedDataType(), null);
        }

        class Test
        {
            public UnsupportedDataType Unsupported { get; set; }
        }

        class UnsupportedDataType
        {
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
