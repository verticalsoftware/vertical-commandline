// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Help;
using Xunit;

namespace Vertical.CommandLine.Tests.Help
{
    public class FormatterTests
    {
        [Fact]
        public void JustifiedFormatterReturnsJustified()
        {
            Formatter.JustifiedFormatter.CreateFormatted("test").ShouldBeOfType<JustifiedString>();
        }

        [Fact]
        public void DefaultFormatterReturnsUnformatted()
        {
            Formatter.DefaultFormatter.CreateFormatted("test").ShouldBeOfType<UnformattedString>();
        }
    }
}