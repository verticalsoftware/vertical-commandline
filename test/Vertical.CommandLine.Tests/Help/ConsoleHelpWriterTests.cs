// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.IO;
using Shouldly;
using Vertical.CommandLine.Help;
using Xunit;

namespace Vertical.CommandLine.Tests.Help
{
    public class ConsoleHelpWriterTests
    {
        [Fact]
        public void WriteContentWritesToConsoleOut()
        {
            var textWriter = Console.Out;

            try
            {
                using (var writer = new StringWriter())
                { 
                    Console.SetOut(writer);
                    ConsoleHelpWriter.Default.WriteContent(new []{"test"});
                    writer.ToString().ShouldBe($"test{Environment.NewLine}");
                }
            }
            finally
            {
                Console.SetOut(textWriter);
            }
        }
    }
}