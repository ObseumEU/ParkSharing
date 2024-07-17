using MassTransit;
using ParkSharing.Contracts;

namespace ParkSharing.Notification.Server.Services
{
    public class UserInfoService : IUserInfoService
    {
        IRequestClient<GetUserInfo> _client;

        public UserInfoService(IRequestClient<GetUserInfo> client)
        {
            _client = client;
        }

        public async Task<UserInfoResult> GetUserInfo(string publicSpotId)
        {
            var userInfo = await _client.GetResponse<UserInfoResult>(new GetUserInfo()
            {
                PublicSpotId = publicSpotId
            });

            return userInfo.Message;
        }
    }
}
