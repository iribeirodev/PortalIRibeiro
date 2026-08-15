using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Entities;

/// <summary>
/// Entity that represents the conversation history between the user and the Iris AI, storing questions, answers and interaction metadata.
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
