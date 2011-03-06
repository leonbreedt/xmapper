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

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMapper.Fluent;
using XMapper.Test.Model;

namespace XMapper.Test
{
    [TestClass]
    public class PerformanceTest : TestBase
    {
        #region Fields
        string _fileName;
        #endregion

        [TestInitialize]
        public void Initialize()
        {
            _fileName = Path.GetTempFileName();
            Debug.WriteLine("Temp file " + _fileName);
        }

        [TestCleanup, Ignore]
        public void Cleanup()
        {
            File.Delete(_fileName);
        }

        [TestCleanup, Ignore]
        public void Deserialization_PerformanceTest()
        {
            const int numberOfRecords = 50000;

            var serializer = new Serializer(FullSchema());

            using (var stream = BuildLargeXml(numberOfRecords)) // Roughly 25MB
            {
                long length = stream.Length;
                Benchmark("XMapper", length, () => { stream.Seek(0, SeekOrigin.Begin); serializer.Deserialize<Document>(stream); });
            }
        }

        [TestCleanup, Ignore]
        public void Deserialization_XDocument_PerformanceTest()
        {
            const int numberOfRecords = 50000;

            using (var stream = BuildLargeXml(numberOfRecords)) // Roughly 25MB
            {
                long length = stream.Length;
                Benchmark("XDocument", length, () => { stream.Seek(0, SeekOrigin.Begin); XDocument.Load(stream); });
            }
        }

        [TestCleanup, Ignore]
        public void Serialization_XMapper_PerformanceTest()
        {
            const int numberOfRecords = 50000;

            Document document;
            Stream stream;
            var serializer = new Serializer(FullSchema());
            long length;

            using (stream = BuildLargeXml(numberOfRecords)) // Roughly 25MB
            {
                length = stream.Length;
                document = new Serializer(FullSchema()).Deserialize<Document>(stream);
            }

            using (stream = File.Create(_fileName))
                Benchmark("XMapper", length, () => { stream.SetLength(0); serializer.Serialize(stream, document); });
        }

        static void Benchmark(string name, long length, Action customAction)
        {
            const int numberOfPasses = 3;
            double totalMilliseconds = 0;
            double totalSeconds = 0;
            double totalMb = 0;


            for (int i = 0; i < numberOfPasses; i++)
            {
                var before = DateTime.Now;
                customAction();
                var after = DateTime.Now;

                var mb = length / (double)1048576;
                var elapsed = (after - before);

                totalMb += mb;
                totalMilliseconds += elapsed.TotalMilliseconds;
                totalSeconds += elapsed.TotalSeconds;

                Debug.WriteLine("[{4}] " + name + " {0:F2}MB deserialized in {1} milliseconds ({2:F3} seconds), {3:F3} MB/sec.",
                                mb,
                                elapsed.TotalMilliseconds,
                                elapsed.TotalSeconds,
                                mb / elapsed.TotalSeconds,
                                i);
            }

            Debug.WriteLine("OVERALL: " + name + " {0:F2}MB deserialized in {1} milliseconds ({2:F3} seconds), {3:F3} MB/sec, average {4} milliseconds",
                            totalMb,
                            totalMilliseconds,
                            totalSeconds,
                            totalMb / totalSeconds,
                            totalMilliseconds / numberOfPasses);
        }

        Stream BuildLargeXml(int numberOfRecords)
        {
            // use writer, so that our test doesn't break if we have a problem in our own serialization

            var before = DateTime.Now;

            long length;

            using (var stream = File.OpenWrite(_fileName))
            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Persons", Ns.NamespaceName);
                for (int i = 1; i <= numberOfRecords; i++)
                {
                    writer.WriteStartElement("Person", Ns.NamespaceName);
                    writer.WriteAttributeString("Id", i.ToString());
                    writer.WriteAttributeString("FirstName", "James" + i);
                    writer.WriteAttributeString("LastName", "Jefferson" + i);
                    writer.WriteAttributeString("DateOfBirth", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Unspecified));
                    writer.WriteAttributeString("TimeSinceLastLogin", TimeSpan.FromMinutes(20).ToString());

                    writer.WriteElementString("IsEnabled", Ns.NamespaceName, "true");

                    writer.WriteStartElement("Address", Ns.NamespaceName);
                    writer.WriteAttributeString("StreetName", i + " Queen Street");
                    writer.WriteAttributeString("City", "Auckland " + i);
                    writer.WriteString("Some comments " + i);
                    writer.WriteEndElement();

                    writer.WriteStartElement("ContactMethods", Ns.NamespaceName);

                    writer.WriteStartElement("ContactMethod", Ns.NamespaceName);
                    writer.WriteAttributeString("Type", "Email");
                    writer.WriteAttributeString("Value", "james" + i + "@jefferson.com");
                    writer.WriteEndElement();

                    writer.WriteStartElement("AddressContactMethod", Ns.NamespaceName);
                    writer.WriteAttributeString("Type", "Address");
                    writer.WriteAttributeString("Value", "Auckland City " + i);
                    writer.WriteAttributeString("StreetName", i + " Queen Street");
                    writer.WriteEndElement();

                    writer.WriteStartElement("ContactMethod", Ns.NamespaceName);
                    writer.WriteAttributeString("Type", "HomePhone");
                    writer.WriteAttributeString("Value", "555-" + i);
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                
                writer.WriteEndElement();
                writer.WriteEndDocument();

                length = stream.Length;
            }

            var after = DateTime.Now;
            var mb = length / (double)1048576;
            var elapsed = (after - before);

            Debug.WriteLine("{0:F2}MB of test file created in {1} milliseconds ({2:F3} seconds), {3:F3} MB/sec.",
                                            mb,
                                            elapsed.TotalMilliseconds,
                                            elapsed.TotalSeconds,
                                            mb / elapsed.TotalSeconds);
            return File.OpenRead(_fileName);
        }

        internal static SchemaDescription FullSchema()
        {
            var description = new FluentSchemaDescription();

            description.Element<Document>(Ns + "Persons")
                           .CollectionElement(Ns + "Person", x => x.Persons)
                               .Attribute("Id", x => x.Id)
                               .Attribute("FirstName", x => x.FirstName)
                               .Attribute("LastName", x => x.LastName)
                               .Attribute("DateOfBirth", x => x.DateOfBirth)
                               .Attribute("TimeSinceLastLogin", x => x.TimeSinceLastLogin,
                                                                x => x != null ? TimeSpan.Parse(x) : (TimeSpan?)null,
                                                                x => x != null ? x.ToString() : (string)null)
                               .TextElement(Ns + "IsEnabled", x => x.IsEnabled)
                               .Element(Ns + "Address", x => x.Address)
                                   .Attribute("StreetName", x => x.StreetName)
                                   .Attribute("City", x => x.City)
                                   .TextContent(x => x.Comments)
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
