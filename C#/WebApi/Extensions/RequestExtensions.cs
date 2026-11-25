using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tagalong.Contracts.Request;

namespace WebApi.Extensions;

/// <summary>
/// Each Validate method records its failures in the model state and returns true
/// when the request is valid and the model state has no errors.
/// </summary>
public static class RequestExtensions
{
    private const string PasswordRule =
        "Password must be at least 6 characters with a lowercase letter, an uppercase letter, a digit and a symbol.";

    private const string EmailRule = "A valid email address is required.";

    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{6,}$");

    extension(CreateUserRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            Require(modelState, request.Username, nameof(request.Username), "User name is required.");

            if (string.IsNullOrWhiteSpace(request.Password) || !PasswordRegex.IsMatch(request.Password))
            {
                modelState.AddModelError(nameof(request.Password), PasswordRule);
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !EmailRegex.IsMatch(request.Email))
            {
                modelState.AddModelError(nameof(request.Email), EmailRule);
            }

            return modelState.IsValid;
        }
    }

    extension(UpdateUserRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            Require(modelState, request.PreviousPassword, nameof(request.PreviousPassword),
                "Previous password is required.");

            // Empty fields are left unchanged, so only provided values are checked.
            if (!string.IsNullOrWhiteSpace(request.Password) && !PasswordRegex.IsMatch(request.Password))
            {
                modelState.AddModelError(nameof(request.Password), PasswordRule);
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && !EmailRegex.IsMatch(request.Email))
            {
                modelState.AddModelError(nameof(request.Email), EmailRule);
            }

            return modelState.IsValid;
        }
    }

    extension(LoginRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            Require(modelState, request.Username, nameof(request.Username), "Username is required.");
            Require(modelState, request.Password, nameof(request.Password), "Password is required.");

            return modelState.IsValid;
        }
    }

    extension(CreateTripRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            Require(modelState, request.Name, nameof(request.Name), "Name is required.");
            Require(modelState, request.Destination, nameof(request.Destination), "Destination is required.");

            if (request.StartDate is null)
            {
                modelState.AddModelError(nameof(request.StartDate), "Start date is required.");
            }

            if (request.EndDate is null)
            {
                modelState.AddModelError(nameof(request.EndDate), "End date is required.");
            }

            if (request.EndDate < request.StartDate)
            {
                modelState.AddModelError(nameof(request.EndDate), "End date cannot be before start date.");
            }

            return modelState.IsValid;
        }
    }

    extension(CreateActivityRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            if (request.TripId <= 0)
            {
                modelState.AddModelError(nameof(request.TripId), "Trip id is required.");
            }

            Require(modelState, request.Title, nameof(request.Title), "Title is required.");

            if (request.EndTime < request.StartTime)
            {
                modelState.AddModelError(nameof(request.EndTime), "End time cannot be before start time.");
            }

            return modelState.IsValid;
        }
    }

    extension(VoteRequest request)
    {
        public bool Validate(ModelStateDictionary modelState)
        {
            if (request.ActivityId <= 0)
            {
                modelState.AddModelError(nameof(request.ActivityId), "Activity id is required.");
            }

            return modelState.IsValid;
        }
    }

    private static void Require(ModelStateDictionary modelState, string? value, string key, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            modelState.AddModelError(key, message);
        }
    }
}
