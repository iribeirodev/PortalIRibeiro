using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

/// <summary>
/// Entity that represents the conversation history between the user and the Iris AI, storing questions, answers and interaction metadata.
/// </summary>
[Table("historico_conversas")]
public class ChatHistory
{
    [Key]
    [Column("id")]
    public long Id { get; set; } // BIGINT no Postgres

    [Required]
    [Column("sessao_id")]
    public Guid SessionId { get; set; }

    [Required]
    [Column("pergunta_usuario")]
    public string UserQuestion { get; set; } = string.Empty;

    [Required]
    [Column("resposta_ia")]
    public string AiResponse { get; set; } = string.Empty;

    [Column("data_interacao")]
    public DateTimeOffset InteractionDate { get; set; } = DateTimeOffset.UtcNow;
}
