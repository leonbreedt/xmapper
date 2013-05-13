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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using XMapper.Util;

namespace XMapper
{
    /// <summary>
    /// Base class for mappings representing custom XML objects, like custom attributes
    /// or custom elements.
    /// </summary>
    /// <typeparam name="TContainer">The containing type.</typeparam>
    /// <typeparam name="TXMLNode">The type of the XML node.</typeparam>
    public abstract class AnyMappingBase<TContainer, TXMLNode> : MappingBase
        where TXMLNode : XObject
    {
        #region Fields
        readonly Func<IList<TXMLNode>> _listConstructor;
        readonly Func<TContainer, IList<TXMLNode>> _listGetter;
        readonly Action<TContainer, IList<TXMLNode>> _listSetter;
        readonly PropertyInfo _propertyInfo;
        #endregion

        protected AnyMappingBase(Expression<Func<TContainer, IList<TXMLNode>>> propertyExpression)
            : base(typeof(TContainer), null)
        {
            if (propertyExpression != null)
            {
                _propertyInfo = ReflectionHelper.GetPropertyInfoFromExpression(propertyExpression);
                _listConstructor = ReflectionHelper.GetCollectionConstructorDelegate<TXMLNode>(_propertyInfo.PropertyType);
                _listGetter = ReflectionHelper.GetCollectionPropertyGetterDelegate<TContainer, TXMLNode>(_propertyInfo);
                _listSetter = ReflectionHelper.GetCollectionPropertySetterDelegate<TContainer, TXMLNode>(_propertyInfo);
            }
        }

        public override IAttributeMapping[] Attributes
        {
            get { return NoAttributes; }
            internal set { }
        }

        public override IChildElementMapping[] ChildElements
        {
            get { return NoChildElements; }
            internal set { }
        }

        public override ITextContentMapping TextContent
        {
            get { return null; }
            internal set { }
        }

        public override ITextContentMapping[] ChildTextElements
        {
            get { return NoChildTextElements; }
            internal set { }
        }

        protected IList GetCustomNodeList(object target)
        {
            if (_propertyInfo == null)
                return (IList)target;
            return (IList)_listGetter((TContainer)target);            
        }

        protected void AddToCustomNodeList(object target, TXMLNode node)
        {
            IList collection = null;
            IList<TXMLNode> typedCollection;

            bool isContainerTheCollection = _propertyInfo == null;
            if (isContainerTheCollection)
            {
                typedCollection = target as IList<TXMLNode>;
                if (typedCollection == null)
                    collection = target as IList;
            }
            else
            {
                typedCollection = _listGetter((TContainer)target);
            }

            if (collection == null && typedCollection == null)
            {
                if (!isContainerTheCollection)
                {
                    typedCollection = _listConstructor();
                    _listSetter((TContainer)target, typedCollection);
                }
                else
                    throw new InvalidOperationException(string.Format("Unable to instantiate a new {0} collection",
                                                                      typeof(TXMLNode)));
            }

            if (collection != null)
                collection.Add(node);
            else if (typedCollection != null)
                typedCollection.Add(node);            
        }
    }
}
