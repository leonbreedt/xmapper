﻿//
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
using System.Runtime.Serialization;
using ObjectGraph.Xml;
using ProtoBuf;

namespace ObjectGraph
{
    public static class Serialization
    {
        public static T Load<T>(Stream stream)
            where T : new()
        {
            return Load<T>(stream, SerializationFormat.ProtocolBuffer, null);
        }

        public static T Load<T>(Stream stream, SerializationFormat format)
            where T : new()
        {
            return Load<T>(stream, format, null);
        }

        public static T Load<T>(Stream stream, SerializationFormat format, IObjectIndex index)
            where T : new()
        {
            T result;

            if (format == SerializationFormat.ProtocolBuffer)
            {
                var context = new StreamingContext(StreamingContextStates.Other, index);

                var formatter = Serializer.CreateFormatter<T>();
                formatter.Context = context;

                result = (T)formatter.Deserialize(stream);
            }
            else if (format == SerializationFormat.Xml)
                result = Serializer<T>.Deserialize(stream);
            else
                throw new NotSupportedException();

            return result;
        }

        public static void Save<T>(T obj, Stream stream, SerializationFormat format)
            where T : new()
        {
            if (format == SerializationFormat.ProtocolBuffer)
                Serializer.Serialize(stream, obj);
            else if (format == SerializationFormat.Xml)
                Serializer<T>.Serialize(stream, obj);
            else
                throw new NotSupportedException();
        }
    }
}
