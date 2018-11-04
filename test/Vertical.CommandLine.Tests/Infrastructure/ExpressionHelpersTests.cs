// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Infrastructure;
using Xunit;

namespace Vertical.CommandLine.Tests.Infrastructure
{
    public class ExpressionHelpersTests
    {
        public class MyType
        {
            public string Field;
        }

        [Fact]
        public void GetPropertyInfoWithNonMemberExpressionThrows()
        {
            Should.Throw<ConfigurationException>(() => ExpressionHelpers.GetPropertyInfo<MyType, string>(
                t => "not-a-member", false));
        }

        [Fact]
        public void GetPropertyInfoWithNonPropertyExpressionThrows()
        {
            Should.Throw<ConfigurationException>(() => ExpressionHelpers.GetPropertyInfo<MyType, string>(
                t => t.Field, false));
        }
    }
}