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
using ObjectGraph.Xml.Fluent;
using Shouldly;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class ElementMappingBuilderTest : TestBase
    {
        [TestMethod]
        public void Attributes_ShouldBeMapped()
        {
            var builder = new ElementMappingBuilder<Person>(Ns + "Person");

            builder.Attribute(Ns + "Id", x => x.Id);
            builder.Attribute(Ns + "FirstName", x => x.FirstName);
            builder.Attribute(Ns + "LastName", x => x.LastName);
            builder.Attribute(Ns + "IsEnabled",
                              x => x.IsEnabled,
                              x => bool.Parse(x),
                              x => x.ToString().ToLowerInvariant());

            var mapping = (ElementMapping<Person>)builder.Build();

            mapping.LocalName.ShouldBe("Person");
            mapping.NamespaceUri.ShouldBe(Ns.NamespaceName);
            mapping.Attributes.Length.ShouldBe(4);
            mapping.Attributes[0].ShouldBeTypeOf(typeof(AttributeMapping<Person, long?>));
            mapping.Attributes[1].ShouldBeTypeOf(typeof(AttributeMapping<Person, string>));
            mapping.Attributes[2].ShouldBeTypeOf(typeof(AttributeMapping<Person, string>));
            mapping.Attributes[3].ShouldBeTypeOf(typeof(AttributeMapping<Person, bool>));
        }

        [TestMethod]
        public void Elements_ShouldBeMapped()
        {
            var builder = new ElementMappingBuilder<Person>(Ns + "Person");

            builder.Element(Ns + "Address", x => x.Address);

            var mapping = (ElementMapping<Person>)builder.Build();

            mapping.ChildElements.Length.ShouldBe(1);
            mapping.ChildElements[0].ShouldBeTypeOf(typeof(ElementMapping<Address>));
        }
    }
}
