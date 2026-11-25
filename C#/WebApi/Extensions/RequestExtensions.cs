using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tagalong.Contracts.Request;

namespace WebApi.Extensions;

public static class RequestExtensions
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{6,}$");

    extension(CreateUserRequest request)
    {
        /// <summary>
        /// Validates the request and records each failure in <paramref name="modelState"/>.
        /// Returns true when the request is valid and the model state has no errors.
        /// </summary>
        public bool Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                modelState.AddModelError(nameof(request.UserName), "User name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || !PasswordRegex.IsMatch(request.Password))
            {
                modelState.AddModelError(nameof(request.Password),
                    "Password must be at least 6 characters with a lowercase letter, an uppercase letter, a digit and a symbol.");
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !EmailRegex.IsMatch(request.Email))
            {
                modelState.AddModelError(nameof(request.Email), "A valid email address is required.");
            }

            return modelState.IsValid;
        }
    }
}
