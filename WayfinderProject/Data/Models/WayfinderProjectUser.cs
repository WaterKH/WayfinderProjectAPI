using Microsoft.AspNetCore.Identity;

namespace WayfinderProject.Data.Models;

// Add profile data for application users by adding properties to the WayfinderProjectUser class
public class WayfinderProjectUser : IdentityUser
{
    public int ApiCallQuota { get; set; } = 5000; // per month
    public string PatreonAccessToken { get; set; } = string.Empty;
    public string PatreonRefreshToken { get; set; } = string.Empty;
}