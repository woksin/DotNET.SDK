﻿Bifrost.namespace("Bifrost.commands", {
    commandValidationService: Bifrost.Singleton(function (validationService) {
        var self = this;
        this.validationService = validationService;

        function shouldSkipProperty(target, property) {
            if (!target.hasOwnProperty(property)) return true;
            if (ko.isObservable(target[property])) return false;
            if (typeof target[property] === "function") return true;
            if (target[property] instanceof Bifrost.Type) return true;
            if (property == "_type") return true;
            if (property == "_namespace") return true;

            return false;
        }


        function extendProperties(target) {
            for (var property in target) {
                if (shouldSkipProperty(target, property)) continue;

                if (ko.isObservable(target[property])) {
                    target[property].extend({ validation: {} });
                } else if (typeof target[property] === "object") {
                    extendProperties(target[property]);
                }
            }
        }

        function validatePropertiesFor(target, result) {
            for (var property in target) {
                if (shouldSkipProperty(target, property)) continue;

                if (typeof target[property].validator !== "undefined") {
                    target[property].validator.validate(target[property]());

                    if (target[property].validator.isValid() == false) {
                        result.valid = false;
                        return;
                    }

                } else if (typeof target[property] === "object") {
                    validatePropertiesFor(target[property], result);
                }
            }
        }


        function applyValidationMessageToMembers(command, members, message) {
            for (var memberIndex = 0; memberIndex < members.length; memberIndex++) {
                var path = members[memberIndex].split(".");
                var property = null;
                var target = command;
                $.each(path, function (pathIndex, member) {
                    property = member.toCamelCase();
                    if (property in target) {
                        if (typeof target[property] === "object") {
                            target = target[property];
                        }
                    }
                });

                if (property != null) {
                    var member = target[property];

                    if (typeof member.validator !== "undefined") {
                        member.validator.isValid(false);
                        member.validator.message(message);
                    }
                }

            }
        }

        this.applyValidationResultToProperties = function (command, validationResults) {

            for (var i = 0; i < validationResults.length; i++) {
                var validationResult = validationResults[i];
                var message = validationResult.errorMessage;
                var memberNames = validationResult.memberNames;
                if (memberNames.length > 0) {
                    applyValidationMessageToMembers(command, memberNames, message);
                }
            }
        };

        this.validate = function (command) {
            var result = { valid: true };
            validatePropertiesFor(command, result);
            return result;
        };

        this.applyRulesTo = function (command) {
            extendProperties(command);
            self.validationService.getForCommand(command.name).continueWith(function (rules) {
                for (var rule in rules) {
                    var path = rule.split(".");
                    var member = command;
                    for (var i in path) {

                        var step = path[i];
                        if (step in member) {
                            member = member[step];
                        } else {
                            throw "Error applying validation rules: " + step + " is not a member of " + member + " (" + rule + ")";
                        }
                    }

                    if (member.validator !== undefined) {

                        member.validator.setOptions(rules[rule]);
                    }
                }
            });
        };
    })
});