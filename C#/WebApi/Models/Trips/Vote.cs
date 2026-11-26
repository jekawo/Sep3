namespace WebApi.Models.Trips;

public record Vote(long ActivityId, long UserId, bool Approve);
