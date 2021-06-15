// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using Moq;
using Vertical.CommandLine.Conversion;
using Vertical.CommandLine.Mapping;
using Vertical.CommandLine.Validation;

namespace Vertical.CommandLine.Tests.Parsing
{
    public abstract class ParserTestsBase<TValue>
    {
        protected Mock<IValueConverter<TValue>> ConverterMock { get; } = new Mock<IValueConverter<TValue>>();
        protected Mock<IMapper<object, TValue>> MapperMock { get; } = new Mock<IMapper<object, TValue>>();
        protected Mock<IValidator<TValue>> ValidatorMock { get; } = new Mock<IValidator<TValue>>();

        protected ParserTestsBase()
        {
            ConverterMock.Setup(m => m.Convert(It.IsAny<string>())).Verifiable();
            MapperMock.Setup(m => m.MapValue(It.IsAny<object>(), It.IsAny<TValue>())).Verifiable();
            ValidatorMock.Setup(m => m.Validate(It.IsAny<TValue>())).Returns(true).Verifiable();
        }

        protected void VerifyMocks()
        {
            ConverterMock.Verify(m => m.Convert(It.IsAny<string>()), Times.Once);
            MapperMock.Verify(m => m.MapValue(It.IsAny<object>(), It.IsAny<TValue>()), Times.Once);
            ValidatorMock.Verify(m => m.Validate(It.IsAny<TValue>()), Times.Once);
        }
    }
}
