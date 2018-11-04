// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using System.Linq;
using System.Collections.Generic;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class MultiValueArgumentConfigurationTests
    {
        public class MyOptions
        {
            public ICollection<string> StringCollection { get; set; } = new List<string>();
        }

        [Fact]
        public void MapManySinksMapper()
        {
            var builder = new ParserBuilder<MyOptions, string>();
            var options = new MyOptions();

            MultiValueArgumentConfiguration<MyOptions, string>.Configure(builder,
                arg => arg.MapMany.ToCollection(opt => opt.StringCollection));

            builder.PositionArgument(0).ProcessContext(options, new ParseContext(new[] { "test" }));
            options.StringCollection.Single().ShouldBe("test");
        }
    }
}
