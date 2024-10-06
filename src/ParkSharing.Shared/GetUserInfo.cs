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
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public string SpotName { get; set; }
    }
}
