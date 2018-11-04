// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Moq;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Validation;
using static Vertical.CommandLine.Tests.Macros;
using System.Collections.Generic;
using Vertical.CommandLine.Infrastructure;

namespace Vertical.CommandLine.Tests.Configuration
{
    public class ValidationConfigurationTests
    {
        private class OppositeComparer : IComparer<int>
        {
            internal static IComparer<int> Instance { get; } = new OppositeComparer();

            public int Compare(int x, int y) => Comparer<int>.Default.Compare(x, y) * -1;
        }

        private class OppositeEqualityComparer : IEqualityComparer<int>
        {
            internal static IEqualityComparer<int> Instance { get; } = new OppositeEqualityComparer();

            public bool Equals(int x, int y) => !EqualityComparer<int>.Default.Equals(x, y);

            public int GetHashCode(int obj) => EqualityComparer<int>.Default.GetHashCode(obj);
        }

        private const string ErrorMessage = "error";

        [Fact]
        public void UsingForwardsState()
        {
            const int ten = 10;
            var fn = new Validation<int, int>((state, _) =>
            {
                state.ShouldBe(ten);
                return true;
            });
            var validator = GetInstance(cfg => cfg.Using(ten, fn, null));
            validator.Validate(0);
        }

        [Fact]
        public void UsingForwardsArg()
        {
            const int ten = 10;
            var validator = GetInstance(cfg => cfg.Using(value =>
            {
                value.ShouldBe(ten);
                return true;
            }));
            validator.Validate(ten);
        }

        [Fact]
        public void UsingInvokesMessageFormat()
        {
            var invoked = false;
            var validator = GetInstance(cfg => cfg.Using(_ => false, _ =>
            {
                invoked = true;
                return "error";
            }));
            validator.GetError(0).ShouldBe("error");
            invoked.ShouldBeTrue();
        }

        [Fact]
        public void UsingWithStateInvokesDefaultMessageFormat()
        {
            var validator = GetInstance(cfg => cfg.Using(0, (state, value) => false));
            validator.GetError(0).ShouldBe(Common.InvalidValue);
        }

        [Theory]
        [InlineData(0, -1, false), InlineData(0, 0, true), InlineData(0, 1, true)]
        public void LessThrowsWhenExpected(int state, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.Less(state));
            validator.Validate(value).ShouldBe(!error);
        }

