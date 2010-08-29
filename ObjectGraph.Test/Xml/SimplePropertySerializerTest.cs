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

using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectGraph.Extensions;
using ObjectGraph.Xml;

namespace ObjectGraph.Test.Xml
{
    [TestClass]
    public class SimplePropertySerializerTest : SerializerTestBase
    {
        [TestMethod]
        public void BoolSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.BooleanValue);
            var target = new WithProperties {BooleanValue = true};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("BooleanValue=\"true\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void BoolDeserializes()
        {
            var serializer = SetupSerializer(x => x.BooleanValue);
            var obj = new WithProperties {BooleanValue = false};

            using (var reader = BuildFragmentReader("<Container BooleanValue=\"true\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, obj);
            }

            Assert.IsTrue(obj.BooleanValue);
        }

        [TestMethod]
        public void CharSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.CharValue);
            var target = new WithProperties {CharValue = 'a'};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("CharValue=\"a\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void CharDeserializes()
        {
            var serializer = SetupSerializer(x => x.CharValue);
            var target = new WithProperties {CharValue = ' '};

            using (var reader = BuildFragmentReader("<Container CharValue=\"a\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual('a', target.CharValue);
        }

        [TestMethod]
        public void SByteSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.SByteValue);
            var target = new WithProperties {SByteValue = -128};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("SByteValue=\"-128\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void SByteDeserializes()
        {
            var serializer = SetupSerializer(x => x.SByteValue);
            var target = new WithProperties {SByteValue = -128};

            using (var reader = BuildFragmentReader("<Container SByteValue=\"127\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(127, target.SByteValue);
        }

        [TestMethod]
        public void ByteSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.ByteValue);
            var target = new WithProperties {ByteValue = 255};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("ByteValue=\"255\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void ByteDeserializes()
        {
            var serializer = SetupSerializer(x => x.ByteValue);
            var target = new WithProperties {ByteValue = 255};

            using (var reader = BuildFragmentReader("<Container ByteValue=\"127\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(127, target.ByteValue);
        }

        [TestMethod]
        public void Int16Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.Int16Value);
            var target = new WithProperties {Int16Value = 32767};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("Int16Value=\"32767\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void Int16Deserializes()
        {
            var serializer = SetupSerializer(x => x.Int16Value);
            var target = new WithProperties { Int16Value = 32767 };

            using (var reader = BuildFragmentReader("<Container Int16Value=\"15535\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(15535, target.Int16Value);
        }

        [TestMethod]
        public void UInt16Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.UInt16Value);
            var target = new WithProperties {UInt16Value = 65535};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("UInt16Value=\"65535\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void UInt16Deserializes()
        {
            var serializer = SetupSerializer(x => x.UInt16Value);
            var target = new WithProperties {UInt16Value = 65535};

            using (var reader = BuildFragmentReader("<Container UInt16Value=\"12345\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345, target.UInt16Value);
        }

        [TestMethod]
        public void Int32Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.Int32Value);
            var target = new WithProperties { Int32Value = 32767 };

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("Int32Value=\"32767\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void Int32Deserializes()
        {
            var serializer = SetupSerializer(x => x.Int32Value);
            var target = new WithProperties { Int32Value = 32767 };

            using (var reader = BuildFragmentReader("<Container Int32Value=\"15535\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(15535, target.Int32Value);
        }

        [TestMethod]
        public void UInt32Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.UInt32Value);
            var target = new WithProperties { UInt32Value = 65535 };

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("UInt32Value=\"65535\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void UInt32Deserializes()
        {
            var serializer = SetupSerializer(x => x.UInt32Value);
            var target = new WithProperties { UInt32Value = 65535 };

            using (var reader = BuildFragmentReader("<Container UInt32Value=\"12345\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345, target.UInt32Value);
        }

        [TestMethod]
        public void Int64Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.Int64Value);
            var target = new WithProperties { Int64Value = 9223372036854775807 };

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("Int64Value=\"9223372036854775807\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void Int64Deserializes()
        {
            var serializer = SetupSerializer(x => x.Int64Value);
            var target = new WithProperties { Int64Value = 9223372036854775807 };

            using (var reader = BuildFragmentReader("<Container Int64Value=\"15535\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(15535, target.Int64Value);
        }

        [TestMethod]
        public void UInt64Serializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.UInt64Value);
            var target = new WithProperties { UInt64Value = 18446744073709551615 };

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("UInt64Value=\"18446744073709551615\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void UInt64Deserializes()
        {
            var serializer = SetupSerializer(x => x.UInt64Value);
            var target = new WithProperties { UInt64Value = 18446744073709551615 };

            using (var reader = BuildFragmentReader("<Container UInt64Value=\"12345\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345UL, target.UInt64Value);
        }

        [TestMethod]
        public void SingleSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.SingleValue);
            var target = new WithProperties {SingleValue = 3.40282347E+38f};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("SingleValue=\"3.40282347E+38\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void SingleDeserializes()
        {
            var serializer = SetupSerializer(x => x.SingleValue);
            var target = new WithProperties {SingleValue = 3.40282347E+38f};

            using (var reader = BuildFragmentReader("<Container SingleValue=\"12345.67\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345.67f, target.SingleValue);
        }

        [TestMethod]
        public void DoubleSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.DoubleValue);
            var target = new WithProperties {DoubleValue = 1.7976931348623157E+308};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("DoubleValue=\"1.7976931348623157E+308\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void DoubleDeserializes()
        {
            var serializer = SetupSerializer(x => x.DoubleValue);
            var target = new WithProperties {DoubleValue = 1.7976931348623157E+308};

            using (var reader = BuildFragmentReader("<Container DoubleValue=\"12345.67\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345.67d, target.DoubleValue);
        }

        [TestMethod]
        public void DecimalSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.DecimalValue);
            var target = new WithProperties {DecimalValue = 79228162514264337593543950335M};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("DecimalValue=\"79228162514264337593543950335\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void DecimalDeserializes()
        {
            var serializer = SetupSerializer(x => x.DecimalValue);
            var target = new WithProperties {DecimalValue = 79228162514264337593543950335M};

            using (var reader = BuildFragmentReader("<Container DecimalValue=\"12345.67\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(12345.67M, target.DecimalValue);
        }

        [TestMethod]
        public void DateTimeSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.DateTimeValue);
            var target = new WithProperties {DateTimeValue = new DateTime(2001, 12, 31, 23, 59, 59)};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("DateTimeValue=\"2001-12-31T23:59:59\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void DateTimeDeserializes()
        {
            var serializer = SetupSerializer(x => x.DateTimeValue);
            var target = new WithProperties {DateTimeValue = DateTime.MinValue};

            using (var reader = BuildFragmentReader("<Container DateTimeValue=\"2001-12-31T23:59:59\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual(new DateTime(2001, 12, 31, 23, 59, 59), target.DateTimeValue);
        }

        [TestMethod]
        public void StringSerializes()
        {
            var stream = new MemoryStream();
            var serializer = SetupSerializer(x => x.StringValue);
            var target = new WithProperties {StringValue = "A String Value"};

            using (var writer = BuildFragmentWriter(stream))
                serializer.WriteProperty(writer, target);

            Assert.AreEqual("StringValue=\"A String Value\"", stream.ToUtf8String());
        }

        [TestMethod]
        public void StringDeserializes()
        {
            var serializer = SetupSerializer(x => x.StringValue);
            var target = new WithProperties {StringValue = "Invalid"};

            using (var reader = BuildFragmentReader("<Container StringValue=\"A String Value\" />".ToStream()))
            {
                reader.Read();
                reader.MoveToAttribute(0);
                serializer.ReadProperty(reader, target);
            }

            Assert.AreEqual("A String Value", target.StringValue);
        }

        #region Helpers
        static SimplePropertySerializer<WithProperties, TPropertyType> SetupSerializer<TPropertyType>(Expression<Func<WithProperties, TPropertyType>> expr)
        {
            return (SimplePropertySerializer<WithProperties, TPropertyType>)BuildPropertySerializerFor(expr);
        }

        [DataContract]
        class WithProperties
        {
            [DataMember]
            public bool BooleanValue { get; set; }
            [DataMember]
            public char CharValue { get; set; }
            [DataMember]
            public sbyte SByteValue { get; set; }
            [DataMember]
            public byte ByteValue { get; set; }
            [DataMember]
            public short Int16Value { get; set; }
            [DataMember]
            public ushort UInt16Value { get; set; }
            [DataMember]
            public short Int32Value { get; set; }
            [DataMember]
            public ushort UInt32Value { get; set; }
            [DataMember]
            public long Int64Value { get; set; }
            [DataMember]
            public ulong UInt64Value { get; set; }
            [DataMember]
            public float SingleValue { get; set; }
            [DataMember]
            public double DoubleValue { get; set; }
            [DataMember]
            public decimal DecimalValue { get; set; }
            [DataMember]
            public DateTime DateTimeValue { get; set; }
            [DataMember]
            public string StringValue { get; set; }
        }
        #endregion
    }
}
