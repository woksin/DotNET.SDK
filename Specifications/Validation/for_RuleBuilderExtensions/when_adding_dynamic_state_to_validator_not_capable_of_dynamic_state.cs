﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using FluentValidation;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using Dolittle.Validation;

namespace Dolittle.FluentValidation.for_RuleBuilderExtensions
{
    public class when_adding_dynamic_state_to_validator_not_capable_of_dynamic_state
    {
        static Mock<AbstractValidator<object>> validator_mock;
        static Exception exception;

        Establish context = () => validator_mock = new Mock<AbstractValidator<object>>();

        Because of = () => exception = Catch.Exception(() => validator_mock.Object.RuleFor(o => o).NotNull().WithDynamicStateFrom(o => o));

        It should_throw_invalid_validator_type_exception = () => exception.ShouldBeOfExactType<InvalidValidatorTypeException>();
    }
}
