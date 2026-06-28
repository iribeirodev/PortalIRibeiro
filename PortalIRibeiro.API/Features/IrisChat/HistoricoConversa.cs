using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Features.IrisChat;

/// <summary>
/// Entidade que representa o histórico de conversas entre o usuário e a IA Íris, armazenando perguntas, respostas e metadados da interação.
/// </summary>
[Table("historico_conversas")]
public class HistoricoConversa
{
    [Key]
    [Column("id")]
    public long Id { get; set; } // BIGINT no Postgres

    [Required]
    [Column("sessao_id")]
    public Guid SessaoId { get; set; }

    [Required]
    [Column("pergunta_usuario")]
    public string PerguntaUsuario { get; set; } = string.Empty;

    [Required]
    [Column("resposta_ia")]
    public string RespostaIa { get; set; } = string.Empty;

    [Column("data_interacao")]
    public DateTimeOffset DataInteracao { get; set; } = DateTimeOffset.UtcNow;
}