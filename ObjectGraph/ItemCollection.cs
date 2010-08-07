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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace ObjectGraph
{
    public abstract class ItemCollection<TCollection, TItem> : List<TItem>, ICollection
        where TCollection : class
        where TItem : class
    {
        public static TCollection Load(Stream stream, SerializationFormat format)
        {
            if (format == SerializationFormat.ProtocolBuffer)
                return Serializer.DeserializeWithLengthPrefix<TCollection>(stream, PrefixStyle.Fixed32);

            throw new NotImplementedException();
        }

        public void Save(Stream stream, SerializationFormat format)
        {
            if (format == SerializationFormat.ProtocolBuffer)
                Serializer.SerializeWithLengthPrefix(stream, this as TCollection, PrefixStyle.Fixed32);
            else
                throw new NotImplementedException();
        }
    }
}
