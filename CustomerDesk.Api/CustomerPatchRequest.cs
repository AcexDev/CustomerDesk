using System.ComponentModel.DataAnnotations;

namespace CustomerDesk.Api;

public class CustomerPatchRequest
{
    [StringLength(100)]
    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }
}
