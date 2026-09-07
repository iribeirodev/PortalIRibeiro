using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalIRibeiro.API.Features.Contact;

[Table("mensagens_contato")]
public class ContactMessage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(150)]
    [Column("assunto")]
    public string? Subject { get; set; }

    [Required]
    [Column("mensagem")]
    public string Message { get; set; } = string.Empty;

    [Column("data_envio")]
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("lida")]
    public bool IsRead { get; set; } = false;
}
