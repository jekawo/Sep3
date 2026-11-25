namespace Tagalong.Contracts.Request;

public class VoteRequest
{
    public long ActivityId { get; set; }

    /// <summary>True is a yes vote, false is a no vote.</summary>
    public bool? Approve { get; set; }
}
