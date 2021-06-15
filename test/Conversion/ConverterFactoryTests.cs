// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using System;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Conversion;
using Xunit;

namespace Vertical.CommandLine.Tests.Conversion
{
    public class ConverterFactoryTests
    {
        public class ConstructableByString
        {
            public ConstructableByString(string _) { }
        }

        [Fact]
        public void CreateOrThrowForUnsupportedTypeThrows()
        {
            Should.Throw<ConfigurationException>(() => ConverterFactory.CreateOrThrow<CastConverter<int>>());
        }

        [Fact]
        public void CreateOrThrowForParseTypeDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<byte>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<short>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<int>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<long>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<float>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<double>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<decimal>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<DateTime>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<TimeSpan>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateOrThrowForEnumTypeDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<DayOfWeek>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateOrThrowForNullableParseTypeDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<byte?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<short?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<int?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<long?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<float?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<double?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<decimal?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<DateTime?>().ShouldNotBeNull();
            ConverterFactory.CreateOrThrow<TimeSpan?>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateOrThrowForNullableEnumTypeDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<DayOfWeek?>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateOrThrowForConvertibleTypeDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<ConstructableByString>().ShouldNotBeNull();
        }

        [Fact]
        public void CreateOrThrowForStringDoesNotThrow()
        {
            ConverterFactory.CreateOrThrow<string>().ShouldNotBeNull();
        }
    }
}
