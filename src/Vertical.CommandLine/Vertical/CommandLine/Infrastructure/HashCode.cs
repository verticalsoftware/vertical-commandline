// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Utility method used to combine hash codes.
    /// </summary>
    internal static class HashCode
    {
        /// <summary>
        /// Combines two hash codes
        /// </summary>
        /// <param name="h1">First hash</param>
        /// <param name="h2">Second hash</param>
        /// <returns></returns>
        internal static int Combine(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
    }
}