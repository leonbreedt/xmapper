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
using System.Xml.Linq;

namespace ObjectGraph.Xml
{
    internal abstract class SerializableProperty
    {
        protected SerializableProperty(XName name, NodeType nodeType, bool isRequired, Type propertyType)
        {
            Name = name;
            NodeType = nodeType;
            IsRequired = isRequired;
            UnderlyingType = Nullable.GetUnderlyingType(propertyType);
            IsNullable = UnderlyingType != null;
            PropertyType = propertyType;
        }

        public XName Name { get; set; }
        public NodeType NodeType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsNullable { get; set; }
        public Type UnderlyingType { get; set; }
        public Type PropertyType { get; set; }

        public abstract string GetValueForXml(object obj);
        public abstract void SetValueFromXml(object obj, string value);
    }

    internal class SerializableProperty<T, TProperty> : SerializableProperty
        where T : class
    {
        public SerializableProperty(XName name,
                                    NodeType nodeType,
                                    bool isRequired,
                                    Type propertyType,
                                    Func<T, TProperty> getter,
                                    Action<T, TProperty> setter,
                                    Func<TProperty, string> toXmlValue,
                                    Func<string, TProperty> fromXmlValue)
            : base(name, nodeType, isRequired, propertyType)
        {
            Getter = getter;
            Setter = setter;
            ToXmlValue = toXmlValue;
            FromXmlValue = fromXmlValue;
        }

        public Func<T, TProperty> Getter { get; set; }
        public Action<T, TProperty> Setter { get; set; }
        public Func<TProperty, string> ToXmlValue { get; set; }
        public Func<string, TProperty> FromXmlValue { get; set; }

        public override string GetValueForXml(object obj)
        {
            return ToXmlValue(Getter((T)obj));
        }

        public override void SetValueFromXml(object obj, string value)
        {
            Setter((T)obj, FromXmlValue(value));
        }
    }
}
