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
    public class QueueMapperTests
    {
        private class MyOptions
        {
            public Queue<int> Queue { get; } = new Queue<int>();
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            QueueMapper<MyOptions, int>.Create(opt => opt.Queue).ShouldNotBeNull();
        }

        [Fact]
        public void CreateReturnsMultivaluedMapper()
        {
            QueueMapper<MyOptions, int>.Create(opt => opt.Queue).MultiValued.ShouldBeTrue();
        }

        [Fact]
        public void MapValueEnqueues()
        {
            var instance = QueueMapper<MyOptions, int>.Create(opt => opt.Queue);
            var options = new MyOptions();
            
            instance.MapValue(options, 10);
            options.Queue.Single().ShouldBe(10);
        }
    }
}