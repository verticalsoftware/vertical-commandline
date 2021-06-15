// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using System;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using Xunit;

namespace Vertical.CommandLine.Tests.Parsing
{
    public class TemplateSetTests
    {
        private readonly TemplateSet _instanceUnderTest = new TemplateSet();

        [Fact]
        public void AddThrowsForDuplicateToken()
        {
            var template = Template.ForOptionOrSwitch("-t");

            _instanceUnderTest.Add(template);
            Should.Throw<ConfigurationException>(() => _instanceUnderTest.Add(template));
        }

        [Fact]
        public void AddRegistersAllTokens()
        {
            _instanceUnderTest.Add(Template.ForOptionOrSwitch("-t | --test"));
            _instanceUnderTest.Count.ShouldBe(2);
        }

        [Fact]
        public void AddThrowsForNullTemplate()
        {
            Should.Throw<ArgumentNullException>(() => _instanceUnderTest.Add(null));
        }
    }
}
