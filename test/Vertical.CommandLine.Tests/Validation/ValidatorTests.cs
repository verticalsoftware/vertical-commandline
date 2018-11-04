// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Moq;
using Shouldly;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Validation;
using Xunit;

namespace Vertical.CommandLine.Tests.Validation
{
    public class ValidatorTests
    {
        [Fact]
        public void StaticValidateThrowsWhenLogicFailed()
        {
            var validator = new DelegateValidator<string, string>(null,
                (_, __) => throw new NullReferenceException(), null, null);

            Should.Throw<ConfigurationException>(() => Validator.Validate(validator, "context", null));
        }

        [Fact]
        public void StaticValidateThrowsWhenValueFailed()
        {
            var validator = new DelegateValidator<string, string>(null,
                (_, __) => false, null, null);

            Should.Throw<ValidationException>(() => Validator.Validate(validator, "context", null));
        }

        [Fact]
        public void ValidateThrowsWhenValidateCoreFalse()
        {
            var validator = new DelegateValidator<string, string>(null,
                (_, __) => throw new ArgumentException(), null, null);

            Should.Throw<ArgumentException>(() => validator.Validate(null));
        }

        [Fact]
        public void GetErrorInvokesFormatter()
        {
            var validator = new DelegateValidator<string, string>("state",
                (_, __) => false,
                (state, value) =>
                {
                    state.ShouldBe("state");
                    value.ShouldBe("value");
                    return "formatted";
                },
                null);

            validator.GetError("value").ShouldBe("formatted");
        }

        [Fact]
        public void ValidateProvidesArgument()
        {
            const string arg = "arg";

            var validator = new DelegateValidator<string, string>(null,
                (_, str) => { str.ShouldBe(arg); return true; }, null, null);

            validator.Validate(arg);
        }

        [Fact]
        public void ValidateProvidesState()
        {
            const string state = "state";

            var validator = new DelegateValidator<string, string>(state,
                (str, _) => { str.ShouldBe(state); return true; }, null, null);

            validator.Validate(null);
        }

        [Fact]
        public void CombineWithNullSecondThrows()
        {
            Should.Throw<ArgumentNullException>(() => Validator.Combine<IValidator<int>>(null, null));
        }

        [Fact]
        public void CombineWithNullFirstReturnsSecond()
        {
            var validator = new Mock<IValidator<int>>().Object;
            Validator.Combine(null, validator).ShouldBe(validator);
        }

        [Fact]
        public void CombineWithNonCompositeReturnsComposite()
        {
            var first = new Mock<IValidator<int>>().Object;
            var second = new Mock<IValidator<int>>().Object;

            Validator.Combine(first, second).ShouldBeOfType<CompositeValidator<int>>();
        }

        [Fact]
        public void CombineWithCompositeAppends()
        {
            var v1Mock = new Mock<IValidator<int>>();
            var v2Mock = new Mock<IValidator<int>>();
            v1Mock.Setup(m => m.Validate(0)).Returns(true).Verifiable();
            v2Mock.Setup(m => m.Validate(0)).Returns(true).Verifiable();
            var composite = new CompositeValidator<int>(v1Mock.Object);
            Validator.Combine(composite, v2Mock.Object);
            composite.Validate(0);
            v1Mock.Verify(m => m.Validate(0), Times.Once);
            v2Mock.Verify(m => m.Validate(0), Times.Once);
        }
    }
}
