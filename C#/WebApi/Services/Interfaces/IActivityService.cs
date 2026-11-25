using Tagalong.Contracts.Request;
using Tagalong.Contracts.Response;

namespace WebApi.Services.Interfaces;

/// <summary>All operations require the user to be a member of the activity's trip.</summary>
public interface IActivityService
{
    Task<ServiceResult<ActivityResponse>> Add(long userId, CreateActivityRequest request);
    Task<ServiceResult<ActivityResponse>> Update(long userId, long activityId, UpdateActivityRequest request);
    Task<ServiceResult<IReadOnlyList<ActivityResponse>>> GetForTrip(long userId, long tripId);
    Task<ServiceResult<ActivityResponse>> GetById(long userId, long activityId);
    Task<ServiceResult> Vote(long userId, long activityId, bool approve);
}
