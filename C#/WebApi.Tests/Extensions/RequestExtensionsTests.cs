using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Models.Trips;
using WebApi.Services;

namespace WebApi.Tests.Extensions;

// [Fact] is a single test. [Theory] runs the same test once for every [InlineData] line.
public class RequestExtensionsTests
{
    private const string ValidPassword = "Abc12!";

    // xUnit creates a new instance of the class for every test, so each test starts with no errors.
    private readonly ModelStateDictionary _modelState = new();

    private static readonly DateTime Noon = new(2030, 1, 1, 12, 0, 0);

    private static CreateUserRequest ValidUser() =>
        new() { Username = "alice", Password = ValidPassword, Email = "alice@example.com" };

    private static CreateTripRequest ValidTrip() => new()
    {
        Name = "Summer",
        Destination = "Rome",
        StartDate = new DateOnly(2030, 7, 1),
        EndDate = new DateOnly(2030, 7, 8)
    };

    private void AssertOnlyError(bool valid, string key)
    {
        Assert.False(valid);
        Assert.Equal(key, Assert.Single(_modelState.Keys));
    }

    // CreateUserRequest

    [Fact]
    public void CreateUser_AcceptsValidRequest() => Assert.True(ValidUser().Validate(_modelState));

    [Fact]
    public void CreateUser_ReportsEveryMissingField()
    {
        Assert.False(new CreateUserRequest().Validate(_modelState));
        Assert.Equal(["Email", "Password", "Username"], _modelState.Keys.Order());
    }

    [Theory]
    [InlineData("Ab1!x")] // too short
    [InlineData("abc12!")] // no uppercase letter
    [InlineData("ABC12!")] // no lowercase letter
    [InlineData("Abcde!")] // no digit
    [InlineData("Abc123")] // no symbol
    [InlineData("Abc 12")] // a space is not a symbol
    [InlineData("      ")]
    public void CreateUser_RejectsWeakPassword(string password)
    {
        var request = ValidUser();
        request.Password = password;

        AssertOnlyError(request.Validate(_modelState), nameof(request.Password));
    }

    [Theory]
    [InlineData("alice")]
    [InlineData("alice@example")]
    [InlineData("@example.com")]
    [InlineData("alice@@example.com")]
    [InlineData("al ice@example.com")]
    public void CreateUser_RejectsInvalidEmail(string email)
    {
        var request = ValidUser();
        request.Email = email;

        AssertOnlyError(request.Validate(_modelState), nameof(request.Email));
    }

    // UpdateUserRequest

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdateUser_AllowsEmptyOptionalFields(string? empty)
    {
        var request = new UpdateUserRequest
        {
            PreviousPassword = ValidPassword, Username = empty, Password = empty, Email = empty
        };

        Assert.True(request.Validate(_modelState));
    }

    [Fact]
    public void UpdateUser_RequiresPreviousPassword()
    {
        var request = new UpdateUserRequest { Email = "alice@example.com" };

        AssertOnlyError(request.Validate(_modelState), nameof(request.PreviousPassword));
    }

    [Fact]
    public void UpdateUser_RejectsWeakNewPassword()
    {
        var request = new UpdateUserRequest { PreviousPassword = ValidPassword, Password = "weak" };

        AssertOnlyError(request.Validate(_modelState), nameof(request.Password));
    }

    [Fact]
    public void UpdateUser_RejectsInvalidNewEmail()
    {
        var request = new UpdateUserRequest { PreviousPassword = ValidPassword, Email = "nope" };

        AssertOnlyError(request.Validate(_modelState), nameof(request.Email));
    }

    // LoginRequest

    [Fact]
    public void Login_DoesNotApplyPasswordStrengthRules()
    {
        var request = new LoginRequest { Username = "alice", Password = "x" };

        Assert.True(request.Validate(_modelState));
    }

    [Fact]
    public void Login_RequiresUsernameAndPassword()
    {
        Assert.False(new LoginRequest().Validate(_modelState));
        Assert.Equal(["Password", "Username"], _modelState.Keys.Order());
    }

    // RefreshTokenRequest

