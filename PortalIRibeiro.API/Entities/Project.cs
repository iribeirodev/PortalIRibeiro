using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

[Table("projetos")]
public class Project
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("titulo")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("descricao")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("tecnologias")]
    public string[] Technologies { get; set; } = [];

    [MaxLength(255)]
    [Column("url_imagem")]
    public string? ImageUrl { get; set; }

    [MaxLength(255)]
    [Column("url_github")]
    public string? GitHubUrl { get; set; }

    [MaxLength(255)]
    [Column("url_demonstracao")]
    public string? DemoUrl { get; set; }

    [Column("data_criacao")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("ativo")]
    public bool IsActive { get; set; } = true;
}
