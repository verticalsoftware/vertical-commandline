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
    public class DelegateValidatorTests
    {
        [Fact]
        public void ConstructWithNullFunctionThrows()
        {
            Should.Throw<ArgumentNullException>(() => new DelegateValidator<string, string>("obj", null, null, null));
        }

        [Fact]
        public void ConstructAssignsDelegate()
        {
            var invoked = false;
            var instance = new DelegateValidator<string, string>(null, (_, __) => invoked = true, null, null);

            instance.Validate(null);
            invoked.ShouldBeTrue();
        }
    }
}