    [Fact]
    public void RefreshToken_AcceptsToken() =>
        Assert.True(new RefreshTokenRequest { RefreshToken = "token" }.Validate(_modelState));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void RefreshToken_RequiresToken(string? token)
    {
        var request = new RefreshTokenRequest { RefreshToken = token! };

        AssertOnlyError(request.Validate(_modelState), nameof(request.RefreshToken));
    }

    // CreateTripRequest

    [Fact]
    public void CreateTrip_AcceptsValidRequest() => Assert.True(ValidTrip().Validate(_modelState));

    [Fact]
    public void CreateTrip_AcceptsOneDayTrip()
    {
        var request = ValidTrip();
        request.EndDate = request.StartDate;

        Assert.True(request.Validate(_modelState));
    }

    [Fact]
    public void CreateTrip_ReportsEveryMissingField()
    {
        Assert.False(new CreateTripRequest().Validate(_modelState));
        Assert.Equal(["Destination", "EndDate", "Name", "StartDate"], _modelState.Keys.Order());
    }

    [Fact]
    public void CreateTrip_RejectsEndBeforeStart()
    {
        var request = ValidTrip();
        request.EndDate = request.StartDate!.Value.AddDays(-1);

        AssertOnlyError(request.Validate(_modelState), nameof(request.EndDate));
    }

    // CreateActivityRequest

    [Fact]
    public void CreateActivity_TimesAreOptional()
    {
        var request = new CreateActivityRequest { TripId = 1, Title = "Tour" };

        Assert.True(request.Validate(_modelState));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateActivity_RequiresTripId(long tripId)
    {
        var request = new CreateActivityRequest { TripId = tripId, Title = "Tour" };

        AssertOnlyError(request.Validate(_modelState), nameof(request.TripId));
    }

    [Fact]
    public void CreateActivity_RequiresTitle()
    {
        var request = new CreateActivityRequest { TripId = 1, Title = " " };

        AssertOnlyError(request.Validate(_modelState), nameof(request.Title));
    }

    [Fact]
    public void CreateActivity_RejectsEndBeforeStart()
    {
        var request = new CreateActivityRequest
        {
            TripId = 1, Title = "Tour", StartTime = Noon, EndTime = Noon.AddMinutes(-1)
        };

        AssertOnlyError(request.Validate(_modelState), nameof(request.EndTime));
    }

    // UpdateActivityRequest

    [Fact]
    public void UpdateActivity_AcceptsEmptyRequest() =>
        Assert.True(new UpdateActivityRequest().Validate(_modelState));

    [Fact]
    public void UpdateActivity_RejectsEndBeforeStart()
    {
        var request = new UpdateActivityRequest { StartTime = Noon, EndTime = Noon.AddMinutes(-1) };

        AssertOnlyError(request.Validate(_modelState), nameof(request.EndTime));
    }

    [Theory]
    [InlineData(null, -1, ServiceStatus.Invalid)] // new end before the stored start
    [InlineData(2, null, ServiceStatus.Invalid)] // new start after the stored end
    [InlineData(null, 0, ServiceStatus.Success)] // ending when it starts is allowed
    [InlineData(2, 3, ServiceStatus.Success)] // moving both past the stored times
    [InlineData(null, null, ServiceStatus.Success)]
    public void UpdateActivity_ValidatesTimesAgainstStoredActivity(int? startHours, int? endHours,
        ServiceStatus expected)
    {
        var stored = new TripActivity { Title = "Tour", StartTime = Noon, EndTime = Noon.AddHours(1) };
        // Attributes cannot hold DateTime values, so the data gives hours relative to Noon.
        // "is { } start" means "is not null, and call the value start".
        var request = new UpdateActivityRequest
        {
            StartTime = startHours is { } start ? Noon.AddHours(start) : null,
            EndTime = endHours is { } end ? Noon.AddHours(end) : null
        };

        Assert.Equal(expected, request.ValidateAgainst(stored).Status);
    }

    // VoteRequest

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Vote_AcceptsYesAndNo(bool vote)
    {
        var request = new VoteRequest { ActivityId = 1, Vote = vote };

        Assert.True(request.Validate(_modelState));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Vote_RequiresActivityId(long activityId)
    {
        var request = new VoteRequest { ActivityId = activityId };

        AssertOnlyError(request.Validate(_modelState), nameof(request.ActivityId));
    }
}
