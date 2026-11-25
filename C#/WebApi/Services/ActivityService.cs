using Tagalong.Contracts.Request;
using Tagalong.Contracts.Response;
using WebApi.Gateways.Interfaces;
using WebApi.Models.Trips;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class ActivityService(ITripService trips, IActivityGateway activities, IVoteGateway votes) : IActivityService
{
    public async Task<ServiceResult<ActivityResponse>> Add(long userId, CreateActivityRequest request)
    {
        var access = await trips.GetTripForMember(userId, request.TripId);

        if (!access.Succeeded)
        {
            return ServiceResult<ActivityResponse>.Failure(access);
        }

        var activity = await activities.Create(new ActivityCreation(
            request.TripId,
            request.Title,
            request.Description,
            request.Location,
            request.StartTime,
            request.EndTime));

        return ToResponse(activity, [], userId);
    }

    public async Task<ServiceResult<ActivityResponse>> Update(long userId, long activityId,
        UpdateActivityRequest request)
    {
        var access = await GetActivityForMember(userId, activityId);

        if (!access.Succeeded)
        {
            return ServiceResult<ActivityResponse>.Failure(access);
        }

        var activity = access.Value!;
        var startTime = request.StartTime ?? activity.StartTime;
        var endTime = request.EndTime ?? activity.EndTime;

        // Validate that the end time is not before the start time.
        if (endTime < startTime)
        {
            return ServiceResult<ActivityResponse>.Failure(
                ServiceResult.Invalid("End time cannot be before start time."));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            activity.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            activity.Description = request.Description;
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            activity.Location = request.Location;
        }

        activity.StartTime = startTime;
        activity.EndTime = endTime;

        await activities.Update(activity);

        return ToResponse(activity, await votes.GetForActivity(activityId), userId);
    }

    public async Task<ServiceResult<IReadOnlyList<ActivityResponse>>> GetForTrip(long userId, long tripId)
    {
        var access = await trips.GetTripForMember(userId, tripId);

        if (!access.Succeeded)
        {
            return ServiceResult<IReadOnlyList<ActivityResponse>>.Failure(access);
        }

        var tripActivities = await activities.GetForTrip(tripId);
        var tripVotes = (await votes.GetForTrip(tripId)).ToLookup(vote => vote.ActivityId);

        return tripActivities
            .Select(activity => ToResponse(activity, tripVotes[activity.Id], userId))
            .ToList();
    }

    public async Task<ServiceResult<ActivityResponse>> GetById(long userId, long activityId)
    {
        var access = await GetActivityForMember(userId, activityId);

        if (!access.Succeeded)
        {
            return ServiceResult<ActivityResponse>.Failure(access);
        }

        return ToResponse(access.Value!, await votes.GetForActivity(activityId), userId);
    }

    public async Task<ServiceResult> Vote(long userId, long activityId, bool approve)
    {
        var access = await GetActivityForMember(userId, activityId);

        if (!access.Succeeded)
        {
            return access;
        }

        await votes.Save(new Vote(activityId, userId, approve));

        return ServiceResult.Success();
    }

    private async Task<ServiceResult<TripActivity>> GetActivityForMember(long userId, long activityId)
    {
        var activity = await activities.GetById(activityId);

        if (activity is null)
        {
            return ServiceResult<TripActivity>.Failure(ServiceResult.NotFound("The activity does not exist."));
        }

        var access = await trips.GetTripForMember(userId, activity.TripId);

        if (!access.Succeeded)
        {
            return ServiceResult<TripActivity>.Failure(access);
        }

        return activity;
    }

    private static ActivityResponse ToResponse(TripActivity activity, IEnumerable<Vote> activityVotes, long userId)
    {
        var all = activityVotes.ToList();

        return new ActivityResponse(
            activity.Id,
            activity.TripId,
            activity.Title,
            activity.Description,
            activity.Location,
            activity.StartTime,
            activity.EndTime,
            all.Count(vote => vote.Approve),
            all.Count(vote => !vote.Approve),
            all.FirstOrDefault(vote => vote.UserId == userId)?.Approve);
    }
}
