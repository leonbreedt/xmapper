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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace XMapper.Util
{
    /// <summary>
    /// Helper class for doing reflection related tasks.
    /// </summary>
    internal static class ReflectionHelper
    {
        static readonly MethodInfo GetTypedConstructorDelegateMethodInfo;
        static readonly MethodInfo GetTypedPropertyGetterDelegateMethodInfo;
        static readonly MethodInfo GetTypedPropertySetterDelegateMethodInfo;

        static ReflectionHelper()
        {
            GetTypedConstructorDelegateMethodInfo = typeof(ReflectionHelper).GetMethod("GetTypedConstructorDelegate", BindingFlags.Static | BindingFlags.NonPublic);
            GetTypedPropertyGetterDelegateMethodInfo = typeof(ReflectionHelper).GetMethod("GetTypedPropertyGetterDelegate", BindingFlags.Static | BindingFlags.NonPublic);
            GetTypedPropertySetterDelegateMethodInfo = typeof(ReflectionHelper).GetMethod("GetTypedPropertySetterDelegate", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets strongly typed delegate for quickly creating new instances.
        /// </summary>
        /// <typeparam name="T">The type to get the constructor for.</typeparam>
        /// <returns>Returns the delegate to use to instantiate new instances of <typeparamref name="T"/>.</returns>
        internal static Func<T> GetTypedConstructorDelegate<T>()
        {
            // http://www.smelser.net/blog/post/2010/03/05/When-Activator-is-just-to-slow.aspx
            var type = typeof(T);
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                // If it's a value type with non-empty constructors, fall back to slow creation using
                // Activator.CreateInstance, simplest way to get equivalent of default(T).
                if (type.IsValueType)
                    return () => (T)Activator.CreateInstance(type);
                throw new NotSupportedException(string.Format("Type {0} does not have an empty constructor", type.FullName));
            }
            return Expression.Lambda<Func<T>>(Expression.New(constructorInfo)).Compile();
        }

        /// <summary>
        /// Gets a delegate for quickly creating new instances for objects of the specified type.
        /// </summary>
        /// <param name="type">The type to get a constructor delegate for, must implement IList with member type <typeparamref name="TMember" />.
        /// <returns></returns>
        internal static Func<IList<TMember>> GetCollectionConstructorDelegate<TMember>(Type type)
        {
            if (!ImplementsList(type, typeof(TMember)))
                throw new ArgumentException(string.Format("Type {0} does not implement IList<{1}>",
                                                          type.FullName,
                                                          typeof(TMember).FullName));


            var builder = GetTypedConstructorDelegateMethodInfo.MakeGenericMethod(type);
            var func = builder.Invoke(null, null);

            Func<IList<TMember>> constructor = () => (IList<TMember>)((Delegate)func).DynamicInvoke();
            return constructor;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> associated with a property member expression.
        /// </summary>
        /// <typeparam name="TContainer">The type containing the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expr">The property reference expression.</param>
        /// <returns>Returns the <see cref="PropertyInfo"/>.</returns>
        internal static PropertyInfo GetPropertyInfoFromExpression<TContainer, TProperty>(Expression<Func<TContainer, TProperty>> expr)
        {
            var memberExpr = expr.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("Expression must be a System.Linq.Expressions.MemberExpression");

            var info = memberExpr.Member as PropertyInfo;
            if (info == null)
                throw new ArgumentException("Expression member must be a property reference");

            return info;
        }

        /// <summary>
        /// Gets a strongly typed delegate for getting the value of a property.
        /// </summary>
        /// <typeparam name="TContainer">The type containing the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="info">The <see cref="PropertyInfo"/> of the property to get the delegate for.</param>
        /// <returns>Returns a delegate for getting the property value from an object instance.</returns>
        internal static Func<TContainer, TProperty> GetTypedPropertyGetterDelegate<TContainer, TProperty>(PropertyInfo info)
        {
            var getMethod = info.GetGetMethod(true);
            if (getMethod == null)
                throw new NotSupportedException(string.Format("Property {0} of type {1} does not have a getter", info.Name, typeof(TContainer).FullName));

            return (Func<TContainer, TProperty>)Delegate.CreateDelegate(typeof(Func<TContainer, TProperty>), getMethod);
        }

        /// <summary>
        /// Gets a delegate for getting the value of a property, when the type of the property isn't known at compile time.
        /// </summary>
        /// <param name="info">The property info of the property.</param>
        /// <returns></returns>
        internal static Func<TContainer, IList<TMember>> GetCollectionPropertyGetterDelegate<TContainer, TMember>(PropertyInfo info)
        {
            var builder = GetTypedPropertyGetterDelegateMethodInfo.MakeGenericMethod(typeof(TContainer), info.PropertyType);
            var func = builder.Invoke(null, new object[] {info});

            Func<TContainer, IList<TMember>> constructor = val => (IList<TMember>)((Delegate)func).DynamicInvoke(val);

            return constructor;
        }

        /// <summary>
        /// Gets a strongly typed delegate for setting the value of a property.
        /// </summary>
        /// <typeparam name="TContainer">The type containing the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="info">The <see cref="PropertyInfo"/> of the property to get the delegate for.</param>
        /// <returns>Returns a delegate for setting the property value on an object instance.</returns>
        internal static Action<TContainer, TProperty> GetTypedPropertySetterDelegate<TContainer, TProperty>(PropertyInfo info)
        {
            var setMethod = info.GetSetMethod(true);
            if (setMethod == null)
                throw new NotSupportedException(string.Format("Property {0} of type {1} does not have a setter", info.Name, typeof(TContainer).FullName));

            return (Action<TContainer, TProperty>)Delegate.CreateDelegate(typeof(Action<TContainer, TProperty>), setMethod);
        }

        /// <summary>
        /// Gets a delegate for getting the value of a property, when the type of the property isn't known at compile time.
        /// </summary>
        /// <param name="info">The property info of the property.</param>
        /// <returns></returns>
        internal static Action<TContainer, IList<TMember>> GetCollectionPropertySetterDelegate<TContainer, TMember>(PropertyInfo info)
        {
            // http://stackoverflow.com/questions/4085798/creating-an-performant-open-delegate-for-an-property-setter-or-getter
            MethodInfo setMethod = info.GetSetMethod();
            if (setMethod != null && setMethod.GetParameters().Length == 1)
            {
                var target = Expression.Parameter(typeof(TContainer), null);
                var value = Expression.Parameter(typeof(IList<TMember>), null);
                var body = Expression.Call(target, setMethod, Expression.Convert(value, info.PropertyType));

                return Expression.Lambda<Action<TContainer, IList<TMember>>>(body, target, value).Compile();
            }
            throw new NotSupportedException(string.Format("Property {0} on type {1} does not have a supported setter", info.Name, typeof(TContainer).FullName));
        }

        /// <summary>
        /// Gets a strongly typed delegate for converting supported simple .NET types (the basic value types) from
        /// their XML representations into CLR equivalents.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns>Returns a delegate for converting the XML string value, or <c>null</c> if the specified type is not supported.</returns>
        internal static Func<string, TProperty> GetXmlSimpleTypeReaderDelegate<TProperty>()
        {
            var type = typeof(TProperty);

            var actualType = Nullable.GetUnderlyingType(type);
            if (actualType == null)
                actualType = type;

            if (actualType.IsEnum)
                return (Func<string, TProperty>)XmlConversionDelegate.ForReadingEnum(type);
            return (Func<string, TProperty>)XmlConversionDelegate.ForReading(type);
        }

        /// <summary>
        /// Gets a strongly typed delegate for converting supported simple .NET types (the basic value types) to
        /// the correct XML representations.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns>Returns a delegate for converting the simple type into an XML string value, or <c>null</c> if the specified type is not supported.</returns>
        internal static Func<TProperty, string> GetXmlSimpleTypeWriterDelegate<TProperty>()
        {
            var type = typeof(TProperty);

            var actualType = Nullable.GetUnderlyingType(type);
            if (actualType == null)
                actualType = type;

            if (actualType.IsEnum)
                return (Func<TProperty, string>)XmlConversionDelegate.ForWritingEnum(type);
            return (Func<TProperty, string>)XmlConversionDelegate.ForWriting(type);
        }

        /// <summary>
        /// Checks whether the specified type implements IList with the specified element type.
        /// </summary>
        /// <param name="containerType">The type to check.</param>
        /// <param name="elementType">The type argument to IList&lt;&gt; to check.</param>
        /// <returns>Returns <c>true</c> if <paramref name="containerType"/> is an IList with generic argument of <paramref name="elementType"/>.</returns>
        internal static bool ImplementsList(Type containerType, Type elementType)
        {
            foreach (Type iface in containerType.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IList<>) && iface.GetGenericArguments()[0] == elementType)
                    return true;
            }
            return false;
        }
    }
}
