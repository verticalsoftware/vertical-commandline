// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using System;
using System.Collections.Generic;

namespace Vertical.CommandLine.Tests
{
    internal static class Macros
    {
        internal static IEnumerable<object[]> Scenarios(params object[][] scenario) => scenario;

        internal static object[] Scenario(params object[] parameter) => parameter;

        internal static void ShouldThrowIf<T>(bool throws, Action actual) where T:Exception
        {
            if (throws)
            {
                Should.Throw<T>(actual);
            }
            else
            {
                Should.NotThrow(actual);
            }
        }
    }
}