/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Machine.Specifications;
using Dolittle.Validation;

namespace Dolittle.FluentValidation.Commands.for_CommandBusinessValidatorFor
{
    public class when_validating_a_valid_command : given.a_command_business_validator
    {
        static IEnumerable<ValidationResult> results;

        Establish context = () =>
                                {
                                    simple_command.SomeString = "Something, something, something, Dark Side";
                                    simple_command.SomeInt = 42;
                                };

        Because of = () => results = simple_command_business_validator.ValidateFor(simple_command);

        It should_have_no_invalid_properties = () => results.ShouldBeEmpty();
    }
}