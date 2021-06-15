// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Vertical.CommandLine.Mapping;
using Xunit;

namespace Vertical.CommandLine.Tests.Mapping
{
    public class StackMapperTests
    {
        private class MyOptions
        {
            public Stack<int> Stack { get; } = new Stack<int>();
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            StackMapper<MyOptions, int>.Create(opt => opt.Stack).ShouldNotBeNull();
        }

        [Fact]
        public void CreateReturnsMultivaluedMapper()
        {
            StackMapper<MyOptions, int>.Create(opt => opt.Stack).MultiValued.ShouldBeTrue();
        }

        [Fact]
        public void MapValueAdds()
        {
            var instance = StackMapper<MyOptions, int>.Create(opt => opt.Stack);
            var options = new MyOptions();
            
            instance.MapValue(options, 10);
            options.Stack.Single().ShouldBe(10);
        }
    }
}