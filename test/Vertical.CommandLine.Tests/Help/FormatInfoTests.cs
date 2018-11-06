// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Help;
using Xunit;

namespace Vertical.CommandLine.Tests.Help
{
    public class FormatInfoTests
    {
        [Fact]
        public void ConstructWithBelowMinimumWidthThrows()
        {
            Should.Throw<ConfigurationException>(() => new FormatInfo(
                FormatInfo.MinimumMarginWidth - 1,
                FormatInfo.MinimumMarginHeight,
                0));
        }

        [Fact]
        public void ConstructWithBelowMinimumHeightThrows()
        {
            Should.Throw<ConfigurationException>(() => new FormatInfo(
                FormatInfo.MinimumMarginWidth,
                FormatInfo.MinimumMarginHeight - 1,
                0));
        }

        [Fact]
        public void ConstructWithNullFormatterThrows()
        {
            Should.Throw<ConfigurationException>(() => new FormatInfo(
                FormatInfo.MinimumMarginWidth,
                FormatInfo.MinimumMarginHeight,
                0,
                null));
        }
    }
}