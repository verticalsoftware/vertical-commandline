// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Mapping;
using Xunit;

namespace Vertical.CommandLine.Tests.Mapping
{
    public class MapperTests
    {
        [Fact]
        public void MapValueCatchesAndThrowsConfigurationException()
        {
            Should.Throw<ConfigurationException>(() => Mapper.MapValue<object, object>(null /* throws NullReference */,
                "context", new object(), new object()));
        }
    }
}