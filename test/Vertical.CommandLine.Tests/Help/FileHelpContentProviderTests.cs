// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Xunit;
using Vertical.CommandLine.Help;
using System.Linq;

namespace Vertical.CommandLine.Tests.Help
{
    public class FileHelpContentProviderTests
    {
        [Fact]
        public void GetInstanceReturnsContent()
        {
            var provider = new FileHelpContentProvider("help.txt");
            var content = provider.GetInstance();

            content.First().ShouldBe("NAME");
        }
    }
}
