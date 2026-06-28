using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

[Table("projetos")]
public class Projeto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Column("tecnologias")]
    public string[] Tecnologias { get; set; } = [];

    [MaxLength(255)]
    [Column("url_imagem")]
    public string? UrlImagem { get; set; }

    [MaxLength(255)]
    [Column("url_github")]
    public string? UrlGithub { get; set; }

    [MaxLength(255)]
    [Column("url_demonstracao")]
    public string? UrlDemonstracao { get; set; }

    [Column("data_criacao")]
    public DateTimeOffset DataCriacao { get; set; } = DateTimeOffset.UtcNow;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}