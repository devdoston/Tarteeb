//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using Tarteeb.Api.Models.Foundations.Securities.Exceptions;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;

namespace Tarteeb.Api.Services.Foundations.Securities
{
    public partial class SecurityService
    {
        private void ValidateUser(User user)
        {
            ValidateUserNotNull(user);

            ValidateUser(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)));
        }

        private void ValidatePassword(string password)
        {
            ValidatePassword(
                (Rule: IsInvalid(password), Parameter: "ValueComplexity"),
                (Rule: IsLessThan8Chars(password), Parameter: "LengthComplexity"),
                (Rule: HasNoUppercase(password), Parameter: "UppercaseComplexity"),
                (Rule: HasNoSymbol(password), Parameter: "SymbolComplexity"),
                (Rule: HasNoDigit(password), Parameter: "DigitComplexity"));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private dynamic IsLessThan8Chars(string password) => new
        {
            Condition = IsLessThan8CharsRule(password),
            Message = "At least 8 characters is required"
        };

        private dynamic HasNoUppercase(string password) => new
        {
            Condition = HasNoUppercaseRule(password),
            Message = "At least one capital letter is required"
        };

        private dynamic HasNoSymbol(string password) => new
        {
            Condition = HasNoSymbolRule(password),
            Message = "At least one symbol is required",
        };

        private dynamic HasNoDigit(string password) => new
        {
            Condition = HasNoDigitRule(password),
            Message = "At least one digit is required"
        };

        private static void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }
        private bool HasNoUppercaseRule(string password) =>
            !password.Any(char.IsUpper);

        private bool HasNoDigitRule(string password) =>
            password.All(character => !char.IsDigit(character));

        private bool HasNoSymbolRule(string password) =>
            password.All(character => !(char.IsLetterOrDigit(character) || !char.IsWhiteSpace(character)));

        private bool IsLessThan8CharsRule(string password) =>
            password.Length < 8;

        private static void ValidateUser(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }

        private static void ValidatePassword(params (dynamic Rule, string Parameter)[] validations)
        {
            var insecurePasswordException = new InsecurePasswordException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    insecurePasswordException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            insecurePasswordException.ThrowIfContainsErrors();
        }
    }
}
