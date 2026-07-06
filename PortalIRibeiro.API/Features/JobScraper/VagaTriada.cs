using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PortalIRibeiro.API.Features.JobScraper.Enums;

namespace PortalIRibeiro.API.Features.JobScraper;

[Table("vagas_triadas")]
public class VagaTriada
{
    [Key]
    [Column("id")]
    public long Id { get; set; } // BIGINT no Postgres

    [Required]
    [Column("feed_id")]
    public int FeedId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("guid_vaga")]
    public string GuidVaga { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("titulo_vaga")]
    public string TituloVaga { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("link_vaga")]
    public string LinkVaga { get; set; } = string.Empty;

    [Column("data_publicacao")]
    public DateTimeOffset? DataPublicacao { get; set; }

    [Column("data_captura")]
    public DateTimeOffset DataCaptura { get; set; } = DateTimeOffset.UtcNow;

    [Column("enviado_por_email")]
    public bool EnviadoPorEmail { get; set; } = false;
    
    [Column(name: "status", TypeName = "varchar(30)")]
    public StatusTriagem Status { get; set; }
    
    [Column(name: "descricao_bruta")]
    public string? DescricaoBruta { get; set; } = string.Empty;
    
    [Column(name: "score_aderencia")]
    public int? ScoreAderencia { get; set; }
    
    [Column(name: "justificativa_ia")]
    public string? JustificativaIa { get; set; }
    
    [Column(name: "gaps_tecnologicos")]
    public string? GapsTecnologicos { get; set; }

    // Propriedade de navegação do EF Core
    [ForeignKey(nameof(FeedId))]
    public virtual RssFeed? Feed { get; set; }
}