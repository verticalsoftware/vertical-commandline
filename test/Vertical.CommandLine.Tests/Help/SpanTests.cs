// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Shouldly;
using Vertical.CommandLine.Help;
using Xunit;

namespace Vertical.CommandLine.Tests.Help
{
    public class SpanTests
    {
        [Fact]
        public void ConstructAssignsProps()
        {
            var span = new Span(2, 3);
            span.Start.ShouldBe(2);
            span.Length.ShouldBe(3);
        }

        [Fact]
        public void EmptyIsZeroStartZeroLength()
        {
            Span.Empty.Start.ShouldBe(0);
            Span.Empty.Length.ShouldBe(0);
        }

        [Fact]
        public void EqualsReturnsExpected()
        {
            Span.Empty.Equals(Span.Empty).ShouldBeTrue();
            Span.Empty.Equals(new Span(0, 1)).ShouldBeFalse();
            Span.Empty.Equals(new Span(1, 0)).ShouldBeFalse();
            Span.Empty.Equals(new Span(1, 1)).ShouldBeFalse();
        }

        [Fact]
        public void OpEqualsReturnsExpected()
        {
            (Span.Empty == new Span(0,0)).ShouldBeTrue();
            (Span.Empty == new Span(1, 1)).ShouldBeFalse();
        }

        [Fact]
        public void OpNotEqualsReturnsExpected()
        {
            (Span.Empty != new Span(0, 0)).ShouldBeFalse();
            (Span.Empty != new Span(1, 1)).ShouldBeTrue();
        }

        [Fact]
        public void GetHashCodeReturnsSameForEqualValues()
        {
            new Span(10, 20).GetHashCode().ShouldBe(new Span(10, 20).GetHashCode());
        }

        [Fact]
        public void EqualsReturnsFalseForNonSpan()
        {
            Span.Empty.Equals("string").ShouldBeFalse();
        }

        [Fact]
        public void EqualsReturnsFalseForNull()
        {
            Span.Empty.Equals(null).ShouldBeFalse();
        }
    }
}