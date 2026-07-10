using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

[Table("rss_feeds")]
public class RssFeed
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome_fonte")]
    public string NomeFonte { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("url_feed")]
    public string UrlFeed { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("ultima_sincronizacao")]
    public DateTimeOffset? UltimaSincronizacao { get; set; }

    // Relacionamento 1:N com as vagas triadas
    public virtual ICollection<VagaTriada> VagasTriadas { get; set; } = new List<VagaTriada>();
}