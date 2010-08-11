//
// Copyright (C) 2010 Leon Breedt
// bitserf -at- gmail [dot] com
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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectGraph.Test
{
    [TestClass]
    public class XmlTest
    {
        [TestMethod]
        public void XmlTest_SerializesAndDeserializes()
        {
            var expected = new SalesAgent { Id = "jd", FirstName = "John", LastName = "Smith", Role = PersonRole.Sales };

            var stream = new MemoryStream();

            Serialization.Save(expected, stream, SerializationFormat.Xml);
        }
    }
}
