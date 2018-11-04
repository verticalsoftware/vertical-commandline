// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Vertical.CommandLine.Conversion;
using Xunit;

namespace Vertical.CommandLine.Tests.Conversion
{
    public class DictionaryConverterTests
    {
        [Fact]
        public void ConstructWithNullValueThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DictionaryConverter<int>(null, null));
        }

        [Fact]
        public void ConstructUsesGivenComparer()
        {
            new DictionaryConverter<int>(new[] { new KeyValuePair<string, int>("ten", 10) },
                StringComparer.OrdinalIgnoreCase)
                .Convert("TEN").ShouldBe(10);
        }

        [Fact]
        public void ConstructWithNullComparerThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DictionaryConverter<string>(
                Enumerable.Empty<KeyValuePair<string, string>>(), null));
        }

        [Fact]
        public void ConvertWithInvalidKeyThrowsKeyNotFound()
        {
            Should.Throw<ArgumentException>(() => new DictionaryConverter<int>(
                new[] { new KeyValuePair<string, int>("one", 1) },
                StringComparer.Ordinal).Convert("two"));
        }
    }
}