        [Fact]
        public void LessUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.Less(0, OppositeComparer.Instance));
            validator.Validate(-1).ShouldBeFalse();
        }

        [Fact]
        public void LessUsesDefaultMessageFormat()
        {
            var validator = GetInstance(cfg => cfg.Less(0));
            validator.GetError(1).ShouldBe(Common.LessThanMessage(0));
        }

        [Theory]
        [InlineData(0, -1, false), InlineData(0, 0, false), InlineData(0, 1, true)]
        public void LessOrEqualThrowsWhenExpected(int state, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.LessOrEqual(state));
            validator.Validate(value).ShouldBe(!error);
        }

        [Fact]
        public void LessOrEqualUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.LessOrEqual(0, OppositeComparer.Instance));
            validator.Validate(-1).ShouldBeFalse();
        }

        [Theory]
        [InlineData(0, -1, true), InlineData(0, 0, true), InlineData(0, 1, false)]
        public void GreaterThrowsWhenExpected(int state, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.Greater(state));
            validator.Validate(value).ShouldBe(!error);
        }

        [Fact]
        public void GreaterUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.Greater(0, OppositeComparer.Instance));
            validator.Validate(1).ShouldBeFalse();
        }

        [Fact]
        public void GreaterUsesDefaultMessageFormat()
        {
            var validator = GetInstance(cfg => cfg.Greater(0));
            validator.GetError(-1).ShouldBe(Common.GreaterThanMessage(0));
        }

        [Theory]
        [InlineData(0, -1, true), InlineData(0, 0, false), InlineData(0, 1, false)]
        public void GreaterOrEqualThrowsWhenExpected(int state, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.GreaterOrEqual(state));
            validator.Validate(value).ShouldBe(!error);
        }

        [Fact]
        public void GreaterOrEqualUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.GreaterOrEqual(0, OppositeComparer.Instance));
            validator.Validate(1).ShouldBeFalse();
        }

        [Theory]
        [InlineData(-1, 1, 0, true)]
        [InlineData(-1, 1, -1, true)]
        [InlineData(-1, 1, 1, true)]
        [InlineData(-1, 1, -2, false)]
        [InlineData(-1, 1, 2, false)]
        public void BetweenThrowsWhenExpected(int min, int max, int value, bool valid)
        {
            var validator = GetInstance(cfg => cfg.Between(min, max));
            validator.Validate(value).ShouldBe(valid);
        }

        [Fact]
        public void BetweenUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.Between(0, 1, OppositeComparer.Instance));
            validator.Validate(0).ShouldBeFalse();
        }

        [Theory, MemberData(nameof(InTheories))]
        public void InThrowsWhenExpected(int[] set, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.In(set));
            validator.Validate(value).ShouldBe(!error);
        }

        public static IEnumerable<object[]> InTheories => Scenarios(
            Scenario(new[] { 1, 2, 3 }, 1, false),
            Scenario(new[] { 1, 2, 3 }, 0, true));

        [Fact]
        public void InUsesProvidedComparer()
        {
            var validator = GetInstance(cfg => cfg.In(new[] { 1 }, OppositeEqualityComparer.Instance));
            validator.Validate(1).ShouldBeFalse();
        }

        [Theory]
        [InlineData("[0-9]", 1, false)]
        [InlineData("[0-9]{2}", 1, true)]
        public void MatchesThrowsWhenExpected(string pattern, int value, bool error)
        {
            var validator = GetInstance(cfg => cfg.Matches(pattern));
            validator.Validate(value).ShouldBe(!error);
        }

        [Theory, MemberData(nameof(MessageFormatTheories))]
        public void ConfigurationUsesProvidedMessageFormat(IValidator<int> validator, int value)
        {
            validator.GetError(value).ShouldBe(ErrorMessage);
        }

        public static IEnumerable<object[]> MessageFormatTheories => Scenarios(
            Scenario(GetInstance(cfg => cfg.Less(0, messageFormat: (_,__) => ErrorMessage)), 1),
            Scenario(GetInstance(cfg => cfg.LessOrEqual(0, messageFormat: (_,__) => ErrorMessage)), 1),
            Scenario(GetInstance(cfg => cfg.Greater(0, messageFormat: (_,__) => ErrorMessage)), -1),
            Scenario(GetInstance(cfg => cfg.GreaterOrEqual(0, messageFormat: (_,__) => ErrorMessage)), -1),
            Scenario(GetInstance(cfg => cfg.Between(0, 1, messageFormat: (_,__) => ErrorMessage)), -1),
            Scenario(GetInstance(cfg => cfg.In(new[] {0}, messageFormat: (_,__) => ErrorMessage)), 1),
            Scenario(GetInstance(cfg => cfg.Matches("[1-2]", messageFormat: (_,__) => ErrorMessage)), 0)
        );

        [Theory, MemberData(nameof(DefaultMessageFormatTheories))]
        public void ConfigurationUsesDefaultMessageFormat(IValidator<int> validator, int value, string expected)
        {
            validator.GetError(value).ShouldBe(expected);
        }

        public static IEnumerable<object[]> DefaultMessageFormatTheories => Scenarios(
            Scenario(GetInstance(cfg => cfg.Less(0)), 1, Common.LessThanMessage(0)),
            Scenario(GetInstance(cfg => cfg.LessOrEqual(0)), 1, Common.LessOrEqualMessage(0)),
            Scenario(GetInstance(cfg => cfg.Greater(0)), -1, Common.GreaterThanMessage(0)),
            Scenario(GetInstance(cfg => cfg.GreaterOrEqual(0)), -1, Common.GreaterEqualMessage(0)),
            Scenario(GetInstance(cfg => cfg.Between(0, 1)), -1, Common.GreaterEqualMessage(0)),
            Scenario(GetInstance(cfg => cfg.In(new[] {0})), 1, Common.OneOfMessage(new[]{0})),
            Scenario(GetInstance(cfg => cfg.Matches("[1-2]")), 0, Common.PatternMessage("[1-2]"))
        );

        private static IValidator<int> GetInstance(Action<ValidationConfiguration<object, int>> configAction)
        {
            IValidator<int> validator = null;
            var mockSink = new Mock<IComponentSink<IValidator<int>>>();

            var config = new ValidationConfiguration<object, int>(null, mockSink.Object);
            mockSink.Setup(m => m.Sink(It.IsAny<IValidator<int>>()))
                .Callback<IValidator<int>>(component => validator = Validator.Combine(validator, component));
            configAction(config);

            return validator;
        }
    }
}
