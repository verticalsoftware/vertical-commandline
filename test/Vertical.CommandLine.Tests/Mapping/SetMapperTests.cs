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
    public class SetMapperTests
    {
        private class MyOptions
        {
            public ISet<int> Set { get; } = new HashSet<int>();
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            SetMapper<MyOptions, int>.Create(opt => opt.Set).ShouldNotBeNull();
        }

        [Fact]
        public void CreateReturnsMultivaluedMapper()
        {
            SetMapper<MyOptions, int>.Create(opt => opt.Set).MultiValued.ShouldBeTrue();
        }

        [Fact]
        public void MapValueAdds()
        {
            var instance = SetMapper<MyOptions, int>.Create(opt => opt.Set);
            var options = new MyOptions();
            
            instance.MapValue(options, 10);
            options.Set.Single().ShouldBe(10);
        }
    }
}