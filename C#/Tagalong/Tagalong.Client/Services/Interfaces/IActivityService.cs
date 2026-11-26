using Tagalong.Contracts.DTOs;
namespace Tagalong.Client.Services.Interfaces;

public interface IActivityService
{ 
        Task<ActivityDto> CreateActivityAsync(
        int tripId, CreateActivityRequest request);
}
