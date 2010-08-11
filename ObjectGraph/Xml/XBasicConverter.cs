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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using ObjectGraph.Extensions;

namespace ObjectGraph.Xml
{
    public static class XBasicConverter
    {
        #region Fields
        static readonly MethodInfo EnumReaderBuilderMethod;
        static readonly MethodInfo EnumWriterBuilderMethod;
        static readonly MethodInfo ParseEnumMethod;
        static readonly Dictionary<TypeCode, object> WritersByTypeCode;
        static readonly Dictionary<TypeCode, object> ReadersByTypeCode;
        static Dictionary<Type, object> _readersByEnumType;
        static Dictionary<Type, object> _writersByEnumType;
        #endregion

        static XBasicConverter()
        {
            EnumReaderBuilderMethod = typeof(XBasicConverter).GetMethod("EnumReaderBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            EnumWriterBuilderMethod = typeof(XBasicConverter).GetMethod("EnumWriterBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            ParseEnumMethod = typeof(XBasicConverter).GetMethod("ParseEnum", BindingFlags.Static | BindingFlags.NonPublic);

            Func<string, string> stringConverter = s => s;

            WritersByTypeCode = new Dictionary<TypeCode, object>(EnumComparer.For<TypeCode>());

            WritersByTypeCode[TypeCode.Boolean] = (Func<bool, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Byte] = (Func<byte, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Char] = (Func<char, string>)XmlConvert.ToString;
#pragma warning disable 0618
            WritersByTypeCode[TypeCode.DateTime] = (Func<DateTime, string>)XmlConvert.ToString;
#pragma warning restore 0618
            WritersByTypeCode[TypeCode.Decimal] = (Func<decimal, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Double] = (Func<double, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Int16] = (Func<short, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Int32] = (Func<int, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Int64] = (Func<long, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.SByte] = (Func<sbyte, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.Single] = (Func<float, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.UInt16] = (Func<ushort, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.UInt32] = (Func<uint, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.UInt64] = (Func<ulong, string>)XmlConvert.ToString;
            WritersByTypeCode[TypeCode.String] = stringConverter;

            ReadersByTypeCode = new Dictionary<TypeCode, object>(EnumComparer.For<TypeCode>());

            ReadersByTypeCode[TypeCode.Boolean] = (Func<string, bool>)XmlConvert.ToBoolean;
            ReadersByTypeCode[TypeCode.Byte] = (Func<string, byte>)XmlConvert.ToByte;
            ReadersByTypeCode[TypeCode.Char] = (Func<string, char>)XmlConvert.ToChar;
#pragma warning disable 0618
            ReadersByTypeCode[TypeCode.DateTime] = (Func<string, DateTime>)XmlConvert.ToDateTime;
#pragma warning restore 0618
            ReadersByTypeCode[TypeCode.Decimal] = (Func<string, decimal>)XmlConvert.ToDecimal;
            ReadersByTypeCode[TypeCode.Double] = (Func<string, double>)XmlConvert.ToDouble;
            ReadersByTypeCode[TypeCode.Int16] = (Func<string, short>)XmlConvert.ToInt16;
            ReadersByTypeCode[TypeCode.Int32] = (Func<string, int>)XmlConvert.ToInt32;
            ReadersByTypeCode[TypeCode.Int64] = (Func<string, long>)XmlConvert.ToInt64;
            ReadersByTypeCode[TypeCode.SByte] = (Func<string, sbyte>)XmlConvert.ToSByte;
            ReadersByTypeCode[TypeCode.Single] = (Func<string, float>)XmlConvert.ToSingle;
            ReadersByTypeCode[TypeCode.UInt16] = (Func<string, ushort>)XmlConvert.ToUInt16;
            ReadersByTypeCode[TypeCode.UInt32] = (Func<string, uint>)XmlConvert.ToUInt32;
            ReadersByTypeCode[TypeCode.UInt64] = (Func<string, ulong>)XmlConvert.ToUInt64;
            ReadersByTypeCode[TypeCode.String] = stringConverter;
        }

        public static object ForWriting(TypeCode code)
        {
            object func;
            if (!WritersByTypeCode.TryGetValue(code, out func))
                return null;
            return func;
        }

        public static object ForReading(TypeCode code)
        {
            object func;
            if (!ReadersByTypeCode.TryGetValue(code, out func))
                return null;
            return func;
        }

        public static object ForReadingEnum(Type type)
        {
            if (_readersByEnumType == null)
                _readersByEnumType = new Dictionary<Type, object>();

            object func;
            if (!_readersByEnumType.TryGetValue(type, out func))
            {
                var builder = EnumReaderBuilderMethod.MakeGenericMethod(type);
                func = builder.Invoke(null, null);
            }
            return func;
        }

        public static object ForWritingEnum(Type type)
        {
            if (_writersByEnumType == null)
                _writersByEnumType = new Dictionary<Type, object>();

            object func;
            if (!_writersByEnumType.TryGetValue(type, out func))
            {
                var builder = EnumWriterBuilderMethod.MakeGenericMethod(type);
                func = builder.Invoke(null, null);
            }
            return func;
        }

// ReSharper disable UnusedMember.Local
        static Func<string, TEnum> EnumReaderBuilder<TEnum>()
// ReSharper restore UnusedMember.Local
        {
            var parserMethodInfo = ParseEnumMethod.MakeGenericMethod(typeof(TEnum));
            return (Func<string, TEnum>)Delegate.CreateDelegate(typeof(Func<string, TEnum>), parserMethodInfo);
        }

// ReSharper disable UnusedMember.Local
        static Func<TEnum, string> EnumWriterBuilder<TEnum>()
// ReSharper restore UnusedMember.Local
        {
            Func<TEnum, string> func = e => e.ToString();
            return func;
        }

// ReSharper disable UnusedMember.Local
        static TEnum ParseEnum<TEnum>(string value)
// ReSharper restore UnusedMember.Local
            where TEnum : struct
        {
            TEnum enumValue;
            if (!Enum.TryParse(value, out enumValue))
                throw new ArgumentException("{0} is not a valid member of {1}".FormatWith(value, typeof(TEnum)));
            return enumValue;
        }
    }
}
