using System.Text.Json.Serialization;

namespace Tagalong.Contracts.Request;

public class VoteRequest
{
    public long ActivityId { get; set; }

    /// <summary>True is a yes vote, false is a no vote.</summary>
    // A bool is false when nothing sets it, so without this attribute a missing "vote"
    // would silently count as a no.
    [JsonRequired]
    public bool Vote { get; set; }
}
