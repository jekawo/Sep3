using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Services;

namespace WebApi.Tests.Extensions;

public class ControllerExtensionsTests
{
    // ControllerBase is abstract, so we need an empty subclass. The HttpContext is set up
    // because methods like Problem() use it.
    private class TestController : ControllerBase;

    private readonly TestController _controller = new()
    {
        ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
    };

    [Theory]
    [InlineData(ServiceStatus.NotFound, 404)]
    [InlineData(ServiceStatus.Conflict, 409)]
    [InlineData(ServiceStatus.Invalid, 422)]
    public void Failure_ReturnsProblemWithMatchingStatusAndMessage(ServiceStatus status, int expected)
    {
        var response = _controller.Failure(new ServiceResult(status, "reason"));

        var result = Assert.IsType<ObjectResult>(response);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(expected, problem.Status);
        Assert.Equal("reason", problem.Detail);
    }

    [Fact]
    public void Failure_ReturnsForbidForForbidden() =>
        Assert.IsType<ForbidResult>(_controller.Failure(ServiceResult.Forbidden()));

    [Fact]
    public void Failure_ThrowsForSuccessfulResult() =>
        Assert.Throws<InvalidOperationException>(() => _controller.Failure(ServiceResult.Success()));
}
