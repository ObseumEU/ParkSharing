using ParkSharing.Contracts;

namespace ParkSharing.Notification.Server.Services
{
    public interface IUserInfoService
    {
        Task<UserInfoResult> GetUserInfo(string publicSpotId);
    }
}