// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
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
            Should.Throw<ConfigurationException>(() => Mapper.MapValue(null /* throws NullReference */,
                "context", new object(), new object()));
        }

        [Fact]
        public void MapValuePropagatesUsageException()
        {
            // Verifies https://github.com/verticalsoftware/vertical-commandline/issues/19

            var mapperThatThrowsUsageExceptionMock = new Mock<IMapper<object, object>>();
            mapperThatThrowsUsageExceptionMock.Setup(m => m.MapValue(It.IsAny<object>()
                    , It.IsAny<object>()))
                .Throws(new UsageException("error"));

            Should.Throw<UsageException>(() => Mapper.MapValue(mapperThatThrowsUsageExceptionMock.Object
                , "context", new object(), new object()));
        }
    }
}