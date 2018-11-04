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
    public class PropertyMapperTests
    {
        private class MyOptions
        {
            public string Value { get; set; }
            public string ValueReadOnly { get; }

            public string Throws
            {
                get => string.Empty;
                set => throw new NotSupportedException();
            }
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            PropertyMapper<MyOptions, string>.Create(opt => opt.Value).ShouldNotBeNull();
        }

        [Fact]
        public void CreateForReadOnlyPropertyThrows()
        {
            Should.Throw<ConfigurationException>(() => PropertyMapper<MyOptions, string>.Create(opt => opt.ValueReadOnly));
        }

        [Fact]
        public void MapValueAssigns()
        {
            var mapper = PropertyMapper<MyOptions, string>.Create(opt => opt.Value);
            var options = new MyOptions();
            
            mapper.MapValue(options, "test");
            
            options.Value.ShouldBe("test");
        }

        [Fact]
        public void MapValueThrows()
        {
            var mapper = PropertyMapper<MyOptions, string>.Create(opt => opt.Throws);
            var options = new MyOptions();

            Should.Throw<ConfigurationException>(() => mapper.MapValue(options, "fail"));
        }
    }
}