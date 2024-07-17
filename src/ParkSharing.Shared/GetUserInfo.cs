using System.ComponentModel.DataAnnotations;

namespace ParkSharing.Contracts
{
    public record GetUserInfo
    {
        [Required]
        public string PublicSpotId { get; set; }
    }
    public record UserInfoResult
    {
        public string Email { get; set; }
        public string PublicSpotId { get; set; }
        public string UserId { get; set; }
    }
}
