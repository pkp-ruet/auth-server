using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; }
}