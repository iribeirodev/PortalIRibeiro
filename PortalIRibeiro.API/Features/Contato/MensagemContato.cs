using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Features.Contato;

[Table("mensagens_contato")]
public class MensagemContato
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(150)]
    [Column("assunto")]
    public string? Assunto { get; set; }

    [Required]
    [Column("mensagem")]
    public string Mensagem { get; set; } = string.Empty;

    [Column("data_envio")]
    public DateTimeOffset DataEnvio { get; set; } = DateTimeOffset.UtcNow;

    [Column("lida")]
    public bool Lida { get; set; } = false;
}