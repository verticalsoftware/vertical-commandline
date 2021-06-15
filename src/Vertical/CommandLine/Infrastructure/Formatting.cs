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
        internal static string Quote(object obj) => obj != null ? $"\"{obj}\"" : string.Empty;
        
        /// <summary>
        /// Used for debugging mostly in ToString methods.
        /// </summary>
        internal static string FriendlyName(Type type)
        {
            var simplifiedName = type.IsSystemType() ? type.Name : type.FullName;
            
            switch (type)
            {
                case Type t when type.IsNullableType():
                    return $"{FriendlyName(type.GetGenericArguments()[0])}?";
                
                case Type t when t.IsGenericType:
                    var genericTypes = string.Join(", ", t.GenericTypeArguments.Select(FriendlyName));
                    return $"{TypeHelpers.GetGenericTypeName(simplifiedName)}<{genericTypes}>";

                case Type t when t.DeclaringType != null:
                    return simplifiedName.Replace('+', '.');

                default:
                    return simplifiedName;
            }
        }
    }
}