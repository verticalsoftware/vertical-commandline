// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Mapping;
using Xunit;

namespace Vertical.CommandLine.Tests.Mapping
{
    public class DelegateMapperTests
    {
        private class MyOptions {}
        
        [Fact]
        public void ConstructWithNullDelegateThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DelegateMapper<MyOptions, string>(null, false));
        }

        [Fact]
        public void ConstructWithMultiValuedSetsProp()
        {
            new DelegateMapper<MyOptions, string>((opt, value) => { }, true).MultiValued.ShouldBeTrue();
            new DelegateMapper<MyOptions, string>((opt, value) => { }, false).MultiValued.ShouldBeFalse();
        }

        [Fact]
        public void MapValueCallsDelegateWithArg()
        {
            var options = new MyOptions();
            var value = string.Empty;
            var mapper = new DelegateMapper<MyOptions, string>((opt, val) =>
            {
                value = val;
                opt.ShouldBeSameAs(options);
            }, false);
            
            mapper.MapValue(options, "test");
            value.ShouldBe("test");
        }

        [Fact]
        public void MapValueWithNullReferenceThrows()
        {
            var mapper = new DelegateMapper<MyOptions, string>((opt, value) => 
                throw new NullReferenceException(), false);
            Should.Throw<ConfigurationException>(() => mapper.MapValue(new MyOptions(), string.Empty));
        }

        [Fact]
        public void MapValueWithFaultyDelegateCodeThrows()
        {
            var mapper = new DelegateMapper<MyOptions, string>((opt, value) =>
                throw new ArgumentException(), false);
            Should.Throw<ConfigurationException>(() => mapper.MapValue(new MyOptions(), string.Empty));
        }
    }
}