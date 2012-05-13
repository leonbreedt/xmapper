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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using XMapper.Fluent;
using XMapper.Test.Model;

namespace XMapper.Test
{
    [TestClass]
    public class CollectionChildElementMappingBuilderTest : TestBase
    {
        [TestMethod]
        public void Build_ShouldCreateValidMapping()
        {
            var person = new Person();
            var method = new ContactMethod {Type = ContactMethodType.HomePhone, Value = "555-1234"};
            var builder = new CollectionChildElementMappingBuilder<Person, ContactMethod, object>(null, Ns + "ContactMethod", x => x.ContactMethods);

            builder.Attribute(Ns + "Value", x => x.Value);
            builder.CollectionElement<string>(Ns + "Value1");
            builder.CollectionElement(Ns + "Value2", x => x.AdditionalValues);

            var actual = (CollectionChildElementMapping<Person, ContactMethod>)builder.Build();

            actual.AddToCollection(person, method);
            person.ContactMethods[0].ShouldBe(method);
            actual.Attributes.Length.ShouldBe(1);
            actual.ChildElements.Length.ShouldBe(2);
        }

        [TestMethod]
        public void EndElement_ShouldReturnParentScope()
        {
            var parentScope = new object();
            var builder = new CollectionChildElementMappingBuilder<Person, ContactMethod, object>(parentScope, Ns + "ContactMethod", x => x.ContactMethods);

            builder.EndElement().ShouldBe(parentScope);
        }
    }
}
