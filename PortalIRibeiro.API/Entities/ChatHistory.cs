using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

/// <summary>
/// Histórico da conversa entre o usuário e a Íris (perguntas, respostas e metadados).
/// </summary>
[Table("chat_history")]
public class ChatHistory
{
    [Key]
    [Column("id")]
    public long Id { get; set; } // BIGINT no Postgres

    [Required]
    [Column("session_id")]
    public Guid SessionId { get; set; }

    [Required]
    [Column("user_question")]
    public string UserQuestion { get; set; } = string.Empty;

    [Required]
    [Column("ai_response")]
    public string AiResponse { get; set; } = string.Empty;

    [Column("interaction_date")]
    public DateTimeOffset InteractionDate { get; set; } = DateTimeOffset.UtcNow;
}
