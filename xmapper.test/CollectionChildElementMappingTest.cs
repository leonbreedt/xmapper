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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using XMapper.Test.Model;

namespace XMapper.Test
{
    [TestClass]
    public class CollectionChildElementMappingTest : TestBase
    {
        [TestMethod]
        public void AddToCollection_Property_ShouldAddMemberAndInstantiateCollectionIfRequired()
        {
            var person = new Person();
            var expected = new ContactMethod{Type = ContactMethodType.HomePhone, Value = "555-1234"};
            var mapping = new CollectionChildElementMapping<Person, ContactMethod>(Ns + "ContactMethod", x => x.ContactMethods);

            mapping.CreateInstance().ShouldBeTypeOf(typeof(ContactMethod));
            mapping.NamespaceUri.ShouldBe(Ns.NamespaceName);
            mapping.LocalName.ShouldBe("ContactMethod");

            mapping.AddToCollection(person, expected);

            person.ContactMethods[0].ShouldBe(expected);
        }

        [TestMethod]
        public void GetCollection_Property_ShouldReturnCollection()
        {
            var person = new Person();
            var expected = new ContactMethod { Type = ContactMethodType.HomePhone, Value = "555-1234" };
            var mapping = new CollectionChildElementMapping<Person, ContactMethod>(Ns + "ContactMethod", x => x.ContactMethods);

            mapping.GetCollection(person).ShouldBe(null);

            mapping.AddToCollection(person, expected);

            mapping.GetCollection(person).ShouldNotBe(null);
            mapping.GetCollection(person).ShouldBe(person.ContactMethods);
        }

        [TestMethod]
        public void AddToCollection_ContainingCollection_ShouldAddMemberToMethodArgument()
        {
            var person = new Person {ContactMethods = new List<ContactMethod>()};
            var expected = new ContactMethod {Type = ContactMethodType.HomePhone, Value = "555-1234"};
            var mapping = new CollectionChildElementMapping<Person, ContactMethod>(Ns + "ContactMethod", null);

            mapping.CreateInstance().ShouldBeTypeOf(typeof(ContactMethod));
            mapping.NamespaceUri.ShouldBe(Ns.NamespaceName);
            mapping.LocalName.ShouldBe("ContactMethod");

            mapping.AddToCollection(person.ContactMethods, expected);

            person.ContactMethods[0].ShouldBe(expected);
        }
    }
}
