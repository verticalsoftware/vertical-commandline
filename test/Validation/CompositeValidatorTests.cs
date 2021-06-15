// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Xunit;
using Vertical.CommandLine.Validation;
using Moq;

namespace Vertical.CommandLine.Tests.Validation
{
    public class CompositeValidatorTests
    {
        private readonly Mock<IValidator<int>> _validator1 = new Mock<IValidator<int>>();
        private readonly Mock<IValidator<int>> _validator2 = new Mock<IValidator<int>>();
        private const int IntValue = 0;

        public CompositeValidatorTests()
        {
            _validator1.Setup(m => m.Validate(IntValue)).Returns(true).Verifiable();
            _validator2.Setup(m => m.Validate(IntValue)).Returns(true).Verifiable();
        }

        [Fact]
        public void ConstructWithNullValidatorsThrows()
        {
            Should.Throw<ArgumentNullException>(() => new CompositeValidator<int>(null));
        }

        [Fact]
        public void ValidateCallsOnEachSubInstance()
        {
            var validator = new CompositeValidator<int>(_validator1.Object, _validator2.Object);
            validator.Validate(IntValue);
            _validator1.Verify(m => m.Validate(IntValue), Times.Once);
            _validator2.Verify(m => m.Validate(IntValue), Times.Once);
        }

        [Fact]
        public void AppendsAddsToInternalList()
        {
            var validator = new CompositeValidator<int>(_validator1.Object);
            validator.Append(_validator2.Object);
            validator.Validate(IntValue);
            _validator1.Verify(m => m.Validate(IntValue), Times.Once);
            _validator2.Verify(m => m.Validate(IntValue), Times.Once);
        }
    }
}
