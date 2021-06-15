// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Defines formatting methods.
    /// </summary>
    internal static class Formatting
    {
        internal static string Quote(object? obj) => obj != null ? $"\"{obj}\"" : string.Empty;
        
        /// <summary>
        /// Used for debugging mostly in ToString methods.
        /// </summary>
        internal static string FriendlyName(Type type)
        {
            var simplifiedName = type.IsSystemType() ? type.Name : type.FullName;
            
            switch (type)
            {
                case { } when type.IsNullableType():
                    return $"{FriendlyName(type.GetGenericArguments()[0])}?";
                
                case {IsGenericType: true} t:
                    var genericTypes = string.Join(", ", t.GenericTypeArguments.Select(FriendlyName));
                    return $"{TypeHelpers.GetGenericTypeName(simplifiedName!)}<{genericTypes}>";

                case {DeclaringType: { }}:
                    return simplifiedName!.Replace('+', '.');

                default:
                    return simplifiedName!;
            }
        }
    }
}