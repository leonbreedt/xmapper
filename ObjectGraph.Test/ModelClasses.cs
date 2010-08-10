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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ObjectGraph.Extensions;
using ObjectGraph.Xml;
using ProtoBuf;

namespace ObjectGraph.Test
{
    internal enum PersonRole
    {
        Undefined,
        Sales,
        Manager
    }

    [DataContract]
    [ProtoContract]
    [ProtoInclude(1000, typeof(SalesAgent))]
    [ProtoInclude(1001, typeof(Manager))]
    [XSerializable]
    internal abstract class Person
    {
        [DataMember(Order = 1)]
        [XRequired]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        [XOptional]
        public string FirstName { get; set; }

        [DataMember(Order = 3)]
        [XOptional]
        public string LastName { get; set; }

        [DataMember(Order = 4)]
        [XRequired]
        public PersonRole Role { get; set; }

        [OnDeserialized]
        internal void OnDeserializing(StreamingContext context)
        {
            context.IndexObject(Id, this);
        }
    }

    [DataContract]
    [XSerializable]
    internal class Manager : Person
    {
        [DataMember(Order = 1)]
        [XRequired]
        public int CarParkNumber { get; set; }

        [DataMember(Order = 2)]
        [XRequired]
        public Person Superior { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Manager)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }

    [DataContract]
    [XSerializable]
    internal class SalesAgent : Person
    {
        [DataMember(Order = 1)]
        [XRequired]
        public decimal SalesTotal { get; set; }
    }

    [DataContract]
    [XSerializable]
    internal class Document
    {
        [DataMember(Order = 1)]
        [XRequired]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        [XOptional]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        [XOptional]
        public List<Manager> Managers { get; set; }
    }
}
