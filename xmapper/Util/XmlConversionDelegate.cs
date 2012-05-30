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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace XMapper.Util
{
    /// <summary>
    /// Helper class for getting at delegates to convert between XML representations and their corresponding CLR value types.
    /// As a convenience hack, we also support arbitrary enums, they're just converted to their string representations and back.
    /// </summary>
    internal static class XmlConversionDelegate
    {
        #region Fields
        static readonly MethodInfo EnumReaderBuilderMethod;
        static readonly MethodInfo NullableEnumReaderBuilderMethod;
        static readonly MethodInfo EnumWriterBuilderMethod;
        static readonly MethodInfo NullableEnumWriterBuilderMethod;
        static readonly MethodInfo ParseEnumMethod;
        static readonly MethodInfo ParseNullableEnumMethod;
        static readonly Dictionary<int, object> WritersByTypeCode;
        static readonly Dictionary<int, object> ReadersByTypeCode;
        static readonly Dictionary<Type, object> NullableWritersByType;
        static readonly Dictionary<Type, object> NullableReadersByType;
        static Dictionary<Type, object> _readersByEnumType;
        static Dictionary<Type, object> _writersByEnumType;
        #endregion

        static XmlConversionDelegate()
        {
            EnumReaderBuilderMethod = typeof(XmlConversionDelegate).GetMethod("EnumReaderBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            NullableEnumReaderBuilderMethod = typeof(XmlConversionDelegate).GetMethod("NullableEnumReaderBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            EnumWriterBuilderMethod = typeof(XmlConversionDelegate).GetMethod("EnumWriterBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            NullableEnumWriterBuilderMethod = typeof(XmlConversionDelegate).GetMethod("NullableEnumWriterBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            ParseEnumMethod = typeof(XmlConversionDelegate).GetMethod("ParseEnum", BindingFlags.Static | BindingFlags.NonPublic);
            ParseNullableEnumMethod = typeof(XmlConversionDelegate).GetMethod("ParseNullableEnum", BindingFlags.Static | BindingFlags.NonPublic);

            Func<string, string> stringConverter = s => s;

            WritersByTypeCode = new Dictionary<int, object>();

            WritersByTypeCode[(int)TypeCode.Boolean] = (Func<bool, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Byte] = (Func<byte, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Char] = (Func<char, string>)XmlConvert.ToString;
#pragma warning disable 0618
            WritersByTypeCode[(int)TypeCode.DateTime] = (Func<DateTime, string>)(d => XmlConvert.ToString(d, XmlDateTimeSerializationMode.Unspecified));
#pragma warning restore 0618
            WritersByTypeCode[(int)TypeCode.Decimal] = (Func<decimal, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Double] = (Func<double, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Int16] = (Func<short, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Int32] = (Func<int, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Int64] = (Func<long, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.SByte] = (Func<sbyte, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.Single] = (Func<float, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.UInt16] = (Func<ushort, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.UInt32] = (Func<uint, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.UInt64] = (Func<ulong, string>)XmlConvert.ToString;
            WritersByTypeCode[(int)TypeCode.String] = stringConverter;

            NullableWritersByType = new Dictionary<Type, object>();

            NullableWritersByType[typeof(bool?)] = CreateNullableWriter<bool>(XmlConvert.ToString);
            NullableWritersByType[typeof(byte?)] = CreateNullableWriter<byte>(XmlConvert.ToString);
            NullableWritersByType[typeof(char?)] = CreateNullableWriter<char>(XmlConvert.ToString);
            NullableWritersByType[typeof(DateTime?)] = CreateNullableWriter<DateTime>(d => XmlConvert.ToString(d, XmlDateTimeSerializationMode.Unspecified));
            NullableWritersByType[typeof(decimal?)] = CreateNullableWriter<decimal>(XmlConvert.ToString);
            NullableWritersByType[typeof(double?)] = CreateNullableWriter<double>(XmlConvert.ToString);
            NullableWritersByType[typeof(short?)] = CreateNullableWriter<short>(XmlConvert.ToString);
            NullableWritersByType[typeof(int?)] = CreateNullableWriter<int>(XmlConvert.ToString);
            NullableWritersByType[typeof(long?)] = CreateNullableWriter<long>(XmlConvert.ToString);
            NullableWritersByType[typeof(sbyte?)] = CreateNullableWriter<sbyte>(XmlConvert.ToString);
            NullableWritersByType[typeof(float?)] = CreateNullableWriter<float>(XmlConvert.ToString);
            NullableWritersByType[typeof(ushort?)] = CreateNullableWriter<ushort>(XmlConvert.ToString);
            NullableWritersByType[typeof(uint?)] = CreateNullableWriter<uint>(XmlConvert.ToString);
            NullableWritersByType[typeof(ulong?)] = CreateNullableWriter<ulong>(XmlConvert.ToString);

            ReadersByTypeCode = new Dictionary<int, object>();

            ReadersByTypeCode[(int)TypeCode.Boolean] = (Func<string, bool>)StringToBoolean;
            ReadersByTypeCode[(int)TypeCode.Byte] = (Func<string, byte>)XmlConvert.ToByte;
            ReadersByTypeCode[(int)TypeCode.Char] = (Func<string, char>)XmlConvert.ToChar;
#pragma warning disable 0618
            ReadersByTypeCode[(int)TypeCode.DateTime] = (Func<string, DateTime>)(s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Unspecified));
#pragma warning restore 0618
            ReadersByTypeCode[(int)TypeCode.Decimal] = (Func<string, decimal>)XmlConvert.ToDecimal;
            ReadersByTypeCode[(int)TypeCode.Double] = (Func<string, double>)XmlConvert.ToDouble;
            ReadersByTypeCode[(int)TypeCode.Int16] = (Func<string, short>)XmlConvert.ToInt16;
            ReadersByTypeCode[(int)TypeCode.Int32] = (Func<string, int>)XmlConvert.ToInt32;
            ReadersByTypeCode[(int)TypeCode.Int64] = (Func<string, long>)XmlConvert.ToInt64;
            ReadersByTypeCode[(int)TypeCode.SByte] = (Func<string, sbyte>)XmlConvert.ToSByte;
            ReadersByTypeCode[(int)TypeCode.Single] = (Func<string, float>)XmlConvert.ToSingle;
            ReadersByTypeCode[(int)TypeCode.UInt16] = (Func<string, ushort>)XmlConvert.ToUInt16;
            ReadersByTypeCode[(int)TypeCode.UInt32] = (Func<string, uint>)XmlConvert.ToUInt32;
            ReadersByTypeCode[(int)TypeCode.UInt64] = (Func<string, ulong>)XmlConvert.ToUInt64;
            ReadersByTypeCode[(int)TypeCode.String] = stringConverter;

            NullableReadersByType = new Dictionary<Type, object>();

            NullableReadersByType[typeof(bool?)] = CreateNullableReader(StringToBoolean);
            NullableReadersByType[typeof(byte?)] = CreateNullableReader(XmlConvert.ToByte);
            NullableReadersByType[typeof(char?)] = CreateNullableReader(XmlConvert.ToChar);
            NullableReadersByType[typeof(DateTime?)] = CreateNullableReader(s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Unspecified));
            NullableReadersByType[typeof(decimal?)] = CreateNullableReader(XmlConvert.ToDecimal);
            NullableReadersByType[typeof(double?)] = CreateNullableReader(XmlConvert.ToDouble);
            NullableReadersByType[typeof(short?)] = CreateNullableReader(XmlConvert.ToInt16);
            NullableReadersByType[typeof(int?)] = CreateNullableReader(XmlConvert.ToInt32);
            NullableReadersByType[typeof(long?)] = CreateNullableReader(XmlConvert.ToInt64);
            NullableReadersByType[typeof(sbyte?)] = CreateNullableReader(XmlConvert.ToSByte);
            NullableReadersByType[typeof(float?)] = CreateNullableReader(XmlConvert.ToSingle);
            NullableReadersByType[typeof(ushort?)] = CreateNullableReader(XmlConvert.ToUInt16);
            NullableReadersByType[typeof(uint?)] = CreateNullableReader(XmlConvert.ToUInt32);
            NullableReadersByType[typeof(ulong?)] = CreateNullableReader(XmlConvert.ToUInt64);
        }

        static bool StringToBoolean(string arg)
        {
            return arg != null ? XmlConvert.ToBoolean(arg.ToLowerInvariant()) : false;
        }

        internal static object ForWriting(Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type);

            object func;
            if (actualType != null)
            {
                if (!NullableWritersByType.TryGetValue(type, out func))
                    return null;
            }
            else if (!WritersByTypeCode.TryGetValue((int)Type.GetTypeCode(type), out func))
                return null;

            return func;
        }

        internal static object ForReading(Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type);

            object func;
            if (actualType != null)
            {
                if (!NullableReadersByType.TryGetValue(type, out func))
                    return null;
            }
            else if (!ReadersByTypeCode.TryGetValue((int)Type.GetTypeCode(type), out func))
                return null;

            return func;
        }

        internal static object ForReadingEnum(Type type)
        {
            if (_readersByEnumType == null)
                _readersByEnumType = new Dictionary<Type, object>();

            object func;
            if (!_readersByEnumType.TryGetValue(type, out func))
            {
                var actualType = Nullable.GetUnderlyingType(type);

                MethodInfo builder;
                if (actualType != null)
                    builder = NullableEnumReaderBuilderMethod.MakeGenericMethod(actualType);
                else
                    builder = EnumReaderBuilderMethod.MakeGenericMethod(type);

                func = builder.Invoke(null, null);
            }
            return func;
        }

        internal static object ForWritingEnum(Type type)
        {
            if (_writersByEnumType == null)
                _writersByEnumType = new Dictionary<Type, object>();

            object func;
            if (!_writersByEnumType.TryGetValue(type, out func))
            {
                var actualType = Nullable.GetUnderlyingType(type);

                MethodInfo builder;
                if (actualType != null)
                    builder = NullableEnumWriterBuilderMethod.MakeGenericMethod(actualType);
                else
                    builder = EnumWriterBuilderMethod.MakeGenericMethod(type);

                func = builder.Invoke(null, null);
            }
            return func;
        }

