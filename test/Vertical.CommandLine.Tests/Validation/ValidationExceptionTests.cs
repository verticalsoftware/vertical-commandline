// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using Shouldly;
using Vertical.CommandLine.Validation;
using Xunit;

namespace Vertical.CommandLine.Tests.Validation
{
    public class ValidationExceptionTests
    {
        private readonly Exception _exception = new Exception();

        [Fact]
        public void ConstructAssignsArgumentValue()
        {
            var ex = new ValidationException(null, "context", null);
            ex.Context.ShouldBe("context");
        }

        [Fact]
        public void ConstructAssignsValue()
        {
            var ex = new ValidationException(null, null, "value");
            ex.ArgumentValue.ShouldBe("value");
        }
    }
}