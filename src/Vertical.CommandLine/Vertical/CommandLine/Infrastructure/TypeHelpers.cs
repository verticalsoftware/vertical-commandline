// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;
using System.Reflection;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Reflection type helpers.
    /// </summary>
    internal static class TypeHelpers
    {
        /// <summary>
        /// Defines the string IsNullOrWhiteSpace method
        /// </summary>
        internal static MethodInfo StringIsNullOrWhiteSpaceMethodInfo { get; } =
            typeof(string).GetKnownMethodInfo(nameof(string.IsNullOrWhiteSpace),
                new[] {typeof(string)},
                typeof(bool));
        
        /// <summary>
        /// Determines if a type is nullable.
        /// </summary>
        internal static bool IsNullableType(this Type type) => type.IsGenericType &&
                                                          type.GetGenericTypeDefinition() == typeof(Nullable<>);

        /// <summary>
        /// Determines if a type is in the System namespace
        /// </summary>
        internal static bool IsSystemType(this Type type) => ReferenceEquals(type.Assembly, typeof(string).Assembly);

        /// <summary>
        /// Gets the underlying type of a Nullable{T}
        /// </summary>
        internal static Type GetNullableUnderlyingType(this Type type) => type.IsNullableType() 
            ? type.GetGenericArguments().Single() 
            : null;

        /// <summary>
        /// Defines the Enum.Parse method.
        /// </summary>
        internal static MethodInfo GetEnumParseMethodInfo() => GetKnownMethodInfo(
            typeof(Enum), nameof(Enum.Parse), new[] {typeof(Type), typeof(string), typeof(bool)}, typeof(object));

        /// <summary>
        /// Gets the Parse method for a type.
        /// </summary>
        internal static bool TryGetParseMethodInfo(Type type, out MethodInfo methodInfo)
        {
            methodInfo = GetMethodInfo(type, nameof(int.Parse), new[] {typeof(string)}, type, null);
            return methodInfo != null;
        }

        /// <summary>
        /// Gets a constructor that accepts a string parameter.
        /// </summary>
        internal static bool TryGetStringConstructor(Type type, out ConstructorInfo constructorInfo)
        {
            constructorInfo = type.GetConstructor(new[] {typeof(string)});
            return constructorInfo != null;
        }
        
        /// <summary>
        /// Gets a known API method
        /// </summary>
        internal static MethodInfo GetKnownMethodInfo(this Type type, string methodName, 
            Type[] parameterTypes,
            Type returnType)
        {
            return GetMethodInfo(type, methodName, parameterTypes, returnType, () =>
                throw new MissingMethodException());
        }

        // Gets method info matching parameters and return type
        private static MethodInfo GetMethodInfo(Type type, string methodName, 
            Type[] parameterTypes,
            Type returnType,
            Action error)
        {
            var methodInfo = type.GetMethod(methodName, parameterTypes);

            if (methodInfo == null || methodInfo.ReturnType != returnType)
                error?.Invoke();

            return methodInfo;
        }
        
        /// <summary>
        /// Gets a generic type name
        /// </summary>
        internal static string GetGenericTypeName(string typeName)
        {
            var markerIndex = typeName.IndexOf('`');
            return markerIndex > -1 ? typeName.Substring(0, markerIndex) : typeName;
        }
    }
}