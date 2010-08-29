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

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace ObjectGraph.Xml
{
    /// <summary>
    /// The serializer class is the entry point into serialization for a particular type.
    /// </summary>
    /// <typeparam name="T">The type to perform serialization of.</typeparam>
    public static class Serializer<T>
        where T : new()
    {
        #region Fields
        static XmlReaderSettings _readerSettings;
        static XmlWriterSettings _writerSettings;
        static TypeSerializer<T> _serializer;
        #endregion

        static Serializer()
        {
            _readerSettings = new XmlReaderSettings { IgnoreComments = true, IgnoreProcessingInstructions = true, IgnoreWhitespace = true };
            _writerSettings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false), OmitXmlDeclaration = true };
            _serializer = TypeSerializer.Build<T>();
        }

        public static T Deserialize(Stream stream)
        {
            using (var reader = XmlReader.Create(stream, _readerSettings))
            {
                Debug.Assert(reader != null);

                while (reader.Read() && reader.NodeType != XmlNodeType.Element)
                    ;

                return _serializer.ReadObject(reader);
            }
        }

        public static void Serialize(Stream stream, T obj)
        {
            using (var writer = XmlWriter.Create(stream, _writerSettings))
            {
                Debug.Assert(writer != null);
                _serializer.WriteObject(writer, obj);
            }
        }
    }
}
