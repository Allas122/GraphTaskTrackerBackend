using System.ComponentModel.DataAnnotations;

namespace GraphTaskTrackerBackend.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    [Required(AllowEmptyStrings = false)]
    public string SecretKey { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string Issuer { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public int ExpirationMinutes { get; set; }
}