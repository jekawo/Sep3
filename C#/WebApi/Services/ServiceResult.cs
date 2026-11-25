namespace WebApi.Services;

public enum ServiceStatus
{
    Success,
    NotFound,
    Forbidden,
    Conflict,
    Invalid
}

public record ServiceResult(ServiceStatus Status, string? Error = null)
{
    public bool Succeeded => Status == ServiceStatus.Success;

    public static ServiceResult Success() => new(ServiceStatus.Success);
    public static ServiceResult NotFound(string error) => new(ServiceStatus.NotFound, error);
    public static ServiceResult Forbidden() => new(ServiceStatus.Forbidden);
    public static ServiceResult Conflict(string error) => new(ServiceStatus.Conflict, error);
    public static ServiceResult Invalid(string error) => new(ServiceStatus.Invalid, error);
}

public record ServiceResult<T> : ServiceResult
{
    private ServiceResult(ServiceStatus status, T? value, string? error) : base(status, error)
    {
        Value = value;
    }

    /// <summary>Set when the result succeeded.</summary>
    public T? Value { get; }

    public static implicit operator ServiceResult<T>(T value) => new(ServiceStatus.Success, value, null);

    public static ServiceResult<T> Failure(ServiceResult failure) => new(failure.Status, default, failure.Error);
}
