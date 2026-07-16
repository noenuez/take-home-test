using Fundo.Applications.Data.Auditing;

namespace Fundo.Applications.Data.Models;

public abstract class BaseEntity : IAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CreatedBy { get; set; } = "system";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