// Spurious warnings, we reference them in static constructor.
// ReSharper disable UnusedMember.Local
        static Func<string, TEnum> EnumReaderBuilder<TEnum>()
        {
            var parserMethodInfo = ParseEnumMethod.MakeGenericMethod(typeof(TEnum));
            return (Func<string, TEnum>)Delegate.CreateDelegate(typeof(Func<string, TEnum>), parserMethodInfo);
        }

        static Func<string, TEnum?> NullableEnumReaderBuilder<TEnum>()
            where TEnum : struct
        {
            var parserMethodInfo = ParseNullableEnumMethod.MakeGenericMethod(typeof(TEnum));
            return (Func<string, TEnum?>)Delegate.CreateDelegate(typeof(Func<string, TEnum?>), parserMethodInfo);
        }

        static Func<TEnum, string> EnumWriterBuilder<TEnum>()
        {
            Func<TEnum, string> func = e => e.ToString();
            return func;
        }

        static Func<TEnum?, string> NullableEnumWriterBuilder<TEnum>()
            where TEnum : struct
        {
            Func<TEnum?, string> func = e => e.HasValue ? e.Value.ToString() : null;
            return func;
        }

        static TEnum ParseEnum<TEnum>(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        static TEnum? ParseNullableEnum<TEnum>(string value)
            where TEnum : struct
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return (TEnum?)Enum.Parse(typeof(TEnum), value, true);
        }
// ReSharper restore UnusedMember.Local

        static Func<TParam?, string> CreateNullableWriter<TParam>(Func<TParam, string> target)
            where TParam : struct
        {
            return x => x.HasValue ? target(x.Value) : null;
        }

        static Func<string, TParam?> CreateNullableReader<TParam>(Func<string, TParam> orig)
            where TParam : struct
        {
            return x => x != null ? orig(x) : default(TParam?);
        }
    }
}
