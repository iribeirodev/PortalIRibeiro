using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

[Table("projects")]
public class Project
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("technologies")]
    public string[] Technologies { get; set; } = [];

    [MaxLength(255)]
    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [MaxLength(255)]
    [Column("github_url")]
    public string? GitHubUrl { get; set; }

    [MaxLength(255)]
    [Column("demo_url")]
    public string? DemoUrl { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
