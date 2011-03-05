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
using System.Runtime.Serialization;
using System.Xml;

namespace ObjectGraph.Xml
{
    [Serializable]
    public class XmlFormatException : FormatException
    {
        public XmlFormatException() { }

        public XmlFormatException(string message)
            : base(message) { }

        public XmlFormatException(string message, Exception inner)
            : base(message, inner) { }

        public XmlFormatException(string message, IXmlLineInfo lineInfo)
            : base(BuildMessageWithLineNumbers(lineInfo, message)) { }

        protected XmlFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        static string BuildMessageWithLineNumbers(IXmlLineInfo lineInfo, string message)
        {
            if (lineInfo == null || (lineInfo.LineNumber == 0 || lineInfo.LinePosition == 0))
                return message;
            return string.Format("({0},{1}) {2}", lineInfo.LineNumber, lineInfo.LinePosition, message);
        }
    }
}
