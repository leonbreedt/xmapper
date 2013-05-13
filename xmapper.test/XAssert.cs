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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XMapper.Test
{
    public static class XAssert
    {
        public static void AreEqual(string expected, string actual)
        {
            AreEqual(XDocument.Parse(expected, LoadOptions.SetLineInfo),
                     XDocument.Parse(expected, LoadOptions.SetLineInfo));
        }

        public static void AreEqual(XObject expected, XObject actual)
        {
            if (expected is XDocument)
            {
                expected = ((XDocument)expected).Root;
            }
            if (actual is XDocument)
            {
                actual = ((XDocument)actual).Root;
            }

            if (expected == null && actual == null)
            {
                return;
            }
            if (expected == null)
            {
                RaiseAssertFailure(null, actual);
                return;
            }
            if (actual == null)
            {
                RaiseAssertFailure(expected, null);
                return;
            }
            if (expected.GetType() != actual.GetType())
            {
                RaiseAssertFailure(expected, actual);
                return;
            }

            AssertEqualNames(expected, actual);
            AssertEqualValues(expected, actual);
        }

        static void AssertEqualValues(XObject expected, XObject actual)
        {
            if (expected is XAttribute)
                AssertEqualAttributeValues((XAttribute)expected, (XAttribute)actual);
            else if (expected is XElement)
                AssertEqualElementValues(actual, (XElement)expected, (XElement)actual);
        }

        static void AssertEqualElementValues(XObject actual, XElement expectedElement, XElement actualElement)
        {
            var expectedAttributesByName = expectedElement.Attributes().ToDictionary(a => a.Name);
            var actualAttributesByName = actualElement.Attributes().ToDictionary(a => a.Name);

            foreach (var expectedAttribute in expectedElement.Attributes())
            {
                // Ignore xmlns: declarations
                if (expectedAttribute.Name.Namespace == XNamespace.Xmlns)
                    continue;

                XAttribute actualAttribute;
                if (!actualAttributesByName.TryGetValue(expectedAttribute.Name, out actualAttribute))
                {
                    // It may be an attribute with a different prefix, but still in the correct namespace,
                    // in which case let it pass.
                    bool raiseError = !actualAttributesByName.Any(
                        a => a.Key.NamespaceName.Equals(expectedAttribute.Name.NamespaceName) &&
                             a.Key.LocalName.Equals(expectedAttribute.Name.LocalName));

                    if (raiseError)
                    {
                        RaiseAssertFailure(string.Format("Attribute {0} is missing from {1}", expectedAttribute.Name,
                                                         GetXDescription(actual)));
                    }
                }
                AssertEqualAttributeValues(expectedAttribute, actualAttribute);
            }

            foreach (var actualAttribute in actualElement.Attributes())
            {
                // Ignore xmlns: declarations
                if (actualAttribute.Name.Namespace == XNamespace.Xmlns)
                    continue;

                XAttribute expectedAttribute;
                if (!expectedAttributesByName.TryGetValue(actualAttribute.Name, out expectedAttribute))
                    RaiseAssertFailure(string.Format("Unexpected attribute {0} found on {1}", actualAttribute.Name,
                                                     GetXDescription(actual)));
            }

            var expectedChildElements = expectedElement.Elements().ToArray();
            var actualChildElements = actualElement.Elements().ToArray();

            if (expectedChildElements.Length != actualChildElements.Length)
                RaiseAssertFailure(
                    string.Format("{0} has {1} child elements", GetXDescription(actual), expectedChildElements.Length),
                    string.Format("{0} has {1} child elements", GetXDescription(actual), actualChildElements.Length));

            for (int i = 0; i < expectedChildElements.Length; i++)
                AssertEqualElementValues(actual, expectedChildElements[i], actualChildElements[i]);

            if (!expectedElement.Value.Equals(actualElement.Value))
            {
                RaiseAssertFailure(
                    string.Format("{0} has text of {1}", GetXDescription(actual), expectedElement.Value),
                    string.Format("{0} has value of {1}", GetXDescription(actual), actualElement.Value));
            }
        }

        static void AssertEqualAttributeValues(XAttribute expected, XAttribute actual)
        {
            if (!expected.Value.Equals(actual.Value))
            {
                RaiseAssertFailure(
                    string.Format("{0} has value {1}", GetXDescription(actual), expected.Value),
                    string.Format("{0} has value {1}", GetXDescription(actual), actual.Value));
            }
        }

        static void RaiseAssertFailure(object expected, object actual)
        {
            if (expected == null)
                expected = "(null)";
            if (actual == null)
                actual = "(null)";

            string message = string.Format("Expected: {0}{1}Actual: {2}{1}",
                                           expected, 
                                           Environment.NewLine,
                                           actual);

            RaiseAssertFailure(message);
        }

        static void RaiseAssertFailure(string message)
        {
            throw new AssertFailedException(message);
        }

        static void AssertEqualNames(XObject expected, XObject actual)
        {
            var expectedName = GetXName(expected);
            var actualName = GetXName(actual);

            if (!expectedName.NamespaceName.Equals(actualName.NamespaceName))
            {
                RaiseAssertFailure(
                    string.Format("{0} has namespace {1}", GetXDescription(actual), expectedName.NamespaceName),
                    string.Format("{0} has namespace {1}", GetXDescription(actual), actualName.NamespaceName));
            }
            if (!expectedName.LocalName.Equals(actualName.LocalName))
            {
                RaiseAssertFailure(
                    string.Format("{0} has local name {1}", GetXDescription(actual), expectedName.LocalName),
                    string.Format("{0} has local name {1}", GetXDescription(actual), actualName.LocalName));
            }
        }

        static XName GetXName(XObject obj)
        {
            var attribute = obj as XAttribute;
            if (attribute != null)
                return attribute.Name;
            var element = obj as XElement;
            if (element != null)
                return element.Name;
            return null;
        }

        static string GetXDescription(XObject obj)
        {
            var lineInfo = obj as IXmlLineInfo;
            string location = "";
            if (lineInfo != null && lineInfo.LineNumber > 0)
                location = string.Format(" at position ({0},{1}) ", lineInfo.LineNumber, lineInfo.LinePosition);

            var attribute = obj as XAttribute;
            if (attribute != null)
            {
                if (attribute.Parent != null)
                    return string.Format("Attribute {0}{2} on element {1}", attribute.Name.LocalName, attribute.Parent.Name, lineInfo);
                return string.Format("Attribute {0}{1}", attribute.Name.LocalName, lineInfo);
            }
            var element = obj as XElement;
            if (element != null)
            {
                if (element.Parent != null)
                    return string.Format("Child element {0}{2} with parent element {1}", element.Name.LocalName, element.Parent.Name, lineInfo);
                return string.Format("Root element {0}{1}", element.Name.LocalName, lineInfo);
            }
            return null;
        }
    }
}
