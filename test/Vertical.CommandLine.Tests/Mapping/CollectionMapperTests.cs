// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Mapping;
using Xunit;

namespace Vertical.CommandLine.Tests.Mapping
{
    public class CollectionMapperTests
    {
        private class MyOptions
        {
            public ICollection<int> Collection { get; set; } = new List<int>();
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            CollectionMapper<MyOptions, int>.Create(opt => opt.Collection).ShouldNotBeNull();
        }

        [Fact]
        public void CreateReturnsMultivaluedMapper()
        {
            CollectionMapper<MyOptions, int>.Create(opt => opt.Collection).MultiValued.ShouldBeTrue();
        }

        [Fact]
        public void MapValueAdds()
        {
            var instance = CollectionMapper<MyOptions, int>.Create(opt => opt.Collection);
            var options = new MyOptions();
            
            instance.MapValue(options, 10);
            options.Collection.Single().ShouldBe(10);
        }

        [Fact]
        public void MapValueWithNullCollectionThrows()
        {
            var myOptions = new MyOptions
            {
                Collection = null
            };
            var instance = CollectionMapper<MyOptions, int>.Create(opt => opt.Collection);
            Should.Throw<ConfigurationException>(() => instance.MapValue(myOptions, 0));
        }

        [Fact]
        public void MapValueWithFaultyAddThrows()
        {
            var collectionMock = new Mock<ICollection<int>>();
            collectionMock.Setup(m => m.Add(It.IsAny<int>())).Throws<Exception>();
            var myOptions = new MyOptions
            {
                Collection = collectionMock.Object
            };
            var instance = CollectionMapper<MyOptions, int>.Create(opt => opt.Collection);
            Should.Throw<ConfigurationException>(() => instance.MapValue(myOptions, 0));
        }
    }
}